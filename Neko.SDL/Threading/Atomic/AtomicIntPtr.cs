using System.Runtime.CompilerServices;

namespace Neko.Sdl.Threading.Atomic;

public unsafe struct AtomicIntPtr {
    private IntPtr _value;
    private IntPtr* _ptr => (IntPtr*)Unsafe.AsPointer(ref _value);
    public IntPtr Value {
        get => SDL_GetAtomicPointer(_ptr);
        set => SDL_SetAtomicPointer(_ptr, value);
    }
    /// <summary>
    /// Set an atomic variable to a new value if it is currently an old value
    /// </summary>
    /// <param name="oldval">the old value</param>
    /// <param name="newval">the new value</param>
    /// <returns>Returns true if the atomic variable was set, false otherwise</returns>
    public bool CompareAndSwap(IntPtr oldval, IntPtr newval) => SDL_CompareAndSwapAtomicPointer(_ptr, oldval, newval);
}