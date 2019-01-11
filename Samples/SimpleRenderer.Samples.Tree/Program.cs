using System;
using System.Drawing;
using System.Windows.Forms;
using SimpleRenderer.Windows;

namespace SimpleRenderer.Samples.Tree
{
    class Program : HostForm
    {
        static void Main()
        {
            Application.Run(new Program());
        }

        protected override void Update()
        {
            
        }

        protected override void Render()
        {
            using (var g = CreateGraphics())
            {
                
                g.DrawLine(Pens.Blue, 0, 0, g.VisibleClipBounds.Width, g.VisibleClipBounds.Height);
            }
        }
    }
}
