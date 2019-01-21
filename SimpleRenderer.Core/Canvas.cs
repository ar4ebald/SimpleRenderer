using System;
using SimpleRenderer.Core.Rendering;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public sealed class Canvas
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Pixel[] ColorBuffer { get; private set; }
        public double[] DepthBuffer { get; private set; }

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
        }

        static unsafe void Fill<T>(T[] array, T value, int length, int size) where T : unmanaged
        {
            int filled = Math.Min(32, length);

            fixed (T* tPtr = &array[0])
            {
                for (int i = 0; i < filled; i++)
                    tPtr[i] = value;

                byte* bPtr = (byte*)tPtr;

                filled *= size;
                length *= size;

                while (filled < length)
                {
                    int toCopy = 2 * filled < length ? filled : length - filled;
                    Buffer.MemoryCopy(bPtr, &bPtr[filled], length - filled, toCopy);
                    filled += toCopy;
                }
            }
        }

        public void Clear(Pixel color, double depth)
        {
            int length = Width * Height;

            Fill(ColorBuffer, color, length, Pixel.Size);
            Fill(DepthBuffer, depth, length, sizeof(double));
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

        public Vector2 ToScreen(in Vector4 @virtual)
        {
            return (
                (@virtual.X / @virtual.W + 1) * 0.5 * Width,
                (1 - @virtual.Y / @virtual.W) * 0.5 * Height
            );
        }

        public void DrawTriangle<T>(
            in T vertex0, in Vector4 p0,
            in T vertex1, in Vector4 p1,
            in T vertex2, in Vector4 p2,
            PixelShader<T> shader, Interpolator<T> interpolator)
        where T : struct
        {
            
        }
    }
}
