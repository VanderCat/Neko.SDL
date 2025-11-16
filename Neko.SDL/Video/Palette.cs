namespace Neko.Sdl.Video;

public unsafe partial class Palette : SdlWrapper<SDL_Palette> {
    /// <summary>
    /// Create a palette structure with the specified number of color entries.
    /// </summary>
    /// <param name="ncolors">represents the number of color entries in the color palette.</param>
    public static Palette Create(int ncolors) {
        var result = SDL_CreatePalette(ncolors);
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Set a range of colors in a palette.
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="firstcolor"></param>
    public void SetColors(Span<Color> colors, int firstcolor = 0) {
        fixed(Color* colorsPtr = colors)
            SDL_SetPaletteColors(this, (SDL_Color*)colorsPtr, firstcolor, colors.Length).ThrowIfError();
    }
    
    public override void Dispose() => SDL_DestroyPalette(this);
    
}