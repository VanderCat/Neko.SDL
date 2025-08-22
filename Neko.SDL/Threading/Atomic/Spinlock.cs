using System.Runtime.CompilerServices;

namespace Neko.Sdl.Threading.Atomic;

/// <summary>
/// An atomic spinlock
/// </summary>
/// <remarks>
/// The atomic locks are efficient spinlocks using CPU instructions, but are vulnerable to starvation
/// and can spin forever if a thread holding a lock has been terminated. For this reason you should
/// minimize the code executed inside an atomic lock and never do expensive things like API or system
/// calls while holding them.
/// <br/><br/>
/// They are also vulnerable to starvation if the thread holding the lock is lower priority than other
/// threads and doesn't get scheduled. In general you should use mutexes instead, since they have better
/// performance and contention behavior.
/// <br/><br/>
/// The atomic locks are not safe to lock recursively.
/// <br/><br/>
/// Porting Note: The spin lock functions and type are required and can not be emulated because they are
/// used in the atomic emulation code.
/// </remarks>
public unsafe struct Spinlock {
    private int _value;
    
    private int* _ptr => (int*)Unsafe.AsPointer(ref _value);
    
    /// <summary>
    /// Lock a spin lock by setting it to a non-zero value
    /// </summary>
    public void Lock() => SDL_LockSpinlock(_ptr);

    /// <summary>
    /// Try to lock a spin lock by setting it to a non-zero value
    /// </summary>
    /// <returns>true if the lock succeeded, false if the lock is already held</returns>
    /// <remarks>Always returns immediately</remarks>
    public bool TryLock() => SDL_TryLockSpinlock(_ptr);
    
    /// <summary>
    /// Unlock a spin lock by setting it to 0
    /// </summary>
    /// <remarks>Always returns immediately</remarks>
    public void Unlock() => SDL_UnlockSpinlock(_ptr);
}