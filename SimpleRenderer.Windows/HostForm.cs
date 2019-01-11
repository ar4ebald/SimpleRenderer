using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SimpleRenderer.Core;

namespace SimpleRenderer.Windows
{
    public abstract class HostForm : Form
    {
        protected Pixel BackgroundColor = new Pixel();

        protected HostForm()
        {
            Application.Idle += HandleApplicationIdle;
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

        void Render()
        {
            using (var g = CreateGraphics())
            {

            }
        }

        protected abstract void Render(Canvas canvas);

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint   Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint   Time;
            public Point  Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
    }
}
