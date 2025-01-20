using Neko.Sdl.Video;

namespace Neko.Sdl.Extra.MessageBox;

public static unsafe class MessageBox {
    public static void ShowSimple(string title, string message, MessageBoxFlags flags = MessageBoxFlags.Information,
        Window? window = null) {
        if (window is null)
            SDL_ShowSimpleMessageBox((SDL_MessageBoxFlags)flags, title, message, null).ThrowIfError();
        else 
            SDL_ShowSimpleMessageBox((SDL_MessageBoxFlags)flags, title, message, window).ThrowIfError();
    }
}