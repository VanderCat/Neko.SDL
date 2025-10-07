using System.Runtime.InteropServices;

namespace Neko.Sdl.Extra.StandardLibrary;

public class MarshalUnmanagedMemoryManager : IUnmanagedMemoryManager {
    public IntPtr Malloc(UIntPtr size) {
        Console.WriteLine($"Allocating: {size}");
        var result =  Marshal.AllocHGlobal((IntPtr)size);
        Console.WriteLine("Success!");
        return result;
    }

    public IntPtr Calloc(UIntPtr nmemb, UIntPtr size) {
        return Malloc(nmemb * size);
    }

    public IntPtr ReAlloc(IntPtr mem, UIntPtr size) {
        return Marshal.ReAllocHGlobal(mem, (IntPtr)size);
    }

    public void Free(IntPtr mem) {
        Marshal.FreeHGlobal(mem);
    }
}