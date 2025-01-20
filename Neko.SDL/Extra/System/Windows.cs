using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neko.Sdl.Extra.System;

[SupportedOSPlatform("Windows")]
public static unsafe class Windows {
    public delegate bool MessageHook(IntPtr msg);
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe SDLBool NativeCallback(IntPtr userdata, MSG* msg) {
        var pin = userdata.AsPin<MessageHook>();
        var managedCallback = pin.Target;
        return managedCallback((IntPtr)msg);
    }
    
    private static Pin<MessageHook>? _callback;

    public static void SetMessageHook(MessageHook callback) {
        _callback?.Dispose();
        _callback = callback.Pin();
        SDL_SetWindowsMessageHook(&NativeCallback, _callback.Pointer);
    }
    
    public static void RemoveMessageHook() {
        _callback?.Dispose();
        _callback = null;
        SDL_SetWindowsMessageHook(null, 0);
    }
}