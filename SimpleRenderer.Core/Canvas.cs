using System;
using System.Runtime.CompilerServices;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public sealed class Canvas
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Pixel[] RawPixels { get; private set; }

        public Canvas()
        {
            RawPixels = Array.Empty<Pixel>();
        }

        public ref Pixel this[int x, int y] => ref RawPixels[y * Width + x];

        public void EnsureSize(int width, int height)
        {
            if (Width * Height >= width * height)
                return;

            var array = RawPixels;
            Array.Resize(ref array, width * height);
            RawPixels = array;

            Width = width;
            Height = height;
        }

        public void Fill(Pixel pixel)
        {
            int length = Width * Height;
            for (int i = 0; i < length; ++i)
                RawPixels[i] = pixel;
        }

        public Point ScreenToIndex(Vector2 screen)
        {
            return (
                (int)((screen.X + 1) * 0.5 * Width),
                (int)((1 - screen.Y) * 0.5 * Height)
            );
        }

        public void DrawLine(Vector2 p0, Vector2 p1, Pixel pixel)
        {
            var (x0, y0) = ScreenToIndex(p0);
            var (x1, y1) = ScreenToIndex(p1);
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
                    RawPixels[y0 * Width + x0] = pixel;

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
        void HorizontalLine(int left, int right, int y, Pixel color)
        {
            if (left > right)
                (left, right) = (right, left);

            left  = Math.Max(0, left);
            right = Math.Min(Width - 1, right);

            int scan = y * Width;
            for (int x = left; x <= right; ++x)
                RawPixels[scan + x] = color;
        }

        public void DrawTriangle(Vector2 p0, Vector2 p1, Vector2 p2, Pixel color)
        {
            var array = RawPixels;

            var t0 = ScreenToIndex(p0);
            var t1 = ScreenToIndex(p1);
            var t2 = ScreenToIndex(p2);

            if (t0.Y > t1.Y) (t0, t1) = (t1, t0);
            if (t0.Y > t2.Y) (t0, t2) = (t2, t0);
            if (t1.Y > t2.Y) (t1, t2) = (t2, t1);

            if (t0.Y > Height || t2.Y < 0)
                return;

            if (Math.Max(t0.X, Math.Max(t1.X, t2.X)) < 0)
                return;

            if (Math.Min(t0.X, Math.Min(t1.X, t2.X)) > Width)
                return;

            int yMax = Math.Min(t1.Y, Height);
            for (int y = Math.Max(0, t0.Y); y < yMax; ++y)
            {
                int left  = GetBorder(t0, t1, y);
                int right = GetBorder(t0, t2, y);

                HorizontalLine(left, right, y, color);
            }

            if (t1.Y >= Height)
                return;

            {
                int left  = t1.X;
                int right = t1.Y == t2.Y ? t2.X : GetBorder(t0, t2, t1.Y);

                HorizontalLine(left, right, t1.Y, color);
            }

            yMax = Math.Min(t2.Y, Height - 1);
            for (int y = Math.Max(0, t1.Y + 1); y <= yMax; ++y)
            {
                int left  = GetBorder(t1, t2, y);
                int right = GetBorder(t0, t2, y);

                HorizontalLine(left, right, y, color);
            }
        }
    }
}
