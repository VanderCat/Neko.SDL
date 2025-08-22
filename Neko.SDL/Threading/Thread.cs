using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Threading;

/// <summary>
/// SDL offers cross-platform thread management functions. These are mostly concerned with starting threads,
/// setting their priority, and dealing with their termination.
/// <br/><br/>
/// In addition, there is support for Thread Local Storage (data that is unique to each thread, but accessed from a single key).
/// <br/><br/>
/// On platforms without thread support (such as Emscripten when built without pthreads), these functions
/// still exist, but things like  <see cref="Create"/> will report failure without doing anything.
/// <br/><br/>
/// If you're going to work with threads, you almost certainly need to have a good understanding of CategoryMutex as well.
/// <br/><br/>
/// This part of the SDL API handles management of threads, but an app also will need locks to manage thread safety. Those pieces are in CategoryMutex.
/// </summary>
public unsafe partial class Thread : SdlWrapper<SDL_Thread> {
    
    /// <summary>
    /// Create a new thread with a default stack size
    /// </summary>
    /// <param name="fn">function pointer to run</param>
    /// <param name="name">the name of the thread</param>
    /// <remarks>
    /// This is a convenience function, equivalent to calling SDL_CreateThreadWithProperties with the following properties set:
    /// <ul>
    /// <li>SDL_PROP_THREAD_CREATE_NAME_STRING: name</li>
    /// <li>SDL_PROP_THREAD_CREATE_STACKSIZE_NUMBER: 0</li>
    /// </ul>
    /// </remarks>
    public static Thread Create(ThreadFunction fn, string? name = null) {
        if (OperatingSystem.IsWindows())
            return WindowsThread.Create(fn, name);
        var fnPin = fn.Pin(GCHandleType.Normal);
        return SDL_CreateThreadRuntime(&NativeThreadFunc, name, fnPin.Pointer, 0, 0);
    }
    
    /// <summary>
    /// Create a new thread with with the specified properties
    /// </summary>
    /// <param name="fn">function pointer to run</param>
    /// <param name="properties">the properties to use</param>
    /// <remarks>
    /// These are the supported properties:
    /// <ul>
    /// <li>
    /// SDL_PROP_THREAD_CREATE_NAME_STRING:
    /// the name of the new thread, which might be available to debuggers. Optional, defaults to NULL.
    /// </li>
    /// <li>
    /// SDL_PROP_THREAD_CREATE_STACKSIZE_NUMBER:
    /// the size, in bytes, of the new thread's stack. Optional, defaults to 0 (system-defined default).
    /// </li>
    /// </ul>
    /// In this language bindings the following is overwritten, if you wish to opt-out, please use <see cref="CreateRaw"/>:
    /// <ul>
    /// <li>
    /// SDL_PROP_THREAD_CREATE_ENTRY_FUNCTION_POINTER
    /// </li>
    /// <li>
    /// SDL_PROP_THREAD_CREATE_USERDATA_POINTER
    /// </li>
    /// </ul>
    /// SDL makes an attempt to report SDL_PROP_THREAD_CREATE_NAME_STRING to the system, so that debuggers can display it. Not all platforms support this.
    /// <br/><br/>
    /// Thread naming is a little complicated: Most systems have very small limits for the string length (Haiku has 32 bytes, Linux currently has 16, Visual C++ 6.0 has nine!), and possibly other arbitrary rules. You'll have to see what happens with your system's debugger. The name should be UTF-8 (but using the naming limits of C identifiers is a better bet). There are no requirements for thread naming conventions, so long as the string is null-terminated UTF-8, but these guidelines are helpful in choosing a name:
    /// <br/><br/>
    /// https://stackoverflow.com/questions/149932/naming-conventions-for-threads
    /// <br/><br/>
    /// If a system imposes requirements, SDL will try to munge the string for it (truncate, etc), but the original string contents will be available from SDL_GetThreadName().
    /// <br/><br/>
    /// The size (in bytes) of the new stack can be specified with SDL_PROP_THREAD_CREATE_STACKSIZE_NUMBER. Zero means "use the system default" which might be wildly different between platforms. x86 Linux generally defaults to eight megabytes, an embedded device might be a few kilobytes instead. You generally need to specify a stack that is a multiple of the system's page size (in many cases, this is 4 kilobytes, but check your system documentation).
    /// </remarks>
    public static Thread Create(ThreadFunction fn, Properties properties) {
        var fnPin = fn.Pin(GCHandleType.Normal);
        properties.SetPointer("SDL_PROP_THREAD_CREATE_ENTRY_FUNCTION_POINTER", (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr, int>)&NativeThreadFunc);
        properties.SetPointer("SDL_PROP_THREAD_CREATE_USERDATA_POINTER", fnPin.Pointer);
        return CreateRaw(properties);
    }

    /// <summary>
    /// Create thread manually, using C# interop
    /// </summary>
    /// <param name="properties"></param>
    public static Thread CreateRaw(Properties properties) {
        if (OperatingSystem.IsWindows())
            return WindowsThread.Create(properties);
        return SDL_CreateThreadWithPropertiesRuntime(properties, 0, 0);
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static int NativeThreadFunc(IntPtr userdata) {
        using var fnPin = userdata.AsPin<ThreadFunction>(true);
        if (fnPin.TryGetTarget(out var function)) {
            return function();
        }
        Log.Error(0, "Failed to get a function to start a thread!");
        return -1;
    }

    /// <summary>
    /// Cleanup all TLS data for this thread
    /// </summary>
    /// <remarks>
    /// If you are creating your threads outside of SDL and then calling SDL functions,
    /// you should call this function before your thread exits, to properly clean up SDL memory.
    /// </remarks>
    public static void CleanupTLS() => SDL_CleanupTLS();

    /// <summary>
    /// Let a thread clean up on exit without intervention
    /// </summary>
    /// <remarks>
    /// A thread may be "detached" to signify that it should not remain until another
    /// thread has called <see cref="Wait"/> on it. Detaching a thread is useful for long-running
    /// threads that nothing needs to synchronize with or further manage. When a detached
    /// thread is done, it simply goes away.
    /// <br/><br/>
    /// There is no way to recover the return code of a detached thread. If you need this,
    /// don't detach the thread and instead use <see cref="Wait"/>.
    /// <br/><br/>
    /// Once a thread is detached, you should usually assume the <see cref="Thread"/> isn't safe to
    /// reference again, as it will become invalid immediately upon the detached thread's exit,
    /// instead of remaining until someone has called <see cref="Wait"/> to finally clean it up.
    /// As such, don't detach the same thread more than once.
    /// <br/><br/>
    /// If a thread has already exited when passed to <see cref="Detach"/>, it will stop waiting
    /// for a call to <see cref="Wait"/> and clean up immediately. It is not safe to detach a
    /// thread that might be used with <see cref="Wait"/>.
    /// <br/><br/>
    /// You may not call <see cref="Wait"/> on a thread that has been detached. Use either that
    /// function or this one, but not both, or behavior is undefined.
    /// </remarks>
    public void Detach() => SDL_DetachThread(this);

    /// <summary>
    /// Get the thread identifier for the current thread
    /// </summary>
    /// <remarks>
    /// This thread identifier is as reported by the underlying operating system.
    /// If SDL is running on a platform that does not support threads the return value will always be zero.
    /// <br/><br/>
    /// This function also returns a valid thread ID when called from the main thread.
    /// </remarks>
    public static ulong Id => (ulong)SDL_GetCurrentThreadID();
    
    /// <summary>
    /// Get the thread identifier for the specified thread
    /// </summary>
    /// <remarks>
    /// This thread identifier is as reported by the underlying operating system.
    /// If SDL is running on a platform that does not support threads the return value will always be zero
    /// </remarks>
    public ulong GetId() => (ulong)SDL_GetThreadID(this);

    /// <summary>
    /// Get the thread name as it was specified on create
    /// </summary>
    public string? Name => SDL_GetThreadName(this);

    /// <summary>
    /// Get the current state of a thread
    /// </summary>
    public ThreadState State => (ThreadState)SDL_GetThreadState(this);

    private int TLSId;
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void TlsDestructor(IntPtr ptr) {
        var pin = ptr.AsPin<object>(true);
        pin.Dispose();
        // if (pin.TryGetTarget(out var obj)) 
        //     if (obj is IDisposable disposable) 
        //         disposable.Dispose();
    }

    /// <summary>
    /// Get or set the current thread's value associated with a thread local storage ID.
    /// </summary>
    /// <remarks>
    /// If the thread local storage ID is not initialized (the value is 0), a new ID will be created in a thread-safe way, so all calls using a pointer to the same ID will refer to the same local storage.
    /// <br/><br/>
    /// Note that replacing a value from a previous call to this function on the same thread does not call the previous value's destructor!
    /// <br/><br/>
    /// destructor can be NULL; it is assumed that value does not need to be cleaned up if so.
    /// </remarks>
    public object? TLS {
        get {
            if (TLSId == 0)
                return null;
            var tlsptr = SDL_GetTLS((SDL_TLSID*)Unsafe.AsPointer(ref TLSId));
            if (tlsptr == 0)
                return null;
            var tlspin = tlsptr.AsPin<object>();
            return tlspin.Target;
        }
        set {
            if (value is null)
                SDL_SetTLS((SDL_TLSID*)Unsafe.AsPointer(ref TLSId), 0, null).ThrowIfError();
            else 
                SDL_SetTLS((SDL_TLSID*)Unsafe.AsPointer(ref TLSId), value.Pin(GCHandleType.Normal).Pointer, &TlsDestructor).ThrowIfError();
        }
    }

    public static ThreadPriority Priority {
        set {
            SDL_SetCurrentThreadPriority((SDL_ThreadPriority)value).ThrowIfError();
        }
    }

    /// <summary>
    /// Wait for a thread to finish
    /// </summary>
    /// <remarks>
    /// Threads that haven't been detached will remain until this function cleans them up.
    /// Not doing so is a resource leak.
    /// <br/><br/>
    /// Once a thread has been cleaned up through this function, the <see cref="Thread"/> that
    /// references it becomes invalid and should not be referenced again. As such, only
    /// one thread may call <see cref="Wait"/> on another.
    /// <br/><br/>
    /// The return code from the thread function is placed in the area pointed to by
    /// status.
    /// <br/><br/>
    /// You may not wait on a thread that has been used in a call to <see cref="Detach"/>.
    /// Use either that function or this one, but not both, or behavior is undefined.
    /// <br/><br/>
    /// Note that the thread pointer is freed by this function and is not valid afterward.
    /// </remarks>
    public void Wait(out int exitcode) {
        exitcode = 0;
        SDL_WaitThread(this, (int*)Unsafe.AsPointer(ref exitcode));
    }
}