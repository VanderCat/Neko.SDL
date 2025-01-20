using System.Drawing;
using Neko.Sdl.Video;

namespace Neko.Sdl.Input;

public static unsafe class TextInput {
    public static void Start(Window window) => SDL_StartTextInput(window).ThrowIfError();
    public static void Start(Window window, Properties properties) => 
        SDL_StartTextInputWithProperties(window, properties).ThrowIfError();
    public static void Stop(Window window) => SDL_StartTextInput(window).ThrowIfError();
    public static bool IsActive(Window window) => SDL_TextInputActive(window);

    public static void SetArea(Window window, Rectangle rect, int cursor) =>
        SDL_SetTextInputArea(window, (SDL_Rect*)&rect, cursor);

    public static void GetArea(Window window, out Rectangle rect, out int cursor) {
        var rect1 = new Rectangle();
        var cursor1 = 0;
        SDL_GetTextInputArea(window, (SDL_Rect*)&rect1, &cursor1);
        rect = rect1;
        cursor = cursor1;
    }
}