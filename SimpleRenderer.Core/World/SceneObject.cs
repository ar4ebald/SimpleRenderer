using SimpleRenderer.Core.Modelling;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core.World
{
    public class SceneObject
    {
        public Model Model { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public SceneObject(Model model, in Vector3 position, in Vector3 scale, in Quaternion rotation)
        {
            Model = model;
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }

        public SceneObject(Model model, in Vector3 position, in Vector3 scale) 
            : this(model, position, scale, Quaternion.Identity)
        {
        }

        public SceneObject(Model model, in Vector3 position)
            : this(model, position, Vector3.Unit)
        {
        }

        public Matrix WorldMatrix => Matrix.Translation(Position) * Matrix.Scale(Scale) * Matrix.Rotation(Rotation);
    }
}
