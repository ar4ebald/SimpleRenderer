using System;
using System.Runtime.CompilerServices;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public sealed class Canvas
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Pixel[] ColorBuffer { get; private set; }
        public double[] DepthBuffer { get; private set; }

        Point _topLeft, _topRight, _bottomLeft, _bottomRight;

        public Canvas()
        {
            ColorBuffer = Array.Empty<Pixel>();
            DepthBuffer = Array.Empty<double>();

            EnsureSize(0, 0);
        }

        public ref Pixel this[int x, int y] => ref ColorBuffer[y * Width + x];

        public void EnsureSize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            if (Width * Height < width * height)
            {
                var color = ColorBuffer;
                Array.Resize(ref color, width * height);
                ColorBuffer = color;

                var depth = DepthBuffer;
                Array.Resize(ref depth, width * height);
                DepthBuffer = depth;
            }

            Width = width;
            Height = height;

            _topLeft = (0, Height - 1);
            _topRight = (Width - 1, Height - 1);
            _bottomLeft = (0, 0);
            _bottomRight = (Width - 1, 0);
        }

        public void Clear(Pixel color, double depth)
        {
            int length = Width * Height;
            for (int i = 0; i < length; ++i)
                ColorBuffer[i] = color;

            for (int i = 0; i < length; ++i)
                DepthBuffer[i] = depth;
        }

        public Point ScreenToIndex(in Vector2 screen)
        {
            return (
                (int)((screen.X + 1) * 0.5 * Width),
                (int)((1 - screen.Y) * 0.5 * Height)
            );
        }

        public void DrawLine(in Vector3 p0, in Vector3 p1, Pixel pixel)
        {
            var (x0, y0) = ScreenToIndex((p0.X, p0.Y));
            var (x1, y1) = ScreenToIndex((p1.X, p1.Y));
            DrawLine(x0, y0, x1, y1, pixel);
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Pixel pixel)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = (dx > dy ? dx : -dy) / 2;

            for (; ; )
            {
                if (y0 >= 0 && y0 < Height && x0 >= 0 && x0 < Width)
                    ColorBuffer[y0 * Width + x0] = pixel;

                if (x0 == x1 && y0 == y1)
                    break;

                var e2 = err;

                if (e2 > -dx)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dy)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }


        static int GetBorder(Point t0, Point t, int y)
        {
            int dy = t.Y - t0.Y;
            int dx = t.X - t0.X;
            return (dx * (y - t0.Y) + dy * t0.X) / dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void HorizontalLine(Point t0, Point t1, Point t2, double z0, double z1, double z2, int left, int right, int y, ColorFactory factory)
        {
            if (left > right)
                (left, right) = (right, left);

            left = Math.Max(0, left);
            right = Math.Min(Width - 1, right);

            int dy12 = t1.Y - t2.Y;
            int dy20 = t2.Y - t0.Y;
            int dx21 = t2.X - t1.X;
            int dx02 = t0.X - t2.X;
            int dy2 = y - t2.Y;
            double det = dy12 * dx02 - dx21 * dy20;

            int bufferIdx = left + y * Width;
            for (int x = left; x <= right; ++x, ++bufferIdx)
            {
                int dx2 = (x - t2.X);
                var alpha0 = (dy12 * dx2 + dx21 * dy2) / det;
                var alpha1 = (dy20 * dx2 + dx02 * dy2) / det;
                var alpha2 = 1 - alpha0 - alpha1;

                var depth = alpha0 * z0 + alpha1 * z1 + alpha2 * z2;

                //if (depth < 0 || depth > 1 || depth >= DepthBuffer[bufferIdx])
                if (depth >= DepthBuffer[bufferIdx])
                    continue;

                DepthBuffer[bufferIdx] = depth;
                ColorBuffer[bufferIdx] = factory(new Vector3(alpha0, alpha1, alpha2));
            }
        }

        public delegate Pixel ColorFactory(in Vector3 barycentric);

        public Vector2 ToScreen(Vector4 @virtual)
        {
            return (
                (@virtual.X / @virtual.W + 1) * 0.5 * Width,
                (1 - @virtual.Y / @virtual.W) * 0.5 * Height
            );
        }

        public void DrawTriangle(in Vector4 p0, in Vector4 p1, in Vector4 p2, ColorFactory factory)
        {
            Vector2 t0 = ToScreen(p0);
            Vector2 t1 = ToScreen(p1);
            Vector2 t2 = ToScreen(p2);

            Point i0 = ((int)(t0.X + 0.5), (int)(t0.Y + 0.5));
            Point i1 = ((int)(t1.X + 0.5), (int)(t1.Y + 0.5));
            Point i2 = ((int)(t2.X + 0.5), (int)(t2.Y + 0.5));

            // Check if clockwise. Notice that we check for counter-clockwise due to reflection of y-axis
            if (Triangle.IsCounterClockwise(i0, i1, i2))
                return;

            // Check if outside of a viewport
            if (!Triangle.Intersects(i2, i1, i0, _bottomLeft, _topRight, _topLeft) &&
                !Triangle.Intersects(i2, i1, i0, _bottomLeft, _bottomRight, _topRight))
                return;

            // Found bounding box
            Point boxMin = (
                Math.Max(0, Math.Min(i0.X, Math.Min(i1.X, i2.X))),
                Math.Max(0, Math.Min(i0.Y, Math.Min(i1.Y, i2.Y)))
            );
            Point boxMax = (
                Math.Min(Width, Math.Max(i0.X, Math.Max(i1.X, i2.X))),
                Math.Min(Height, Math.Max(i0.Y, Math.Max(i1.Y, i2.Y)))
            );

            // Plot points inside triangle using barycentric coordinates
            int dy23 = i1.Y - i2.Y;
            int dy13 = i0.Y - i2.Y;
            int dx32 = i2.X - i1.X;
            int dx13 = i0.X - i2.X;
            int det = dy23 * dx13 + dx32 * dy13;
            int detSign = Math.Sign(det);

            for (int y = boxMin.Y, scanBase = boxMin.Y * Width; y < boxMax.Y; ++y, scanBase += Width)
            {
                int dy3 = y - i2.Y;

                for (int x = boxMin.X, scan = scanBase + boxMin.X; x < boxMax.X; ++x, ++scan)
                {
                    int dx3 = x - i2.X;

                    int alphaDividend = dy23 * dx3 + dx32 * dy3;
                    if (alphaDividend * detSign < 0)
                        continue;

                    int betaDividend = dx13 * dy3 - dy13 * dx3;
                    if (betaDividend * detSign < 0)
                        continue;

                    int gammaDividend = det - alphaDividend - betaDividend;
                    if (gammaDividend * detSign < 0)
                        continue;


                    Vector3 barycentric = (
                        alphaDividend / (p0.Z * det),
                        betaDividend / (p1.Z * det),
                        gammaDividend / (p2.Z * det)
                    );
                    barycentric /= (barycentric.X + barycentric.Y + barycentric.Z);

                    double depth = Vector3.Dot(barycentric, (p0.Z, p1.Z, p2.Z));

                    if (depth > DepthBuffer[scan])
                        continue;

                    DepthBuffer[scan] = depth;

                    Pixel color = new Pixel(
                        (byte)(byte.MaxValue * barycentric.X),
                        (byte)(byte.MaxValue * barycentric.Y),
                        (byte)(byte.MaxValue * barycentric.Z)
                    );
                    ColorBuffer[scan] = color;
                }
            }
        }



    }
}
