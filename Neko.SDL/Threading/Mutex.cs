namespace Neko.Sdl.Threading;

/// <remarks>
/// Mutexes (short for "mutual exclusion") are a synchronization primitive that
/// allows exactly one thread to proceed at a time.
/// <br/><br/>
/// Wikipedia has a thorough explanation of the concept:
/// <br/><br/>
/// https://en.wikipedia.org/wiki/Mutex
/// </remarks>
public unsafe partial class Mutex : SdlWrapper<SDL_Mutex> {
    /// <summary>
    /// Create a new mutex
    /// </summary>
    /// <remarks>
    /// All newly-created mutexes begin in the unlocked state.
    /// <br/><br/>
    /// Calls to <see cref="Lock"/> will not return while the mutex is locked
    /// by another thread. See <see cref="TryLock"/> to attempt to lock without blocking.
    /// <br/><br/>
    /// SDL mutexes are reentrant.
    /// </remarks>
    public static Mutex Create() {
        var result = SDL_CreateMutex();
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Destroy a mutex
    /// </summary>
    /// <remarks>
    /// This function must be called on any mutex that is no longer needed.
    /// Failure to destroy a mutex will result in a system memory or resource leak.
    /// While it is safe to destroy a mutex that is unlocked, it is not safe to attempt to
    /// destroy a locked mutex, and may result in undefined behavior depending on the platform.
    /// </remarks>
    public override void Dispose() {
        SDL_DestroyMutex(this);
        base.Dispose();
    }

    /// <summary>
    /// Lock the mutex
    /// </summary>
    /// <remarks>
    /// This will block until the mutex is available, which is to say it is in the
    /// unlocked state and the OS has chosen the caller as the next thread to lock
    /// it. Of all threads waiting to lock the mutex, only one may do so at a time.
    /// <br/><br/>
    /// It is legal for the owning thread to lock an already-locked mutex. It must
    /// unlock it the same number of times before it is actually made available
    /// for other threads in the system (this is known as a "recursive mutex").
    /// <br/><br/>
    /// This function will always block until it can lock the mutex, and return
    /// with it locked.
    /// </remarks>
    public void Lock() => SDL_LockMutex(this);

    /// <summary>
    /// Try to lock a mutex without blocking
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This works just like <see cref="Lock"/>, but if the mutex is not available,
    /// this function returns false immediately.
    /// <br/><br/>
    /// This technique is useful if you need exclusive access to a resource but
    /// don't want to wait for it, and will return to it to try again later.
    /// </remarks>
    public bool TryLock() => SDL_TryLockMutex(this);

    /// <summary>
    /// Unlock the mutex
    /// </summary>
    /// <remarks>
    /// It is legal for the owning thread to lock an already-locked mutex. It must
    /// unlock it the same number of times before it is actually made available
    /// for other threads in the system (this is known as a "recursive mutex").
    /// <br/><br/>
    /// It is illegal to unlock a mutex that has not been locked by the current
    /// thread, and doing so results in undefined behavior.
    /// </remarks>
    public void Unlock() => SDL_UnlockMutex(this);
}