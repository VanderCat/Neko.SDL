using System.Runtime.InteropServices;

namespace Neko.Sdl.Extra.StandardLibrary;

public unsafe class NativeUnmanagedMemoryManager : IUnmanagedMemoryManager {
    public IntPtr Malloc(UIntPtr size) => (IntPtr)NativeMemory.Alloc(size);

    public IntPtr Calloc(UIntPtr nmemb, UIntPtr size) => (IntPtr)NativeMemory.Alloc(nmemb, size);

    public IntPtr ReAlloc(IntPtr mem, UIntPtr size) => (IntPtr)NativeMemory.Realloc((void*)mem, size);

    public void Free(IntPtr mem) => NativeMemory.Free((void*)mem);
}