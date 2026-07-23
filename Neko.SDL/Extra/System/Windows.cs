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

    /// <summary>
    /// Set a callback for every Windows message, run before TranslateMessage().
    /// </summary>
    /// <param name="callback">the function to call</param>
    /// <remarks>
    /// The callback may modify the message, and should return true if the message should continue to be processed, or
    /// false to prevent further processing.
    /// </remarks>
    public static void SetMessageHook(MessageHook callback) {
        _callback?.Dispose();
        _callback = callback.Pin();
        SDL_SetWindowsMessageHook(&NativeCallback, _callback.Pointer);
    }
    
    /// <summary>
    /// Remove a callback before TranslateMessage()
    /// </summary>
    public static void RemoveMessageHook() {
        _callback?.Dispose();
        _callback = null;
        SDL_SetWindowsMessageHook(null, 0);
    }

    /// <summary>
    /// Register a win32 window class for SDL's use.
    /// </summary>
    /// <param name="name">the window class name. If null, SDL currently uses "SDL_app" but this isn't guaranteed.</param>
    /// <param name="style">the value to use in WNDCLASSEX::style.</param>
    /// <param name="hInst">the HINSTANCE to use in WNDCLASSEX::hInstance. If zero, SDL will use GetModuleHandle(NULL) instead.</param>
    /// <remarks>
    /// <p>
    /// This can be called to set the application window class at startup. It is safe to call this multiple times, as
    /// long as every call is eventually paired with a call to <see cref="UnregisterApp"/>, but a second registration
    /// attempt while a previous registration is still active will be ignored, other than to increment a counter.
    /// </p>
    /// <p>
    /// Most applications do not need to, and should not, call this directly; SDL will call it when initializing the
    /// video subsystem.
    /// </p>
    /// <p>
    /// If name is NULL, SDL currently uses (CS_BYTEALIGNCLIENT | CS_OWNDC) for the style, regardless of what is
    /// specified here.
    /// </p>
    /// </remarks>
    public static void RegisterApp(string? name, uint style, nint hInst) {
        if (name is not null) {
            using var rented = name.RentUtf8();
            fixed(byte* ptr = rented.Rented)
                SDL_RegisterApp(ptr, style, hInst).ThrowIfError();
            return;
        }
        SDL_RegisterApp((byte*)null, style, hInst).ThrowIfError();
    }

    /// <summary>
    /// Deregister the win32 window class from an <see cref="RegisterApp"/> call.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This can be called to undo the effects of <see cref="RegisterApp"/>.
    /// </p>
    /// <p>
    /// Most applications do not need to, and should not, call this directly; SDL will call it when deinitializing the
    /// video subsystem.
    /// </p>
    /// <p>
    /// It is safe to call this multiple times, as long as every call is eventually paired with a prior call to
    /// <see cref="RegisterApp"/>. The window class will only be deregistered when the registration counter in
    /// <see cref="RegisterApp"/> decrements to zero through calls to this function.
    /// </p>
    /// </remarks>
    public static void UnregisterApp() => SDL_UnregisterApp();
}