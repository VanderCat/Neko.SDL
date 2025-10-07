using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Extra.StandardLibrary;

public partial class LibcUnmanagedMemoryManager : IUnmanagedMemoryManager {

    [LibraryImport("c")]
    private static partial IntPtr malloc(UIntPtr size);
    [LibraryImport("c")]
    private static partial IntPtr calloc(UIntPtr nmemb, UIntPtr size);
    [LibraryImport("c")]
    private static partial IntPtr realloc(IntPtr mem, UIntPtr size);
    [LibraryImport("c")]
    private static partial IntPtr free(IntPtr mem);
    
    public IntPtr Malloc(UIntPtr size) => malloc(size);

    public IntPtr Calloc(UIntPtr nmemb, UIntPtr size) => calloc(nmemb, size);

    public IntPtr ReAlloc(IntPtr mem, UIntPtr size) => realloc(mem, size);

    public void Free(IntPtr mem) => free(mem);
}