using System;
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

        ref Pixel this[int x, int y] => ref RawPixels[y * Width + x];

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

        (int X, int Y) ScreenToIndex(Vector2 screen)
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
    }
}
