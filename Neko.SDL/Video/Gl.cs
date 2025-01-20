using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neko.Sdl.Video;

public static unsafe class Gl {
    public class AttrubuteContainer {
        internal AttrubuteContainer() { }
        
        public void Reset() => SDL_GL_ResetAttributes();

        public bool TryGetValue(GlAttr attribute, [NotNullWhen(true)] out int? value) {
            var value1 = 0;
            if (SDL_GL_GetAttribute((SDL_GLAttr)(int)attribute, &value1)) {
                value = value1;
                return true;
            }
            value = null;
            return false;
        }
        
        public int this[GlAttr attribute]
        {
            get {
                var a = 0;
                SDL_GL_GetAttribute((SDL_GLAttr)(int)attribute, &a).ThrowIfError();
                return a;
            }
            set => SDL_GL_SetAttribute((SDL_GLAttr)(int)attribute, value).ThrowIfError();
        }
    }
    public static void LoadLibrary(string path) => SDL_GL_LoadLibrary(path).ThrowIfError();
    public static void UnloadLibrary() => SDL_GL_UnloadLibrary();

    public static IntPtr CreateContext(Window window) {
        var ptr = SDL_GL_CreateContext(window.Handle);
        if (ptr is null) throw new SdlException("");
        return (IntPtr)ptr;
    }

    public static void DestroyContext(IntPtr ptr) => SDL_GL_DestroyContext((SDL_GLContextState*)ptr);
    
    public static IntPtr CurrentContext {
        get {
            var ptr = SDL_GL_GetCurrentContext();
            if (ptr is null) throw new SdlException("");
            return (IntPtr)ptr;
        }
    }

    public static Window CurrentWindow {
        get {
            var ptr = SDL_GL_GetCurrentWindow();
            if (ptr is null) throw new SdlException("");
            return Window.GetFromPtr(ptr);
        }
    }

    public static void MakeGlCurrent(this Window window, IntPtr context) =>
        SDL_GL_MakeCurrent(window.Handle, (SDL_GLContextState*)context);

    public static IntPtr GetProcAddr(string proc) => 
        SDL_GL_GetProcAddress(proc);

    public static int SwapInterval {
        get {
            var a = 0;
            SDL_GL_GetSwapInterval((int*)Unsafe.AsPointer(ref a)).ThrowIfError();
            return a;
        }
        set => SDL_GL_SetSwapInterval(value);
    }

    public static bool TryGetSwapInterval(out int interval) {
        interval = 0;
        return SDL_GL_GetSwapInterval((int*)Unsafe.AsPointer(ref interval));
    }

    public static AttrubuteContainer Attributes = new();
    
    public static void SwapWindow(Window window) => SDL_GL_SwapWindow(window.Handle).ThrowIfError();
    public static void GlSwap(this Window window) => SwapWindow(window);

}