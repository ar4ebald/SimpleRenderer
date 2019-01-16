using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public static class WireframeRenderer
    {
        static readonly int[] _indicesOffsets =
        {
            0, 1,
            1, 2,
            2, 0
        };

        public static void Render(Canvas canvas, Model model, Matrix worldViewProjection, Pixel color)
        {
            for (int i = 0; i < model.Faces.Count; i += 3)
            {
                for (int j = 0; j < _indicesOffsets.Length; j += 2)
                {
                    var vertex0 = model.Vertices[model.Faces[i + _indicesOffsets[j + 0]].Vertex];
                    var vertex1 = model.Vertices[model.Faces[i + _indicesOffsets[j + 1]].Vertex];

                    Vector4 homo0 = (vertex0.X, vertex0.Y, vertex0.Z, 1);
                    Vector4 homo1 = (vertex1.X, vertex1.Y, vertex1.Z, 1);

                    homo0 = worldViewProjection * homo0;
                    homo1 = worldViewProjection * homo1;

                    canvas.DrawLine(
                        (homo0.X / homo0.W, homo0.Y / homo0.W, homo0.Z / homo0.W),
                        (homo1.X / homo1.W, homo1.Y / homo1.W, homo1.Z / homo1.W),
                        color
                    );
                }
            }
        }
    }
}
