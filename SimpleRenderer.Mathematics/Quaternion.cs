using System;

namespace SimpleRenderer.Mathematics
{
    public struct Quaternion
    {
        public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        public double X;
        public double Y;
        public double Z;
        public double W;

        public Quaternion(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Quaternion Inverted()
        {
            double lengthSq = 1 / (X * X + Y * Y + Z * Z + W * W);

            return new Quaternion(
                X = -X * lengthSq,
                Y = -Y * lengthSq,
                Z = -Z * lengthSq,
                W = W * lengthSq
            );
        }

        public Vector3 Rotate(in Vector3 v)
        {
            Vector3 u = new Vector3(X, Y, Z);

            var a = u * (2 * Vector3.Dot(u, v));
            var b = v * (W * W - Vector3.Dot(u, u));
            var c = Vector3.Cross(u, v) * (2 * W);

            return a + b + c;
        }

        public static Quaternion RotationAxis(in Vector3 axis, double angle)
        {
            Vector3 normalized = axis.Normalized;

            double half = angle * 0.5f;
            double sin = Math.Sin(half);
            double cos = Math.Cos(half);

            return new Quaternion(
                normalized.X * sin,
                normalized.Y * sin,
                normalized.Z * sin,
                cos
            );
        }

        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            double lx = left.X;
            double ly = left.Y;
            double lz = left.Z;
            double lw = left.W;

            double rx = right.X;
            double ry = right.Y;
            double rz = right.Z;
            double rw = right.W;

            double a = (ly * rz - lz * ry);
            double b = (lz * rx - lx * rz);
            double c = (lx * ry - ly * rx);
            double d = (lx * rx + ly * ry + lz * rz);

            return new Quaternion(
                (lx * rw + rx * lw) + a,
                (ly * rw + ry * lw) + b,
                (lz * rw + rz * lw) + c,
                lw * rw - d
            );
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
    }
}
