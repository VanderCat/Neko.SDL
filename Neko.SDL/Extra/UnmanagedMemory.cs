namespace Neko.Sdl.Extra;

public static unsafe class UnmanagedMemory {
    public static nint Calloc(nuint length, nuint size) => SDL_calloc(length, size);
    public static nint Malloc(nuint size) => SDL_malloc(size);
    public static nint Relloc(nint old, nuint size) => SDL_realloc(old, size);
    public static void Free(nint ptr) => SDL_free(ptr);
    public static void Free(void* ptr) => SDL_free(ptr);
    
    public static nint AlignedAlloc(nuint alignment, nuint size) => SDL_aligned_alloc(alignment, size);
    public static void AlignedFree(nint ptr) => SDL_aligned_free(ptr);
}