namespace Neko.Sdl.Video;

public static class VideoDriver {
    public static string? Current => SDL_GetCurrentVideoDriver();
    public static int Count => SDL_GetNumVideoDrivers();

    public static string? Get(int index) => SDL_GetVideoDriver(index);
}