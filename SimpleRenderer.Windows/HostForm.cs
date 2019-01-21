using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SimpleRenderer.Core;
using Point = System.Drawing.Point;

namespace SimpleRenderer.Windows
{
    public abstract class HostForm : Form
    {
        readonly Canvas _canvas;
        readonly Stopwatch _timer;

        readonly IDictionary<Keys, bool> _isKeyPressed;

        protected Pixel BackgroundColor = Pixel.CornflowerBlue;

        protected HostForm()
        {
            Application.Idle += HandleApplicationIdle;

            _canvas = new Canvas();
            _timer = Stopwatch.StartNew();

            KeyPreview = true;

            _isKeyPressed = new Dictionary<Keys, bool>();
        }

        protected bool IsKeyDown(Keys key) => _isKeyPressed.TryGetValue(key, out var isDown) && isDown;

        static bool IsApplicationIdle()
        {
            return PeekMessage(out _, IntPtr.Zero, 0, 0, 0) == 0;
        }

        void HandleApplicationIdle(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                var dt = _timer.Elapsed.TotalSeconds;
                _timer.Restart();

                Update(dt);
                Render(dt);
            }
        }

        protected abstract void Update(double dt);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _isKeyPressed[e.KeyCode] = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _isKeyPressed[e.KeyCode] = false;
        }

        unsafe void Display(Canvas canvas, Graphics graphics)
        {
            Bitmap bitmap;
            fixed (Pixel* ptr = &canvas.ColorBuffer[0])
            {
                bitmap = new Bitmap(
                    canvas.Width, canvas.Height,
                    Pixel.Size * canvas.Width,
                    PixelFormat.Format24bppRgb,
                    new IntPtr(ptr)
                );
            }

            using (bitmap)
                graphics.DrawImageUnscaled(bitmap, Point.Empty);
        }

        void Render(double dt)
        {
            using (var g = CreateGraphics())
            {
                var width = (int)g.VisibleClipBounds.Width;
                var height = (int)g.VisibleClipBounds.Height;

                _canvas.EnsureSize(width, height);
                _canvas.Clear(BackgroundColor, double.MaxValue);

                Render(_canvas, dt);
                Display(_canvas, g);
            }
        }

        protected abstract void Render(Canvas canvas, double dt);

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
