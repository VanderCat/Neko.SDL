using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neko.Sdl.Extra.System;

[SupportedOSPlatform("Linux")]
public static unsafe class Linux {
    /// <summary>
    /// Sets the UNIX nice value for a thread.
    /// </summary>
    /// <param name="threadId">the Unix thread ID to change priority of.</param>
    /// <param name="priority">the new, Unix-specific, priority value.</param>
    /// <remarks>
    /// This uses setpriority() if possible, and RealtimeKit if available.
    /// </remarks>
    public static void SetThreadPriority(long threadId, int priority) =>
        SDL_SetLinuxThreadPriority(threadId, priority).ThrowIfError();
    
    /// <summary>
    /// Sets the priority (not nice level) and scheduling policy for a thread.
    /// </summary>
    /// <param name="threadId">the Unix thread ID to change priority of.</param>
    /// <param name="priority">the new SDL_ThreadPriority value.</param>
    /// <param name="schedPolicy">the new scheduling policy (SCHED_FIFO, SCHED_RR, SCHED_OTHER, etc...).</param>
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