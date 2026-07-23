using System.Runtime.Versioning;
using Neko.Sdl.Events;

namespace Neko.Sdl.Extra.System;

public static unsafe class GDK {
    /// <summary>
    /// Callback from the application to let the suspend continue.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This should be called in response to an <see cref="EventType.DidEnterBackground"/> event, which can be detected
    /// via event watch. However, do NOT call this function directly from within an event watch callback. Instead, wait
    /// until the app has suppressed all rendering operations, then call this from the application render thread.
    /// </p>
    /// <p>
    /// When using SDL_Render, this should be called after calling SDL_GDKSuspendRenderer.
    /// </p>
    /// <p>
    /// When using SDL_GPU, this should be called after calling SDL_GDKSuspendGPU.
    /// </p>
    /// <p>
    /// If you're writing your own D3D12 renderer, this should be called after calling ID3D12CommandQueue::SuspendX.
    /// </p>
    /// <p>
    /// This function is only needed for Xbox GDK support; all other platforms will do nothing and set an "unsupported"
    /// error message.
    /// </p>
    /// </remarks>
    public static void SuspendComplete() => SDL_GDKSuspendComplete();
    
    /// <summary>
    /// A reference to the default user handle for GDK.
    /// </summary>
    /// <remarks>
    /// This is effectively a synchronous version of XUserAddAsync, which always prefers the default user and allows
    /// a sign-in UI.
    /// </remarks>
    public static IntPtr DefaultUser {
        get {
            var result = (XUser*)0;
            if (!SDL_GetGDKDefaultUser(&result))
                throw new SdlException();
            return (IntPtr)result;
        }
    }
    
    /// <summary>
    /// A reference to the global async task queue handle for GDK, initializing if needed.
    /// </summary>
    /// <remarks>
    /// Once you are done with the task queue, you should call XTaskQueueCloseHandle to reduce the reference count to
    /// avoid a resource leak.
    /// </remarks>
    public static IntPtr TaskQueue {
        get {
            var result = (XTaskQueueObject*)0;
            if (!SDL_GetGDKTaskQueue(&result))
                throw new SdlException();
            return (IntPtr)result;
        }
    }
}