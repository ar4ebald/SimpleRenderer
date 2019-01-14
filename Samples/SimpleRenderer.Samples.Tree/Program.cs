using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SimpleRenderer.Core;
using SimpleRenderer.Mathematics;
using SimpleRenderer.Windows;

namespace SimpleRenderer.Samples.Tree
{
    class Program : HostForm
    {
        static void Main()
        {
            Application.Run(new Program());
        }

        readonly Model _model;
        readonly Matrix _projection;
        readonly Vector3 _lightDirection;

        public Program()
        {
            Size = SizeFromClientSize(new Size(600, 600));

            using (var reader = new StreamReader("Lowpoly_tree_sample.obj"))
                _model = Model.ReadWavefrontObj(reader);

            _projection = Matrix.Perspective(Math.PI / 180 * 60, 1, 0.1, 1000);

            _lightDirection = new Vector3(1, 1, 1).Normalized;
        }

        protected override void Update()
        {

        }

        protected override void Render(Canvas canvas)
        {
            var t = DateTime.Now.TimeOfDay.TotalSeconds;
            var world = Matrix.Scale(new Vector3(0.025)) * Matrix.Rotation((0, 1, 0), -t) * Matrix.Translation((0, -0.4, 1));

            var worldViewProjection = world * _projection;

            FillingRenderer.Render(canvas, _model, worldViewProjection, Shader);
        }

        Pixel Shader(int idx0, int idx1, int idx2, in Vector3 barycentric)
        {
            //var color = new Pixel(
            //    (byte)(byte.MaxValue * barycentric.X),
            //    (byte)(byte.MaxValue * barycentric.Y),
            //    (byte)(byte.MaxValue * barycentric.Z)
            //);

            var normal = _model.Normals[_model.Indices[idx0].Normal];
            var shade = Vector3.Dot(normal, _lightDirection);
            shade = Math.Min(1, Math.Max(0, shade * 0.6 + 0.4));
            byte intensity = (byte)(byte.MaxValue * shade);
            var color = new Pixel(intensity, intensity, intensity);

            //var depth = Vector3.Dot(
            //    (_model.Vertices[idx0].Z, _model.Vertices[idx1].Z, _model.Vertices[idx2].Z),
            //    barycentric
            //);

            return color;
        }
    }
}
