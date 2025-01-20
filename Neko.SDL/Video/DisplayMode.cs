using Neko.Sdl.CodeGen;

namespace Neko.Sdl.Video;

public unsafe partial class DisplayMode : SdlWrapper<SDL_DisplayMode> {
    [GenAccessor("displayID", true)]
    public partial uint DisplayId { get; set; } 

    public PixelFormat PixelFormat; 
    
    [GenAccessor("w")]
    public partial int Width { get; set; }
    
    [GenAccessor("h")]
    public partial int Height { get; set; }
    
    [GenAccessor("pixel_density")]
    public partial float PixelDensity { get; set; }
    
    [GenAccessor("refresh_rate")]
    public partial float RefreshRate { get; set; }
    
    [GenAccessor("refresh_rate_numerator")]
    public partial int RefreshRateNumerator { get; set; }
}