using System.Runtime.CompilerServices;

namespace SimpleRenderer.Mathematics
{
    public static class TriangleCollisions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCounterClockwise(in Point p1, in Point p2, in Point p3)
            => Point.Cross(p2 - p1, p3 - p1) <= 0;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(in Point u1, in Point u2, in Point u3, in Point v1, in Point v2, in Point v3)
        {
            Point vec;

            vec = u2 - u1;
            if (Point.Cross(vec, v1 - u1) < 0 &&
                Point.Cross(vec, v2 - u1) < 0 &&
                Point.Cross(vec, v3 - u1) < 0)
                return false;

            vec = u3 - u2;
            if (Point.Cross(vec, v1 - u2) < 0 &&
                Point.Cross(vec, v2 - u2) < 0 &&
                Point.Cross(vec, v3 - u2) < 0)
                return false;

            vec = u1 - u3;
            if (Point.Cross(vec, v1 - u3) < 0 &&
                Point.Cross(vec, v2 - u3) < 0 &&
                Point.Cross(vec, v3 - u3) < 0)
                return false;

            vec = v2 - v1;
            if (Point.Cross(vec, u1 - v1) < 0 &&
                Point.Cross(vec, u2 - v1) < 0 &&
                Point.Cross(vec, u3 - v1) < 0)
                return false;

            vec = v3 - v2;
            if (Point.Cross(vec, u1 - v2) < 0 &&
                Point.Cross(vec, u2 - v2) < 0 &&
                Point.Cross(vec, u3 - v2) < 0)
                return false;

            vec = v1 - v3;
            if (Point.Cross(vec, u1 - v3) < 0 &&
                Point.Cross(vec, u2 - v3) < 0 &&
                Point.Cross(vec, u3 - v3) < 0)
                return false;

            return true;
        }
    }
}
