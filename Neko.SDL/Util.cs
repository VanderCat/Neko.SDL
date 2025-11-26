using System.Buffers;
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
        var len = Encoding.UTF8.GetByteCount(str) + 1;
        var ptr = (byte*)UnmanagedMemory.Malloc((nuint)len);
        Encoding.UTF8.GetBytes(str, new Span<byte>(ptr, len));
        ptr[len-1] = 0;
        return ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventType EventType(this ref SDL_Event @event) => (EventType)@event.Type;

    public ref struct RentScope<T> : IDisposable {
        public readonly T[] Rented;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RentScope(int len) {
            Rented = ArrayPool<T>.Shared.Rent(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[](RentScope<T> rent) => rent.Rented;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Span<T>(RentScope<T> rent) => rent.Rented;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlySpan<T>(RentScope<T> rent) => rent.Rented;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => ArrayPool<T>.Shared.Return(Rented);
        
        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rented[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Rented[index] = value;
        }
        
        public int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rented.Length;
        }
        public ref T GetPinnableReference() => ref ((Span<T>)Rented).GetPinnableReference();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RentScope<T> RentArray<T>(int len) => new(len);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RentScope<byte> RentEncodedString(Encoding encoding, string str) {
        var len = encoding.GetByteCount(str);
        var rent = RentArray<byte>(len+1);
        rent.Rented[len] = 0;
        encoding.GetBytes(str, rent);
        return rent;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RentScope<byte> RentUtf8(this string str) => RentEncodedString(Encoding.UTF8, str);
}