using System.Runtime.CompilerServices;
using System.Text;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl;

internal static unsafe class Util {
    
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte* ToUnmanagedPointer(this string str) {
        var buf = Encoding.UTF8.GetBytes(str);
        var ptr = UnmanagedMemory.Malloc((nuint)buf.Length);
        return (byte*)UnmanagedMemory.Copy(buf, new Span<byte>((void*)ptr, buf.Length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventType EventType(this ref SDL_Event @event) => (EventType)@event.Type;
}