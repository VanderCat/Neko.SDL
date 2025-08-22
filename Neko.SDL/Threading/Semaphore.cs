namespace Neko.Sdl.Threading;

/// <summary>
/// A means to manage access to a resource, by count, between threads
/// </summary>
/// <remarks>
/// Semaphores (specifically, "counting semaphores"), let X number of threads request
/// access at the same time, each thread granted access decrementing a counter. When
/// the counter reaches zero, future requests block until a prior thread releases
/// their request, incrementing the counter again.
/// <br/><br/>
/// Wikipedia has a thorough explanation of the concept:
/// <br/><br/>
/// https://en.wikipedia.org/wiki/Semaphore_(programming)
/// </remarks>
public unsafe partial class Semaphore : SdlWrapper<SDL_Semaphore> {
    /// <summary>
    /// Create a semaphore
    /// </summary>
    /// <remarks>
    /// This function creates a new semaphore and initializes it with the value initialValue.
    /// Each wait operation on the semaphore will atomically decrement the semaphore value
    /// and potentially block if the semaphore value is 0. Each post operation will atomically
    /// increment the semaphore value and wake waiting threads and allow them to retry the
    /// wait operation.
    /// </remarks>
    public static Semaphore Create(uint initialValue) {
        var result = SDL_CreateSemaphore(initialValue);
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Destroy a semaphore
    /// </summary>
    /// <remarks>
    /// It is not safe to destroy a semaphore if there are threads currently waiting on it.
    /// </remarks>
    public override void Dispose() {
        SDL_DestroySemaphore(this);
        base.Dispose();
    }

    /// <summary>
    /// Atomically increment a semaphore's value and wake waiting threads
    /// </summary>
    public void Signal() => SDL_SignalSemaphore(this);
    
    /// <summary>
    /// See if a semaphore has a positive value and decrement it if it does
    /// </summary>
    /// <remarks>
    /// This function checks to see if the semaphore pointed to by sem has a positive value
    /// and atomically decrements the semaphore value if it does. If the semaphore doesn't
    /// have a positive value, the function immediately returns false.
    /// </remarks>
    public void TryWait() => SDL_TryWaitSemaphore(this);
    
    /// <summary>
    /// The current value of a semaphore
    /// </summary>
    public uint Value => SDL_GetSemaphoreValue(this);
    
    /// <summary>
    /// Wait until a semaphore has a positive value and then decrements it
    /// </summary>
    /// <remarks>
    /// This function suspends the calling thread until the semaphore pointed to by sem has
    /// a positive value, and then atomically decrement the semaphore value.
    /// <br/><br/>
    /// This function is the equivalent of calling <see cref="WaitTimeout"/> with a time
    /// length of -1.
    /// </remarks>
    public void Wait() => SDL_WaitSemaphore(this);
    
    /// <summary>
    /// Wait until a semaphore has a positive value and then decrements it
    /// </summary>
    /// <param name="timeoutMs"></param>
    /// <remarks>
    /// This function suspends the calling thread until either the semaphore pointed to by
    /// sem has a positive value or the specified time has elapsed. If the call is successful
    /// it will atomically decrement the semaphore value.
    /// </remarks>
    public void WaitTimeout(int timeoutMs)=> SDL_WaitSemaphoreTimeout(this, timeoutMs);
}