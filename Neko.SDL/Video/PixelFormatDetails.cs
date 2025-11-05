using System.Runtime.InteropServices;

namespace Neko.Sdl.Video;

public unsafe class PixelFormatDetails {
    private static Dictionary<IntPtr, PixelFormatDetails> _cache = new();
    public PixelFormatDetails(SDL_PixelFormatDetails* pixelFormatDetails) {
        Handle = pixelFormatDetails;
    }
    public static PixelFormatDetails GetOrCreate(PixelFormat pixelFormat) {
        var result = SDL_GetPixelFormatDetails((SDL_PixelFormat)pixelFormat);
        if (result is null)
            throw new SdlException();
        if (_cache.TryGetValue((IntPtr)result, out var value))
            return value;
        return _cache[(IntPtr)result] = new PixelFormatDetails(result);
    }
    internal SDL_PixelFormatDetails* Handle;
    public PixelFormat Format => (PixelFormat)Handle->format;
    public byte BitsPerPixel => Handle->bits_per_pixel;
    public byte BytesPerPixel => Handle->bits_per_pixel;
    public uint RMask => Handle->Rmask;
    public uint GMask => Handle->Gmask;
    public uint BMask => Handle->Bmask;
    public uint AMask => Handle->Amask;
    public byte RBits => Handle->Rbits;
    public byte GBits => Handle->Gbits;
    public byte BBits => Handle->Bbits;
    public byte ABits => Handle->Abits;
    public byte RShift => Handle->Rshift;
    public byte GShift => Handle->Gshift;
    public byte BShift => Handle->Bshift;
    public byte AShift => Handle->Ashift;
};