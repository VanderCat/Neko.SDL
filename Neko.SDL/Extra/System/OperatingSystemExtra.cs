namespace Neko.Sdl.Extra.System;

public static class OperatingSystemExtra {
    public static bool IsTablet() => SDL_IsTablet();
    public static bool IsTv() => SDL_IsTV();
}