namespace Neko.Sdl.Video;

public static unsafe class PixelFormatExtensions {
    /// <summary>
    /// Convert one of the enumerated pixel formats to a bpp value and RGBA masks
    /// </summary>
    /// <param name="pixelFormat">one of the PixelFormat values</param>
    /// <param name="bpp">a bits per pixel value; usually 15, 16, or 32</param>
    /// <param name="Rmask">the red mask for the format</param>
    /// <param name="Gmask">the green mask for the format</param>
    /// <param name="Bmask">the blue mask for the format</param>
    /// <param name="Amask">the alpha mask for the format</param>
    public static void GetMasks(this PixelFormat pixelFormat, out int bpp, out uint Rmask, out uint Gmask, out uint Bmask, out uint Amask) {
        int _bpp;
        uint rmask, gmask, bmask, amask;
        SDL_GetMasksForPixelFormat((SDL_PixelFormat)pixelFormat, &_bpp, &rmask, &gmask, &bmask, &amask);
        bpp = _bpp;
        Rmask = rmask;
        Gmask = gmask;
        Bmask = bmask;
        Amask = amask;
    }

    /// <summary>
    /// Create an SDL_PixelFormatDetails structure corresponding to a pixel format.
    /// </summary>
    /// <remarks>
    /// Returned structure may come from a shared global cache (i.e. not newly allocated),
    /// and hence could not be modified.
    /// </remarks>
    /// <param name="pixelFormat"></param>
    public static void GetDetails(this PixelFormat pixelFormat) => PixelFormatDetails.GetOrCreate(pixelFormat);

    /// <summary>
    /// Convert a bpp value and RGBA masks to an enumerated pixel format.
    /// </summary>
    /// <param name="bpp">a bits per pixel value; usually 15, 16, or 32</param>
    /// <param name="Rmask">the red mask for the format</param>
    /// <param name="Gmask">the green mask for the format</param>
    /// <param name="Bmask">the blue mask for the format</param>
    /// <param name="Amask">the alpha mask for the format</param>
    /// <returns>the <see cref="PixelFormat"/> value corresponding to the format masks, or <see cref="PixelFormat.Unknown"/> if there isn't a match</returns>
    public static PixelFormat GetFromMask(int bpp, uint Rmask, uint Gmask, uint Bmask, uint Amask) => 
        (PixelFormat)SDL_GetPixelFormatForMasks(bpp, Rmask, Gmask, Bmask, Amask);
}