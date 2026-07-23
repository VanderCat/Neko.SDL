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
    /// <summary>
    /// Use this function to set the animation callback on Apple iOS.
    /// </summary>
    /// <param name="window">the window for which the animation callback should be set.</param>
    /// <param name="interval">the number of frames after which callback will be called.</param>
    /// <param name="callback">the function to call for every frame.</param>
    /// <remarks>
    /// <para>
    /// For more information see:
    /// </para>
    /// <para>
    /// https://wiki.libsdl.org/SDL3/README-ios
    /// </para>
    /// <para>
    /// Note that if you use the "main callbacks" instead of a standard C main function, you don't have to use this API,
    /// as SDL will manage this for you.
    /// </para>
    /// <para>
    /// Details on main callbacks are here:
    /// </para>
    /// <para>
    /// https://wiki.libsdl.org/SDL3/README-main-functions
    /// </para>
    /// </remarks>
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

    /// <summary>
    /// Use this function to enable or disable the SDL event pump on Apple iOS.
    /// </summary>
    /// <param name="enabled">true to enable the event pump, false to disable it.</param>
    public static void SetEventPump(bool enabled) => SDL_SetiOSEventPump(enabled);

    /// <summary>
    /// Let iOS apps with external event handling report onApplicationDidChangeStatusBarOrientation.
    /// </summary>
    /// <remarks>
    /// This functions allows iOS apps that have their own event handling to hook into SDL to generate SDL events. This
    /// maps directly to an iOS-specific event, but since it doesn't do anything iOS-specific internally, it is
    /// available on all platforms, in case it might be useful for some specific paradigm. Most apps do not need to use
    /// this directly; SDL's internal event code will handle all this for windows created by SDL_CreateWindow!
    /// </remarks>
    public static void OnApplicationDidChangeStatusBarOrientation() => SDL_OnApplicationDidChangeStatusBarOrientation();
    /// <summary>
    /// Let iOS apps with external event handling report onApplicationDidEnterBackground.
    /// </summary>
    /// <inheritdoc cref="OnApplicationDidChangeStatusBarOrientation"/>
    public static void OnApplicationDidEnterBackground() => SDL_OnApplicationDidEnterBackground();
    /// <summary>
    /// Let iOS apps with external event handling report onApplicationDidEnterForeground.
    /// </summary>
    /// <inheritdoc cref="OnApplicationDidChangeStatusBarOrientation"/>
    public static void OnApplicationDidEnterForeground() => SDL_OnApplicationDidEnterForeground();
    /// <summary>
    /// Let iOS apps with external event handling report onApplicationDidReceiveMemoryWarning.
    /// </summary>
    /// <inheritdoc cref="OnApplicationDidChangeStatusBarOrientation"/>
    public static void OnApplicationDidReceiveMemoryWarning() => SDL_OnApplicationDidReceiveMemoryWarning();
    /// <summary>
    /// Let iOS apps with external event handling report onApplicationWillEnterBackground.
    /// </summary>
    /// <inheritdoc cref="OnApplicationDidChangeStatusBarOrientation"/>
    public static void OnApplicationWillEnterBackground() => SDL_OnApplicationWillEnterBackground();
    /// <summary>
    /// Let iOS apps with external event handling report onApplicationWillEnterForeground.
    /// </summary>
    /// <inheritdoc cref="OnApplicationDidChangeStatusBarOrientation"/>
    public static void OnApplicationWillEnterForeground() => SDL_OnApplicationWillEnterForeground();
    /// <summary>
    /// Let iOS apps with external event handling report onApplicationWillTerminate.
    /// </summary>
    /// <inheritdoc cref="OnApplicationDidChangeStatusBarOrientation"/>
    public static void OnApplicationWillTerminate() => SDL_OnApplicationWillTerminate();
}