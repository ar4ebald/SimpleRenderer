using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SimpleRenderer.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = sizeof(byte))]
    public struct Pixel
    {
        public byte R;
        public byte G;
        public byte B;

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte GetByte(int integer, int index) 
            => (byte)((integer >> (8 * index)) & 0xFF);

        public static implicit operator Pixel((byte r, byte g, byte b) tuple)
            => new Pixel(tuple.r, tuple.g, tuple.b);

        public static implicit operator Pixel(int rgb)
            => new Pixel(GetByte(rgb, 2), GetByte(rgb, 1), GetByte(rgb, 0));

        public static readonly Pixel Black = 0x000000;

        //public static readonly Pixel CornflowerBlue
    }
}
