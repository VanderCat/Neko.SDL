using System.Runtime.CompilerServices;
using Neko.Sdl.Extra;

namespace Neko.Sdl;

internal unsafe class Util {
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ConvertSdlArrayToManaged<T>(T* ptr, in uint count) where T : unmanaged {
        if (count == 0)
            return [];
        var array = new T[count];
        fixed(T* arrayPtr = array)
            Unsafe.CopyBlock(arrayPtr, ptr, count);
        UnmanagedMemory.Free(ptr);
        return array;
    }
}