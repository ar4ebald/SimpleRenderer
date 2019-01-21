using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core.World
{
    public sealed class Camera
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public Matrix ViewMatrix => Matrix.Translation(-Position) * Matrix.Rotation(Rotation.Inverted());

        public Camera()
        {
            Rotation = Quaternion.Identity;
        }
    }
}
