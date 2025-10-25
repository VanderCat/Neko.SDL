namespace Neko.Sdl.Ttf;

public static unsafe class Extensions {
    public static void DrawText(this Surface surface, Text text, int x, int y) => TTF_DrawSurfaceText(text, x, y, surface).ThrowIfError();
}