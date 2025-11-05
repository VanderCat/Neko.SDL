using System.Numerics;
using System.Runtime.InteropServices;
using Neko.Sdl.Video;

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

    /// <summary>
    /// Get RGB values from a pixel in the specified format.
    /// </summary>
    /// <param name="pixelvalue">a pixel value</param>
    /// <remarks>
    /// This function uses the entire 8-bit [0..255] range when converting color components
    /// from pixel formats with less than 8-bits per RGB component (e.g., a completely white pixel
    /// in 16-bit RGB565 format would return [0xff, 0xff, 0xff] not [0xf8, 0xfc, 0xf8]).
    /// </remarks>
    /// <param name="format">a pointer PixelFormatDetails describing the pixel format</param>
    /// 
    /// <param name="r">the red component</param>
    /// <param name="g">the green component</param>
    /// <param name="b">the blue component</param>
    public static unsafe void GetRGB(uint pixelvalue, PixelFormatDetails format, out byte r, out byte g, out byte b) {
        byte _r, _g, _b;
        SDL_GetRGB(pixelvalue, format.Handle, null, &_r, &_g, &_b);
        r = _r;
        g = _g;
        b = _b;
    }
    /// <inheritdoc cref="GetRGB(uint,Neko.Sdl.Video.PixelFormatDetails,out byte,out byte,out byte)"/>
    /// <param name="palette">an optional palette for indexed formats</param>
    public static unsafe void GetRGB(uint pixelvalue, PixelFormatDetails format, Palette palette, out byte r, out byte g, out byte b) {
        byte _r, _g, _b;
        SDL_GetRGB(pixelvalue, format.Handle, palette, &_r, &_g, &_b);
        r = _r;
        g = _g;
        b = _b;
    }
    
    /// <summary>
    /// Get RGB values from a pixel in the specified format.
    /// </summary>
    /// <param name="pixelvalue">a pixel value</param>
    /// <remarks>
    /// This function uses the entire 8-bit [0..255] range when converting color components
    /// from pixel formats with less than 8-bits per RGB component (e.g., a completely white pixel
    /// in 16-bit RGB565 format would return [0xff, 0xff, 0xff] not [0xf8, 0xfc, 0xf8]).
    /// If the surface has no alpha component, the alpha will be returned as 0xff (100% opaque).
    /// </remarks>
    /// <param name="format">a pointer PixelFormatDetails describing the pixel format</param>
    /// <param name="r">the red component</param>
    /// <param name="g">the green component</param>
    /// <param name="b">the blue component</param>
    /// <param name="a">the alpha component</param>
    public static unsafe void GetRGBA(uint pixelvalue, PixelFormatDetails format, out byte r, out byte g, out byte b, out byte a) {
        byte _r, _g, _b, _a;
        SDL_GetRGBA(pixelvalue, format.Handle, null, &_r, &_g, &_b, &_a);
        r = _r;
        g = _g;
        b = _b;
        a = _a;
    }
    /// <inheritdoc cref="GetRGBA(uint,Neko.Sdl.Video.PixelFormatDetails,out byte,out byte,out byte)"/>
    /// <param name="palette">an optional palette for indexed formats</param>
    public static unsafe void GetRGBA(uint pixelvalue, PixelFormatDetails format, Palette palette, out byte r, out byte g, out byte b, out byte a) {
        byte _r, _g, _b, _a;
        SDL_GetRGBA(pixelvalue, format.Handle, palette, &_r, &_g, &_b, &_a);
        r = _r;
        g = _g;
        b = _b;
        a = _a;
    }
    public static void GetRGBA(Color pixelvalue, PixelFormatDetails format, out byte r, out byte g, out byte b, out byte a) => 
        GetRGBA(pixelvalue.Packed, format, out r, out g, out b, out a);
    
    public static void GetRGB(Color pixelvalue, PixelFormatDetails format, out byte r, out byte g, out byte b) => 
        GetRGB(pixelvalue.Packed, format, out r, out g, out b);
    
    public static void GetRGBA(Color pixelvalue, PixelFormatDetails format, Palette palette, out byte r, out byte g, out byte b, out byte a) => 
        GetRGBA(pixelvalue.Packed, format, palette, out r, out g, out b, out a);
    
    public static void GetRGB(Color pixelvalue, PixelFormatDetails format, Palette palette, out byte r, out byte g, out byte b) => 
        GetRGB(pixelvalue.Packed, format, palette, out r, out g, out b);
    
    public static unsafe Color GetRGBA(Color pixelvalue, PixelFormatDetails format) {
        var color1 = new Color();
        SDL_GetRGBA(pixelvalue.Packed, format.Handle, null, &color1.R, &color1.G, &color1.B, &color1.A);
        return color1;
    }
    public static unsafe Color GetRGB(Color pixelvalue, PixelFormatDetails format) {
        var color1 = new Color();
        SDL_GetRGB(pixelvalue.Packed, format.Handle, null, &color1.R, &color1.G, &color1.B);
        return color1;
    }
    public static unsafe Color GetRGBA(Color pixelvalue, PixelFormatDetails format, Palette palette) {
        var color1 = new Color();
        SDL_GetRGBA(pixelvalue.Packed, format.Handle, palette, &color1.R, &color1.G, &color1.B, &color1.A);
        return color1;
    }
    public static unsafe Color GetRGB(Color pixelvalue, PixelFormatDetails format, Palette palette) {
        var color1 = new Color();
        SDL_GetRGB(pixelvalue.Packed, format.Handle, palette, &color1.R, &color1.G, &color1.B);
        return color1;
    }
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
