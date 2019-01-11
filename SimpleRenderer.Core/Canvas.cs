using System;

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
    }
}
