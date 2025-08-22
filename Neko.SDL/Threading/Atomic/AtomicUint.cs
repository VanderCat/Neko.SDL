using System.Runtime.CompilerServices;

namespace Neko.Sdl.Threading.Atomic;

/// <summary>
/// A type representing an atomic unsigned 32-bit value
/// </summary>
/// <remarks>
/// This can be used to manage a value that is synchronized across multiple CPUs without a race
/// condition; when an app sets a value with SDL_SetAtomicU32 all other threads, regardless of
/// the CPU it is running on, will see that value when retrieved with SDL_GetAtomicU32,
/// regardless of CPU caches, etc.
/// <br/><br/>
/// This is also useful for atomic compare-and-swap operations: a thread can change the value
/// as long as its current value matches expectations. When done in a loop, one can guarantee
/// data consistency across threads without a lock (but the usual warnings apply: if you don't
/// know what you're doing, or you don't do it carefully, you can confidently cause any number
/// of disasters with this, so in most cases, you should use a mutex instead of this!).
/// <br/><br/>
/// This is a struct so people don't accidentally use numeric operations on it directly. You
/// have to use SDL atomic functions.
/// </remarks>
public unsafe struct AtomicUint {
    private uint _value;
    
    public uint Value {
        get => SDL_GetAtomicU32(_ptr);
        set => SDL_SetAtomicU32(_ptr, value);
    }
    
    private SDL_AtomicU32* _ptr => (SDL_AtomicU32*)Unsafe.AsPointer(ref _value);
    
    /// <summary>
    /// Set an atomic variable to a new value if it is currently an old value
    /// </summary>
    /// <param name="oldval">the old value</param>
    /// <param name="newval">the new value</param>
    /// <returns>Returns true if the atomic variable was set, false otherwise</returns>
    public bool CompareAndSwap(uint oldval, uint newval) => SDL_CompareAndSwapAtomicU32(_ptr, oldval, newval);
}