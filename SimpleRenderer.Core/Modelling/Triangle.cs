namespace SimpleRenderer.Core.Modelling
{
    public struct Triangle
    {
        public int FaceIndex0;
        public int FaceIndex1;
        public int FaceIndex2;
        public Material Material;

        public Triangle(int faceIndex0, int faceIndex1, int faceIndex2, Material material)
        {
            FaceIndex0 = faceIndex0;
            FaceIndex1 = faceIndex1;
            FaceIndex2 = faceIndex2;
            Material = material;
        }
    }
}