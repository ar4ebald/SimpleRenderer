namespace SimpleRenderer.Core
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

        public static implicit operator Face((int Vertex, int Texture, int Normal) tuple)
            => new Face(tuple.Vertex, tuple.Texture, tuple.Normal);
    }
}
