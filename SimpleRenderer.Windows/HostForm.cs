using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SimpleRenderer.Core;

namespace SimpleRenderer.Windows
{
    public abstract class HostForm : Form
    {
        readonly Canvas _canvas;

        protected Pixel BackgroundColor = Pixel.CornflowerBlue;

        protected HostForm()
        {
            Application.Idle += HandleApplicationIdle;

            Size = SizeFromClientSize(new Size(800, 800));

            _canvas = new Canvas();
        }

        static bool IsApplicationIdle()
        {
            return PeekMessage(out _, IntPtr.Zero, 0, 0, 0) == 0;
        }

        void HandleApplicationIdle(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                Update();
                Render();
            }
        }

        protected new abstract void Update();

        unsafe void Display(Canvas canvas, Graphics graphics)
        {
            Bitmap bitmap;
            fixed (Pixel* ptr = &canvas.RawPixels[0])
            {
                bitmap = new Bitmap(
                    canvas.Width, canvas.Height,
                    Pixel.Size * canvas.Width,
                    PixelFormat.Format24bppRgb,
                    new IntPtr(ptr)
                );
            }

            using (bitmap)
                graphics.DrawImage(bitmap, Point.Empty);
        }

        void Render()
        {
            using (var g = CreateGraphics())
            {
                var width = (int)g.VisibleClipBounds.Width;
                var height = (int)g.VisibleClipBounds.Height;

                _canvas.EnsureSize(width, height);
                _canvas.Fill(BackgroundColor);

                Render(_canvas);
                Display(_canvas, g);
            }
        }

        protected abstract void Render(Canvas canvas);

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
    }
}
