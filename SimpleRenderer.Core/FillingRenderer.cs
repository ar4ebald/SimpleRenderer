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

    public enum CullingMode
    {
        None,
        CounterClockwise,
        Clockwise
    }

    public static class FillingRenderer
    {
        public static void Render<T>(Canvas canvas, IReadOnlyList<Face> faces, VertexShader<T> vertexShader, PixelShader<T> pixelShader, CullingMode culling)
            where T : unmanaged
        {
            int width = canvas.Width;
            int height = canvas.Height;

            Rectangle viewport = new Rectangle(0, 0, height, width);

            var colorBuffer = canvas.ColorBuffer;
            var depthBuffer = canvas.DepthBuffer;

            var interpolator = InterpolatorCache<T>.Instance;

            Parallel.For(0, faces.Count / 3, i =>
            {
                i *= 3;

                var (vertex0, p0) = vertexShader(faces[i + 0]);
                var (vertex1, p1) = vertexShader(faces[i + 1]);
                var (vertex2, p2) = vertexShader(faces[i + 2]);

                Vector2 t0 = canvas.ToScreen(p0);
                Vector2 t1 = canvas.ToScreen(p1);
                Vector2 t2 = canvas.ToScreen(p2);

                Point i0 = ((int)(t0.X + 0.5), (int)(t0.Y + 0.5));
                Point i1 = ((int)(t1.X + 0.5), (int)(t1.Y + 0.5));
                Point i2 = ((int)(t2.X + 0.5), (int)(t2.Y + 0.5));

                // Check if clockwise. Notice that we check for counter-clockwise due to reflection of y-axis
                switch (culling)
                {
                    case CullingMode.Clockwise:
                    {
                        if (Triangle.IsCounterClockwise(i0, i1, i2)) return;
                        break;
                    }
                    case CullingMode.CounterClockwise:
                    {
                        if (!Triangle.IsCounterClockwise(i0, i1, i2)) return;
                        break;
                    }
                }

                // Check if outside of a viewport
                if (!viewport.Contains(i0) &&
                    !viewport.Contains(i1) &&
                    !viewport.Contains(i2) &&
                    !Triangle.Intersects(i2, i1, i0, viewport.BottomLeft, viewport.TopRight, viewport.TopLeft) &&
                    !Triangle.Intersects(i2, i1, i0, viewport.BottomLeft, viewport.BottomRight, viewport.TopRight))
                {
                    return;
                }

                // Found bounding box
                Point boxMin = (
                    Math.Max(viewport.Left, Math.Min(i0.X, Math.Min(i1.X, i2.X))),
                    Math.Max(viewport.Top, Math.Min(i0.Y, Math.Min(i1.Y, i2.Y)))
                );
                Point boxMax = (
                    Math.Min(viewport.Right, Math.Max(i0.X, Math.Max(i1.X, i2.X))),
                    Math.Min(viewport.Bottom, Math.Max(i0.Y, Math.Max(i1.Y, i2.Y)))
                );

                // Plot points inside triangle using barycentric coordinates
                int dy23 = i1.Y - i2.Y;
                int dy13 = i0.Y - i2.Y;
                int dx32 = i2.X - i1.X;
                int dx13 = i0.X - i2.X;
                int det = dy23 * dx13 + dx32 * dy13;
                int detSign = Math.Sign(det);

                for (int y = boxMin.Y, scanBase = boxMin.Y * width; y < boxMax.Y; ++y, scanBase += width)
                {
                    int dy3 = y - i2.Y;

                    for (int x = boxMin.X, scan = scanBase + boxMin.X; x < boxMax.X; ++x, ++scan)
                    {
                        int dx3 = x - i2.X;

                        int alphaDividend = dy23 * dx3 + dx32 * dy3;
                        if (alphaDividend * detSign < 0)
                            continue;

                        int betaDividend = dx13 * dy3 - dy13 * dx3;
                        if (betaDividend * detSign < 0)
                            continue;

                        int gammaDividend = det - alphaDividend - betaDividend;
                        if (gammaDividend * detSign < 0)
                            continue;


                        Vector3 barycentric = (
                            alphaDividend / (p0.Z * det),
                            betaDividend / (p1.Z * det),
                            gammaDividend / (p2.Z * det)
                        );
                        barycentric /= (barycentric.X + barycentric.Y + barycentric.Z);

                        double depth = Vector3.Dot(barycentric, (p0.Z, p1.Z, p2.Z));

                        if (depth > depthBuffer[scan])
                            continue;

                        depthBuffer[scan] = depth;

                        var color = pixelShader(interpolator(vertex0, vertex1, vertex2, barycentric));

                        if (depth > depthBuffer[scan])
                            continue;

                        colorBuffer[scan] = color;
                    }
                }
            });
        }
    }
}
