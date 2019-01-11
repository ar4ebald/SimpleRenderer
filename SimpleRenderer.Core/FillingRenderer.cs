using System;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public static class FillingRenderer
    {
        public static void Render(Canvas canvas, Model model, Matrix worldViewProjection, Pixel color)
        {
            for (int i = 0; i < model.Indices.Count; i += 3)
            {
                Vector4 homo0 = worldViewProjection * (model.Vertices[model.Indices[i + 0]], 1);
                Vector4 homo1 = worldViewProjection * (model.Vertices[model.Indices[i + 1]], 1);
                Vector4 homo2 = worldViewProjection * (model.Vertices[model.Indices[i + 2]], 1);

                canvas.DrawTriangle(
                    (homo0.X / homo0.W, homo0.Y / homo0.W),
                    (homo1.X / homo1.W, homo1.Y / homo1.W),
                    (homo2.X / homo2.W, homo2.Y / homo2.W),
                    color
                );
            }
        }
    }
}
