using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using SimpleRenderer.Mathematics;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace SimpleRenderer.Core.Modelling
{
    public sealed class Texture
    {
        public readonly int Width;
        public readonly int Height;

        public readonly Vector3[] ColorMap;

        Texture(int width, int height, Vector3[] colorMap)
        {
            if (width * height != colorMap.Length)
                throw new ArgumentException("Invalid texture size");

            Width = width;
            Height = height;

            ColorMap = colorMap;
        }


        public ref Vector3 this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                x %= Width;
                if (x < 0) x += Width;
                y %= Height;
                if (y < 0) y += Height;

                return ref ColorMap[y * Width + x];
            }
        }

        public ref Vector3 this[in Vector2 position]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int uvY = (int)((1 - position.Y) * Height);
                int uvX = (int)(position.X * Width);

                return ref this[uvX, uvY];
            }
        }

        public static Texture ReadFrom(string path)
        {
            using (var reader = File.OpenRead(path))
                return ReadFrom(reader);
        }

        public static unsafe Texture ReadFrom(Stream stream)
        {
            using (var bitmap = new Bitmap(stream))
            {
                if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                    throw new ArgumentException($"Texture format is expected to be {nameof(PixelFormat.Format24bppRgb)}");

                int width = bitmap.Width;
                int height = bitmap.Height;

                var colorMap = new Vector3[width * height];
                var rect = new Rectangle(Point.Empty, new Size(width, height));

                var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                try
                {
                    Pixel* start = (Pixel*)data.Scan0.ToPointer();
                    for (int i = 0; i < colorMap.Length; ++i)
                    {
                        colorMap[i] = new Vector3(
                            start[i].R / (double)byte.MaxValue,
                            start[i].G / (double)byte.MaxValue,
                            start[i].B / (double)byte.MaxValue
                        );
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                return new Texture(width, height, colorMap);
            }
        }
    }
}
