using System;

namespace SimpleRenderer.Mathematics
{
    public struct Vector4 : IEquatable<Vector4>
    {
        public double X, Y, Z, W;

        public Vector4(double value)
        {
            X = Y = Z = W = value;
        }

        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector2 XY => (X, Y);
        public Vector3 XYZ => (X, Y, Z);

        public bool Equals(Vector4 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"({X:N4}, {Y:N4}, {Z:N4}, {W:N4})";

        public static implicit operator Vector4((double X, double Y, double Z, double W) tuple)
            => new Vector4(tuple.X, tuple.Y, tuple.Z, tuple.W);

        public static implicit operator Vector4((Vector3 XYZ, double W) tuple)
            => new Vector4(tuple.XYZ.X, tuple.XYZ.Y, tuple.XYZ.Z, tuple.W);

        public static bool operator ==(Vector4 left, Vector4 right) => left.Equals(right);
        public static bool operator !=(Vector4 left, Vector4 right) => !(left == right);

        public static Vector4 operator +(Vector4 left, Vector4 right)
            => new Vector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        public static Vector4 operator -(Vector4 left, Vector4 right)
            => new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

        public static Vector4 operator *(Vector4 vector, double factor)
            => new Vector4(vector.X * factor, vector.Y * factor, vector.Z * factor, vector.W * factor);
        public static Vector4 operator /(Vector4 dividend, double divisor)
            => new Vector4(dividend.X / divisor, dividend.Y / divisor, dividend.Z / divisor, dividend.W / divisor);

        public static double Dot(Vector4 left, Vector4 right)
            => left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;

        public double Length => Math.Sqrt(Dot(this, this));

        public Vector4 Normalized => this / Length;
    }
}
