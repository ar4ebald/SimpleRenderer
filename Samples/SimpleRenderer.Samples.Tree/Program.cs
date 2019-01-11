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
        readonly Pixel _wireframeColor;

        public Program()
        {
            //_model = Model.CreatePlane(0.5);
            using (var reader = new StreamReader("Lowpoly_tree_sample.obj"))
                _model = Model.ReadWavefrontObj(reader);
            _wireframeColor = Pixel.Red;
        }

        protected override void Update()
        {

        }

        protected override void Render(Canvas canvas)
        {
            for (int i = 0; i < _model.Indices.Count; i += 3)
            {
                canvas.DrawLine(
                    (Vector2)_model.Vertices[_model.Indices[i + 0]],
                    (Vector2)_model.Vertices[_model.Indices[i + 1]],
                    _wireframeColor
                );

                canvas.DrawLine(
                    (Vector2)_model.Vertices[_model.Indices[i + 1]],
                    (Vector2)_model.Vertices[_model.Indices[i + 2]],
                    _wireframeColor
                );

                canvas.DrawLine(
                    (Vector2)_model.Vertices[_model.Indices[i + 2]],
                    (Vector2)_model.Vertices[_model.Indices[i + 0]],
                    _wireframeColor
                );
            }
        }
    }
}
