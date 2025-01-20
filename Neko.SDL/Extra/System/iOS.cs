using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Neko.Sdl.Video;

namespace Neko.Sdl.Extra.System;

[SupportedOSPlatform("iOS")]
public static unsafe class iOS {
    public delegate void AnimationCallback();
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void NativeCallback(IntPtr userdata) {
        var pin = userdata.AsPin<AnimationCallback>();
        var managedCallback = pin.Target;
        managedCallback();
    }

    private static Pin<AnimationCallback>? _callback;
    public static void SetAnimationCallback(Window window, int interval, AnimationCallback callback) {
        _callback?.Dispose();
        _callback = callback.Pin(GCHandleType.Normal);
        SDL_SetiOSAnimationCallback(window, interval, &NativeCallback, _callback.Pointer).ThrowIfError();
    }

    public static void RemoveAnimationCallback(Window window) {
        _callback?.Dispose();
        _callback = null;
        SDL_SetiOSAnimationCallback(window, 0, null, 0).ThrowIfError();
    }

    public static void SetEventPump(bool enabled) => SDL_SetiOSEventPump(enabled);
}