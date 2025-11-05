namespace Neko.Sdl.Video;

public unsafe partial class Surface : SdlWrapper<SDL_Surface> {
    
    /// <summary>
    /// Set the palette used by a surface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Setting the palette keeps an internal reference to the palette, which can be safely destroyed afterwards.
    /// </para>
    /// <para>
    /// A single palette can be shared with many surfaces.
    /// </para>
    /// </remarks>
    /// <param name="palette"></param>
    public void SetPalette(Palette palette) => SDL_SetSurfacePalette(this, palette).ThrowIfError();
}