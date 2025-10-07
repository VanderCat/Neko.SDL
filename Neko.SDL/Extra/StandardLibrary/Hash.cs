namespace Neko.Sdl.Extra.StandardLibrary;

public static unsafe class Hash {
    public static ushort Crc16<T>(ushort crc, Span<T> data) where T : unmanaged {
        fixed (T* dataPtr = data)
            return SDL_crc16(crc, (IntPtr)dataPtr, (nuint)data.Length);
    }

    public static ushort Crc16<T>(Span<T> data) where T : unmanaged => Crc16(0, data);

    public static uint Crc32<T>(uint crc, Span<T> data) where T : unmanaged {
        fixed (T* dataPtr = data)
            return SDL_crc32(crc, (IntPtr)dataPtr, (nuint)data.Length);
    }
    
    public static uint Crc32<T>(Span<T> data) where T : unmanaged => Crc32(0, data);

    public static uint Murmur3<T>(Span<T> data, uint seed) where T : unmanaged {
        fixed (T* dataPtr = data)
            return SDL_murmur3_32((IntPtr)dataPtr, (nuint)data.Length, seed);
    }
    
}