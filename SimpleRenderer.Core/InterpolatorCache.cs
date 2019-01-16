using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public delegate T Interpolator<T>(in T v0, in T v1, in T v2, in Vector3 barycentric) where T : struct;

    static class InterpolatorCache<T> where T : unmanaged
    {
        public static Interpolator<T> Instance { get; } = InterpolatorFactory.CreateDelegate<T>();
    }
}
