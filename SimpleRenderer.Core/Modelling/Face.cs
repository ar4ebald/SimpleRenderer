namespace SimpleRenderer.Core.Modelling
{
    public struct Face
    {
        public readonly int Vertex;
        public readonly int Texture;
        public readonly int Normal;

        public Face(int vertex, int texture, int normal)
        {
            Vertex = vertex;
            Texture = texture;
            Normal = normal;
        }

        public override string ToString() => $"(Vertex:{Vertex}, Texture:{Texture}, Normal:{Normal})";
    }
}
