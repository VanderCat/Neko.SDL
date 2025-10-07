using System.Runtime.CompilerServices;
using Mutex = Neko.Sdl.Threading.Mutex;

namespace Neko.Sdl.Extra.StandardLibrary;

public unsafe class SdlDebugUnmanagedMemoryManager : IUnmanagedMemoryManager{
    private static delegate*unmanaged[Cdecl]<UIntPtr,IntPtr> _mallocFunc;
    private static delegate*unmanaged[Cdecl]<UIntPtr, UIntPtr, IntPtr> _callocFunc;
    private static delegate*unmanaged[Cdecl]<IntPtr, UIntPtr, IntPtr> _reallocFunc;
    private static delegate*unmanaged[Cdecl]<IntPtr, void> _freeFunc;

    static SdlDebugUnmanagedMemoryManager() {
        delegate*unmanaged[Cdecl]<UIntPtr,IntPtr> mallocFunc;
        delegate*unmanaged[Cdecl]<UIntPtr, UIntPtr, IntPtr> callocFunc;
        delegate*unmanaged[Cdecl]<IntPtr, UIntPtr, IntPtr> reallocFunc;
        delegate*unmanaged[Cdecl]<IntPtr, void> freeFunc;
        SDL_GetOriginalMemoryFunctions(&mallocFunc, &callocFunc, &reallocFunc, &freeFunc);
        _mallocFunc = mallocFunc;
        _callocFunc = callocFunc;
        _reallocFunc = reallocFunc;
        _freeFunc = freeFunc;
    }

    public HashSet<IntPtr> Allocations = new();
    public Mutex Mutex = Mutex.Create();

    public IntPtr Malloc(UIntPtr size) {
        var result = _mallocFunc(size);
        using(Mutex.ScopeLock()) Allocations.Add(result);
        return result;
    }

    public IntPtr Calloc(UIntPtr nmemb, UIntPtr size) {
        var result = _callocFunc(nmemb, size);
        using(Mutex.ScopeLock()) Allocations.Add(result);
        return result;
    }

    public IntPtr ReAlloc(IntPtr mem, UIntPtr size) {
        var result = _reallocFunc(mem, size);
        using (Mutex.ScopeLock()) {
            Allocations.Remove(mem);
            Allocations.Add(result);
        }

        return result;
    }

    public void Free(IntPtr mem) {
        using(Mutex.ScopeLock()) Allocations.Remove(mem);
        _freeFunc(mem);
    }

    public void Check() {
        using(Mutex.ScopeLock()) foreach (var allocation in Allocations) {
            Log.Critical(0, $"Application leaked at {allocation}");
        }
    }
}