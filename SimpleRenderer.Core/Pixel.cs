using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SimpleRenderer.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = sizeof(byte))]
    public struct Pixel
    {
        public const int Size = 3 * sizeof(byte);

        public byte B;
        public byte G;
        public byte R;

        public Pixel(byte r, byte g, byte b)
        {
            B = b;
            G = g;
            R = r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte GetByte(int triplet, int index) 
            => (byte)((triplet >> (8 * index)) & 0xFF);

        public static implicit operator Pixel((byte r, byte g, byte b) tuple)
            => new Pixel(tuple.r, tuple.g, tuple.b);

        public static implicit operator Pixel(int triplet)
            => new Pixel(GetByte(triplet, 2), GetByte(triplet, 1), GetByte(triplet, 0));


        public static readonly Pixel Black = 0x000000;
        public static readonly Pixel CornflowerBlue = 0x6495ED;
        public static readonly Pixel Red = 0xFF0000;
    }
}
