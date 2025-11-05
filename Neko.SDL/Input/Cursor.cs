using Neko.Sdl.Video;

namespace Neko.Sdl.Input;

public unsafe partial class Cursor : SdlWrapper<SDL_Cursor> {
    public static Cursor CreateColor(Video.Surface surface, int hotX, int hotY) {
        var ptr = SDL_CreateColorCursor(surface, hotX, hotY);
        if (ptr is null) throw new SdlException("");
        return ptr;
    }
    public static Cursor Create(byte[] data, byte[] mask, int w, int h, int hotX, int hotY) {
        fixed(byte* dataPtr = data)
        fixed (byte* maskPtr = mask) {
            var ptr = SDL_CreateCursor(dataPtr, maskPtr, w, h, hotX, hotY);
            if (ptr is null) throw new SdlException("");
            return ptr;
        }
    }
    
    public static Cursor CreateSystem(SystemCursor cursor) {
        var ptr = SDL_CreateSystemCursor((SDL_SystemCursor)cursor);
        if (ptr is null) throw new SdlException("");
        return ptr;
    }

    public static bool IsVisible => SDL_CursorVisible();
    public static void Hide() => SDL_HideCursor().ThrowIfError();
    public static void Show() => SDL_ShowCursor().ThrowIfError();
    public static void Redraw() => SDL_SetCursor(null).ThrowIfError(); 
    public void SetActive() => SDL_SetCursor(this).ThrowIfError(); 

    public static Window GetFocusedWindow() => SDL_GetMouseFocus();

    public override void Dispose() {
        base.Dispose();
        SDL_DestroyCursor(this);
    }
}