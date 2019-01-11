using System;

namespace SimpleRenderer.Core
{
    public class Canvas
    {
        public int Width { get; }
        public int Height { get; }

        public Pixel[] RawPixels { get; }

        public Canvas(int width, int height)
        {
            Width  = width;
            Height = height;

            RawPixels = new Pixel[Width * Height];
        }

        public void Fill(Pixel pixel)
        {
            for (int i = 0; i < RawPixels.Length; ++i)
                RawPixels[i] = pixel;
        }
    }
}
