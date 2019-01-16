using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public delegate (T Vertex, Vector4 Position) VertexShader<T>(in Face face) where T : struct;
    public delegate Pixel PixelShader<T>(in T input) where T : struct;

    public static class FillingRenderer
    {
        public static void Render<T>(Canvas canvas, IReadOnlyList<Face> faces, VertexShader<T> vertexShader, PixelShader<T> pixelShader)
            where T : unmanaged
        {
            Parallel.For(0, faces.Count / 3, i =>
            {
                i *= 3;

                var (vertex0, p0) = vertexShader(faces[i + 0]);
                var (vertex1, p1) = vertexShader(faces[i + 1]);
                var (vertex2, p2) = vertexShader(faces[i + 2]);

                canvas.DrawTriangle(
                    vertex0, p0,
                    vertex1, p1,
                    vertex2, p2,
                    pixelShader,
                    InterpolatorCache<T>.Instance
                );
            });

            //for (int i = 0; i < faces.Count; i += 3)
            //{
            //    var (vertex0, p0) = vertexShader(faces[i + 0]);
            //    var (vertex1, p1) = vertexShader(faces[i + 1]);
            //    var (vertex2, p2) = vertexShader(faces[i + 2]);

            //    canvas.DrawTriangle(
            //        vertex0, p0,
            //        vertex1, p1,
            //        vertex2, p2,
            //        pixelShader,
            //        InterpolatorCache<T>.Instance
            //    );
            //}
        }
    }
}
