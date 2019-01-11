using System;
using System.Drawing;
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

        readonly Model _plane;
        readonly Pixel _wireframeColor;

        public Program()
        {
            _plane = Model.CreatePlane(0.5);
            _wireframeColor = Pixel.Red;
        }

        protected override void Update()
        {

        }

        protected override void Render(Canvas canvas)
        {
            for (int i = 0; i < _plane.Indices.Count; i += 3)
            {
                canvas.DrawLine(
                    (Vector2)_plane.Vertices[_plane.Indices[i + 0]],
                    (Vector2)_plane.Vertices[_plane.Indices[i + 1]],
                    _wireframeColor
                );

                canvas.DrawLine(
                    (Vector2)_plane.Vertices[_plane.Indices[i + 1]],
                    (Vector2)_plane.Vertices[_plane.Indices[i + 2]],
                    _wireframeColor
                );

                canvas.DrawLine(
                    (Vector2)_plane.Vertices[_plane.Indices[i + 2]],
                    (Vector2)_plane.Vertices[_plane.Indices[i + 0]],
                    _wireframeColor
                );
            }
        }
    }
}
