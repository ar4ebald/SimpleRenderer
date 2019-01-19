using System.Runtime.CompilerServices;

namespace SimpleRenderer.Mathematics
{
    public struct Rectangle
    {
        public readonly int Top;
        public readonly int Left;

        public readonly int Height;
        public readonly int Width;

        public readonly int Bottom;
        public readonly int Right;

        public readonly Point TopLeft;
        public readonly Point TopRight;
        public readonly Point BottomLeft;
        public readonly Point BottomRight;

        public Rectangle(int top, int left, int height, int width)
        {
            Top  = top;
            Left = left;

            Height = height;
            Width  = width;

            Bottom = top + height;
            Right  = left + width;

            TopLeft     = (Top, Left);
            TopRight    = (Top, Right);
            BottomLeft  = (Bottom, Left);
            BottomRight = (Bottom, Right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Point point)
        {
            return point.X >= Left && point.X < Right && point.Y >= Top && point.Y < Bottom;
        }
    }
}
