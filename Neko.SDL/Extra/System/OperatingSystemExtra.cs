namespace Neko.Sdl.Extra.System;

public static class OperatingSystemExtra {
    /// <summary>
    /// Get the application sandbox environment, if any.
    /// </summary>
    public static Sandbox Sandbox => (Sandbox)SDL_GetSandbox();
    
    public static bool IsTablet => SDL_IsTablet();
    public static bool IsTv => SDL_IsTV();
    // TODO: SDL 3.6.0
    // public static bool IsUbuntuTouch => SDL_IsUbuntuTouch();
}