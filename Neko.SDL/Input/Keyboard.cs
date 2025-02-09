using Neko.Sdl.Video;

namespace Neko.Sdl.Input;

public static unsafe class Keyboard {
    public static Window? GetFocusedWindow() {
        var ptr = SDL_GetKeyboardFocus();
        return ptr is null ? null : Window.GetFromPtr(ptr);
    }
}