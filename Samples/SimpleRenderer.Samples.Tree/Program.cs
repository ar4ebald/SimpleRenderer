using System;
using System.Drawing;
using System.Windows.Forms;
using SimpleRenderer.Core;
using SimpleRenderer.Windows;

namespace SimpleRenderer.Samples.Tree
{
    class Program : HostForm
    {
        static void Main()
        {
            Application.Run(new Program());
        }

        public Program()
        {
            BackgroundColor = Pixel.Red;
        }

        protected override void Update()
        {

        }

        protected override void Render(Canvas canvas)
        {

        }
    }
}
