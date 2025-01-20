using System.Numerics;
using Neko.Sdl.Extra;

namespace Neko.Sdl.Input;

public static unsafe class Mouse {
    public struct MouseState {
        public MouseButtonFlags Flags;
        public Vector2 Position;
    }
    
    public static bool Capture {
        set => SDL_CaptureMouse(value).ThrowIfError();
    }

    public static Cursor CurrentCursor => SDL_GetCursor();
    public static Cursor DefaultCursor => SDL_GetDefaultCursor();

    public static MouseState State {
        get {
            var state = new MouseState();
            state.Flags = (MouseButtonFlags)SDL_GetMouseState(&state.Position.X, &state.Position.Y);
            return state;
        }
    }
    
    public static MouseState GlobalState {
        get {
            var state = new MouseState();
            state.Flags = (MouseButtonFlags)SDL_GetGlobalMouseState(&state.Position.X, &state.Position.Y);
            return state;
        }
    }
    
    public static MouseState RelativeState {
        get {
            var state = new MouseState();
            state.Flags = (MouseButtonFlags)SDL_GetRelativeMouseState(&state.Position.X, &state.Position.Y);
            return state;
        }
    }

    public static uint[] GetAll() {
        int count;
        var ptr = SDL_GetMice(&count);
        if (ptr is null) throw new SdlException("");
        var arr = new Span<uint>(ptr, count).ToArray();
        UnmanagedMemory.Free(ptr);
        return arr;
    }

    public static string GetNameFor(uint id) {
        var str = SDL_GetMouseNameForID((SDL_MouseID)id);
        if (str is null) throw new SdlException("");
        return str;
    }
    
    public static bool HasMouse => SDL_HasMouse();

    public static void Warp(float x, float y) =>
        SDL_WarpMouseGlobal(x, y);
    
    public static void Warp(Vector2 position) =>
        Warp(position.X, position.Y);
}