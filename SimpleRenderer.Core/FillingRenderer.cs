using System;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public static class FillingRenderer
    {
        public delegate Pixel PixelShader(int idx0, int idx1, int idx2, in Vector3 barycentric);

        public static void Render(Canvas canvas, Model model, Matrix worldViewProjection, PixelShader shader)
        {
            for (int i = 0; i < model.Indices.Count; i += 3)
            {
                var idx0 = model.Indices[i + 0];
                var idx1 = model.Indices[i + 1];
                var idx2 = model.Indices[i + 2];

                Vector4 homo0 = worldViewProjection * (model.Vertices[idx0], 1);
                Vector4 homo1 = worldViewProjection * (model.Vertices[idx1], 1);
                Vector4 homo2 = worldViewProjection * (model.Vertices[idx2], 1);

                canvas.DrawTriangle(
                    homo0,
                    homo1,
                    homo2,
                    (in Vector3 barycentric) => shader(idx0, idx1, idx2, barycentric)
                );
            }
        }
    }
}
