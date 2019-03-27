using SimpleRenderer.Core.Modelling;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core.Rendering
{
    public static class WireframeRenderer
    {
        static readonly int[] _indicesOffsets =
        {
            0, 1,
            1, 2,
            2, 0
        };

        //public static void Render(Canvas canvas, Model model, Matrix worldViewProjection, Pixel color)
        //{
        //    foreach (var triangle in model.Triangles)
        //    {
        //        ref var f0 = ref model.Faces[triangle.FaceIndex0];
        //    }

        //    for (int i = 0; i < model.Faces.Count; i += 3)
        //    {
        //        for (int j = 0; j < _indicesOffsets.Length; j += 2)
        //        {

        //            var vertex0 = model.Vertices[model.Faces[i + _indicesOffsets[j + 0]].Vertex];
        //            var vertex1 = model.Vertices[model.Faces[i + _indicesOffsets[j + 1]].Vertex];

        //            Vector4 homo0 = (vertex0.X, vertex0.Y, vertex0.Z, 1);
        //            Vector4 homo1 = (vertex1.X, vertex1.Y, vertex1.Z, 1);

        //            homo0 = worldViewProjection * homo0;
        //            homo1 = worldViewProjection * homo1;

        //            canvas.DrawLine(
        //                new Vector3(homo0.X / homo0.W, homo0.Y / homo0.W, homo0.Z / homo0.W),
        //                new Vector3(homo1.X / homo1.W, homo1.Y / homo1.W, homo1.Z / homo1.W),
        //                color
        //            );
        //        }
        //    }
        //}

        public static void Render(Canvas canvas, Model model, Matrix worldViewProjection, Pixel color)
        {
            foreach (var triangle in model.Triangles)
            {
                var v0 = model.Vertices[model.Faces[triangle.FaceIndex0].Vertex];
                var v1 = model.Vertices[model.Faces[triangle.FaceIndex1].Vertex];
                var v2 = model.Vertices[model.Faces[triangle.FaceIndex2].Vertex];

                var h0 = worldViewProjection * new Vector4(v0.X, v0.Y, v0.Z, 1);
                var h1 = worldViewProjection * new Vector4(v1.X, v1.Y, v1.Z, 1);
                var h2 = worldViewProjection * new Vector4(v2.X, v2.Y, v2.Z, 1);

                v0 = new Vector3(h0.X / h0.W, h0.Y / h0.W, h0.Z / h0.W);
                v1 = new Vector3(h1.X / h1.W, h1.Y / h1.W, h1.Z / h1.W);
                v2 = new Vector3(h2.X / h2.W, h2.Y / h2.W, h2.Z / h2.W);

                canvas.DrawLine(v0, v1, color);
                canvas.DrawLine(v1, v2, color);
                canvas.DrawLine(v2, v0, color);
            }
        }
    }
}
