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

        public Program()
        {
            using (var reader = new StreamReader("Lowpoly_tree_sample.obj"))
                _model = Model.ReadWavefrontObj(reader);

            _projection = Matrix.Perspective(Math.PI / 180 * 60, 1, 0.1, 1000);
        }

        protected override void Update()
        {

        }

        protected override void Render(Canvas canvas)
        {
            var t = DateTime.Now.TimeOfDay.TotalSeconds;
            var world = Matrix.Scale(new Vector3(0.025)) * Matrix.Rotation((0, 1, 0), -t) * Matrix.Translation((0, -0.4, 1));

            var worldViewProjection = world * _projection;

            FillingRenderer.Render(canvas, _model, worldViewProjection, Pixel.Red);
            //WireframeRenderer.Render(canvas, _model, worldViewProjection, Pixel.Red);
        }
    }
}
