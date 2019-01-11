using System;

namespace SimpleRenderer.Core
{
    public struct Point : IEquatable<Point>
    {
        public int X, Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public bool Equals(Point other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => obj is Point other && Equals(other);

        public override int GetHashCode() => unchecked((X * 397) ^ Y);

        public override string ToString() => $"(X:{X}, Y:{Y})";


        public static implicit operator Point((int X, int Y) tuple)
            => new Point(tuple.X, tuple.Y);

        public static Point operator +(Point left, Point right)
            => (left.X + right.X, left.Y + right.Y);

        public static Point operator -(Point left, Point right)
            => (left.X - right.X, left.Y - right.Y);
    }
}
