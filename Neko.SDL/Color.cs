using System.Numerics;
using System.Runtime.InteropServices;

namespace Neko.Sdl;

[StructLayout(LayoutKind.Explicit)]
public struct Color {
    [FieldOffset(0)] public byte R;
    [FieldOffset(1)] public byte G;
    [FieldOffset(2)] public byte B;
    [FieldOffset(3)] public byte A;
    [FieldOffset(0)] public uint Packed;
    public Color(byte r, byte g, byte b, byte a) {
        Packed = 0;
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(uint packed) {
        Packed = packed;
    }

    public Color(ColorF color) {
        R = (byte) Math.Clamp(color.R * 255, Byte.MinValue, Byte.MaxValue);
        G = (byte) Math.Clamp(color.G * 255, Byte.MinValue, Byte.MaxValue);
        B = (byte) Math.Clamp(color.B * 255, Byte.MinValue, Byte.MaxValue);
        A = (byte) Math.Clamp(color.A * 255, Byte.MinValue, Byte.MaxValue);
    }
    
    public Color(Vector4 color) {
        R = (byte) Math.Clamp(color.X * 255, Byte.MinValue, Byte.MaxValue);
        G = (byte) Math.Clamp(color.Y * 255, Byte.MinValue, Byte.MaxValue);
        B = (byte) Math.Clamp(color.Z * 255, Byte.MinValue, Byte.MaxValue);
        A = (byte) Math.Clamp(color.W * 255, Byte.MinValue, Byte.MaxValue);
    }
    public Color(Vector3 color) {
        R = (byte) Math.Clamp(color.X * 255, Byte.MinValue, Byte.MaxValue);
        G = (byte) Math.Clamp(color.Y * 255, Byte.MinValue, Byte.MaxValue);
        B = (byte) Math.Clamp(color.Z * 255, Byte.MinValue, Byte.MaxValue);
        A = 255;
    }
    
    public static explicit operator Color(ColorF o) => new(o);
}

[StructLayout(LayoutKind.Sequential)]
public struct ColorF {
    public float R;
    public float G;
    public float B;
    public float A;
    
    public ColorF(float r, float g, float b, float a = 1f) {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public ColorF(Vector4 color) {
        R = color.X;
        G = color.Y;
        B = color.Z;
        A = color.W;
    }
    
    public ColorF(Vector3 color) {
        R = color.X;
        G = color.Y;
        B = color.Z;
        A = 1f;
    }

    public ColorF(Color color) {
        R = color.R / 255f;
        G = color.G / 255f;
        B = color.B / 255f;
        A = color.A / 255f;
    }
    
    public static explicit operator ColorF(Color o) => new(o);
}
