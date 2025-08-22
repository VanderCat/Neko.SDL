namespace Neko.Sdl.Threading;

/// <summary>
/// A means to block multiple threads until a condition is satisfied
/// </summary>
/// <remarks>
/// Condition variables, paired with an SDL_Mutex, let an app halt multiple threads
/// until a condition has occurred, at which time the app can release one or all waiting threads.
/// <br/><br/>
/// Wikipedia has a thorough explanation of the concept:
/// <br/><br/>
/// https://en.wikipedia.org/wiki/Condition_variable
/// </remarks>
public unsafe partial class Condition : SdlWrapper<SDL_Condition> {
    /// <summary>
    /// Create a condition variable
    /// </summary>
    public static Condition Create() {
        var result = SDL_CreateCondition();
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Restart all threads that are waiting on the condition variable
    /// </summary>
    public void Broadcast() => SDL_BroadcastCondition(this);
    
    /// <summary>
    /// Restart one of the threads that are waiting on the condition variable
    /// </summary>
    public void Signal() => SDL_SignalCondition(this);
    
    /// <summary>
    /// Wait until a condition variable is signaled
    /// </summary>
    /// <param name="mutex">the mutex used to coordinate thread access</param>
    /// <remarks>
    /// This function unlocks the specified mutex and waits for another thread to call
    /// <see cref="Signal"/> or <see cref="Broadcast"/> on the condition variable cond.
    /// Once the condition variable is signaled, the mutex is re-locked and the function
    /// returns.
    /// <br/><br/>
    /// The mutex must be locked before calling this function. Locking the mutex
    /// recursively (more than once) is not supported and leads to undefined behavior.
    /// <br/><br/>
    /// This function is the equivalent of calling <see cref="WaitTimeout"/> with a time
    /// length of -1.
    /// </remarks>
    public void Wait(Mutex mutex) => SDL_WaitCondition(this, mutex);
    
    /// <summary>
    /// Wait until a condition variable is signaled or a certain time has passed
    /// </summary>
    /// <param name="mutex">the mutex used to coordinate thread access</param>
    /// <param name="timeoutMs">the maximum time to wait, in milliseconds, or -1 to wait indefinitely</param>
    /// <remarks>
    /// This function unlocks the specified mutex and waits for another thread to call
    /// <see cref="Signal"/> or <see cref="Broadcast"/> on the condition variable cond,
    /// or for the specified time to elapse. Once the condition variable is signaled or
    /// the time elapsed, the mutex is re-locked and the function returns.
    /// <br/><br/>
    /// The mutex must be locked before calling this function. Locking the mutex
    /// recursively (more than once) is not supported and leads to undefined behavior.
    /// </remarks>
    public void WaitTimeout(Mutex mutex, int timeoutMs) => SDL_WaitConditionTimeout(this, mutex, timeoutMs);
    public override void Dispose() {
        SDL_DestroyCondition(this);
        base.Dispose();
    }
}