using System.Runtime.InteropServices;

namespace SimpleRenderer.Core
{
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(byte))]
    public struct PixelAndDepth
    {
        [FieldOffset(0)] public byte Depth;
        [FieldOffset(1)] public byte B;
        [FieldOffset(2)] public byte G;
        [FieldOffset(3)] public byte R;

        [FieldOffset(0)] public int RGBDepth;
    }
}