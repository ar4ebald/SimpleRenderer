using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SimpleRenderer.Mathematics;

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

        public static implicit operator Pixel(int triplet)
            => new Pixel(GetByte(triplet, 2), GetByte(triplet, 1), GetByte(triplet, 0));

        public static explicit operator Pixel(in Vector3 color)
        {
            return new Pixel(
                color.X < 0 ? (byte)0 : color.X >= byte.MaxValue ? byte.MaxValue : (byte)(color.X * byte.MaxValue),
                color.Y < 0 ? (byte)0 : color.Y >= byte.MaxValue ? byte.MaxValue : (byte)(color.Y * byte.MaxValue),
                color.Z < 0 ? (byte)0 : color.Z >= byte.MaxValue ? byte.MaxValue : (byte)(color.Z * byte.MaxValue)
            );
        }

        public static readonly Pixel Black = 0x000000;
        public static readonly Pixel CornflowerBlue = 0x6495ED;
        public static readonly Pixel Red = 0xFF0000;
    }
}
