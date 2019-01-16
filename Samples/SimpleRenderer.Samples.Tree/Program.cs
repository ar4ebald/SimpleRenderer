using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SimpleRenderer.Core;
using SimpleRenderer.Mathematics;
using SimpleRenderer.Windows;

namespace SimpleRenderer.Samples.Tree
{
    public struct Vertex
    {
        //public double Value;
        public Vector3 Normal;
    }

    class Program : HostForm
    {
        //public static Vertex Keken(in Vertex v0, in Vertex v1, in Vertex v2, in Vector3 barycentric)
        //{
        //    var v = v0;
        //    v.Value = (v0.Value * barycentric.X + v1.Value * barycentric.Y + v2.Value * barycentric.Z);
        //    return v;
        //}

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

            _lightDirection = new Vector3(1, 1, -1).Normalized;
        }

        protected override void Update()
        {

        }

        protected override void Render(Canvas canvas)
        {
            var t = DateTime.Now.TimeOfDay.TotalSeconds;
            var world = Matrix.Scale(new Vector3(0.025)) * Matrix.Rotation((0, 1, 0), -t) * Matrix.Translation((0, -0.4, 1));

            var worldViewProjection = world * _projection;

            (Vertex Vertex, Vector4 Position) VertexShader(in Face face)
            {
                var position = _model.Vertices[face.Vertex];
                var normal = _model.Normals[face.Normal];

                var projection = worldViewProjection * (position, 1);
                normal = (world * (normal, 0)).XYZ.Normalized;

                var vertex = new Vertex { Normal = normal };

                return (vertex, projection);
            }

            Pixel PixelShader(in Vertex input)
            {
                var shade = Vector3.Dot(input.Normal, _lightDirection);
                shade = Math.Min(1, Math.Max(0, shade * 0.6 + 0.4));
                byte intensity = (byte)(byte.MaxValue * shade);
                var color = new Pixel(intensity, intensity, intensity);
                return color;
            }

            FillingRenderer.Render(canvas, _model.Faces, VertexShader, PixelShader);
        }

        Pixel Shader(int idx0, int idx1, int idx2, in Vector3 barycentric)
        {
            //var color = new Pixel(
            //    (byte)(byte.MaxValue * barycentric.X),
            //    (byte)(byte.MaxValue * barycentric.Y),
            //    (byte)(byte.MaxValue * barycentric.Z)
            //);

            var normal = _model.Normals[_model.Faces[idx0].Normal];
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
