using System;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace SimpleRenderer.Mathematics
{
    [StructLayout(LayoutKind.Sequential, Pack = sizeof(double))]
    public struct Matrix
    {
        public static readonly Matrix Identity = new Matrix {M11 = 1, M22 = 1, M33 = 1, M44 = 1};

        public double M11, M12, M13, M14;
        public double M21, M22, M23, M24;
        public double M31, M32, M33, M34;
        public double M41, M42, M43, M44;

        public Matrix(
            double m11, double m12, double m13, double m14,
            double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34,
            double m41, double m42, double m43, double m44
        )
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }


        public static Matrix operator *(in Matrix left, in Matrix right) =>
            new Matrix(
                (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31) + (left.M14 * right.M41),
                (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32) + (left.M14 * right.M42),
                (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33) + (left.M14 * right.M43),
                (left.M11 * right.M14) + (left.M12 * right.M24) + (left.M13 * right.M34) + (left.M14 * right.M44),
                (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31) + (left.M24 * right.M41),
                (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32) + (left.M24 * right.M42),
                (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33) + (left.M24 * right.M43),
                (left.M21 * right.M14) + (left.M22 * right.M24) + (left.M23 * right.M34) + (left.M24 * right.M44),
                (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31) + (left.M34 * right.M41),
                (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32) + (left.M34 * right.M42),
                (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33) + (left.M34 * right.M43),
                (left.M31 * right.M14) + (left.M32 * right.M24) + (left.M33 * right.M34) + (left.M34 * right.M44),
                (left.M41 * right.M11) + (left.M42 * right.M21) + (left.M43 * right.M31) + (left.M44 * right.M41),
                (left.M41 * right.M12) + (left.M42 * right.M22) + (left.M43 * right.M32) + (left.M44 * right.M42),
                (left.M41 * right.M13) + (left.M42 * right.M23) + (left.M43 * right.M33) + (left.M44 * right.M43),
                (left.M41 * right.M14) + (left.M42 * right.M24) + (left.M43 * right.M34) + (left.M44 * right.M44)
            );

        public static Vector4 operator *(in Matrix transform, in Vector4 vector)
        {
            return (
                (vector.X * transform.M11) + (vector.Y * transform.M21) + (vector.Z * transform.M31) + (vector.W * transform.M41),
                (vector.X * transform.M12) + (vector.Y * transform.M22) + (vector.Z * transform.M32) + (vector.W * transform.M42),
                (vector.X * transform.M13) + (vector.Y * transform.M23) + (vector.Z * transform.M33) + (vector.W * transform.M43),
                (vector.X * transform.M14) + (vector.Y * transform.M24) + (vector.Z * transform.M34) + (vector.W * transform.M44));
        }

        public static Matrix Rotation(in Vector3 axis, double angle)
        {
            var (x, y, z) = axis;
            var (cos, sin) = (Math.Cos(angle), Math.Sin(angle));
            double xx  = x * x;
            double yy  = y * y;
            double zz  = z * z;
            double xy  = x * y;
            double xz  = x * z;
            double yz  = y * z;

            return new Matrix
            {
                M11 = xx + (cos * (1.0f - xx)),
                M12 = (xy - (cos * xy)) + (sin * z),
                M13 = (xz - (cos * xz)) - (sin * y),
                M21 = (xy - (cos * xy)) - (sin * z),
                M22 = yy + (cos * (1.0f - yy)),
                M23 = (yz - (cos * yz)) + (sin * x),
                M31 = (xz - (cos * xz)) + (sin * y),
                M32 = (yz - (cos * yz)) - (sin * x),
                M33 = zz + (cos * (1.0f - zz)),
                M44 = 1
            };
        }

        public static Matrix Translation(Vector3 position)
        {
            return new Matrix
            {
                M11 = 1, M22 = 1, M33 = 1, M44 = 1,

                M41 = position.X,
                M42 = position.Y,
                M43 = position.Z
            };
        }

        public static Matrix Scale(Vector3 factor)
        {
            return new Matrix
            {
                M11 = factor.X,
                M22 = factor.Y,
                M33 = factor.Z,
                M44 = 1
            };
        }


        public static Matrix Perspective(double fov, double aspect, double znear, double zfar)
        {
            double yScale = 1 / Math.Tan(fov * 0.5);
            double q = zfar / (zfar - znear);

            return new Matrix
            {
                M11 = yScale / aspect,
                M22 = yScale,
                M33 = q,
                M34 = 1,
                M43 = -q * znear
            };
        }
    }
}
