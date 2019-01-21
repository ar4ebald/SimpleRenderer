using System;

namespace SimpleRenderer.Mathematics
{
    public struct Vector2 : IEquatable<Vector2>
    {
        public static Vector2 Zero = (0, 0);
        public static Vector2 Unit = (1, 1);
        public static Vector2 UnitX = (1, 0);
        public static Vector2 UnitY = (0, 1);


        public double X, Y;

        public Vector2(double value)
        {
            X = Y = value;
        }

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString() => $"({X:N4}, {Y:N4})";

        public static implicit operator Vector2(in (double X, double Y) tuple)
            => new Vector2(tuple.X, tuple.Y);

        public static explicit operator Vector2(in Vector3 vector)
            => new Vector2(vector.X, vector.Y);

        public static bool operator ==(in Vector2 left, in Vector2 right) => left.Equals(right);
        public static bool operator !=(in Vector2 left, in Vector2 right) => !(left == right);

        public static Vector2 operator +(in Vector2 left, in Vector2 right)
            => new Vector2(left.X + right.X, left.Y + right.Y);
        public static Vector2 operator -(in Vector2 left, in Vector2 right)
            => new Vector2(left.X - right.X, left.Y - right.Y);

        public static Vector2 operator *(in Vector2 vector, double factor)
            => new Vector2(vector.X * factor, vector.Y * factor);
        public static Vector2 operator /(in Vector2 dividend, double divisor)
            => new Vector2(dividend.X / divisor, dividend.Y / divisor);


        public static double Dot(in Vector2 left, in Vector2 right)
            => left.X * right.X + left.Y * right.Y;

        public static double Cross(in Vector2 left, in Vector2 right)
            => left.X * right.Y - left.Y * right.X;

        public double Length => Math.Sqrt(Dot(this, this));

        public Vector2 Normalized => this / Length;
    }
}
