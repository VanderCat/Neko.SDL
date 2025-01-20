using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neko.Sdl.Extra.System;

[SupportedOSPlatform("Linux")]
public static unsafe class Linux {
    public static void SetThreadPriority(long threadId, int priority) =>
        SDL_SetLinuxThreadPriority(threadId, priority).ThrowIfError();
    
    public static void SetThreadPriorityAndPolicy(long threadId, int priority, int schedPolicy) =>
        SDL_SetLinuxThreadPriorityAndPolicy(threadId, priority, schedPolicy).ThrowIfError();
    
    public delegate bool X11EventHook(IntPtr msg);
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe SDLBool NativeCallback(IntPtr userdata, IntPtr msg) {
        var pin = userdata.AsPin<X11EventHook>();
        var managedCallback = pin.Target;
        return managedCallback(msg);
    }
    
    private static Pin<X11EventHook>? _callback;

    public static void SetMessageHook(X11EventHook callback) {
        _callback?.Dispose();
        _callback = callback.Pin();
        SDL_SetX11EventHook(&NativeCallback, _callback.Pointer);
    }
    
    public static void RemoveMessageHook() {
        _callback?.Dispose();
        _callback = null;
        SDL_SetX11EventHook(null, 0);
    }
}