using System;

namespace SimpleRenderer.Mathematics
{
    public struct Vector3 : IEquatable<Vector3>
    {
        public double X, Y, Z;

        public Vector3(double value)
        {
            X = Y = Z = value;
        }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Vector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"({X:N4}, {Y:N4}, {Z:N4})";

        public static implicit operator Vector3((double X, double Y, double Z) tuple)
            => new Vector3(tuple.X, tuple.Y, tuple.Z);

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);
        public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);

        public static Vector3 operator +(Vector3 left, Vector3 right)
            => new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static Vector3 operator -(Vector3 left, Vector3 right)
            => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        public static Vector3 operator *(Vector3 vector, double factor)
            => new Vector3(vector.X * factor, vector.Y * factor, vector.Z * factor);
        public static Vector3 operator /(Vector3 dividend, double divisor)
            => new Vector3(dividend.X / divisor, dividend.Y / divisor, dividend.Z / divisor);


        public static double Dot(in Vector3 left, in Vector3 right)
            => left.X * right.X + left.Y * right.Y + left.Z * right.Z;

        public static Vector3 Cross(in Vector3 left, in Vector3 right)
        {
            return (
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X
            );
        }

        public double Length => Math.Sqrt(Dot(this, this));

        public Vector3 Normalized => this / Length;
    }
}
