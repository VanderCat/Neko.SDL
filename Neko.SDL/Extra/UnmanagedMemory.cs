using System.Runtime.InteropServices;

namespace Neko.Sdl.Extra;

public static unsafe class UnmanagedMemory {
    
    /// <summary>
    /// Allocate a zero-initialized array
    /// </summary>
    /// <param name="length">the number of elements in the array</param>
    /// <param name="size">the size of each element of the array</param>
    /// <returns>Returns a pointer to the allocated array, or 0 if allocation failed</returns>
    /// <remarks>
    /// The memory returned by this function must be freed with <see cref="Free(IntPtr)"/>.
    /// <br/><br/>
    /// If either of length or size is 0, they will both be set to 1.
    /// <br/><br/>
    /// If the allocation is successful, the returned pointer is guaranteed to be aligned to either the fundamental alignment (alignof(max_align_t) in C11 and later) or 2 * sizeof(void *), whichever is smaller.
    /// </remarks>
    public static nint Calloc(nuint length, nuint size) => SDL_calloc(length, size);
    
    /// <summary>
    /// Allocate a zero-initialized array
    /// </summary>
    /// <param name="length">the number of elements in the array</param>
    /// <returns>Returns a pointer to the allocated array, or 0 if allocation failed</returns>
    /// <remarks>
    /// The memory returned by this function must be freed with <see cref="Free(IntPtr)"/>.
    /// <br/><br/>
    /// If either of length or size is 0, they will both be set to 1.
    /// <br/><br/>
    /// If the allocation is successful, the returned pointer is guaranteed to be aligned to either the fundamental alignment (alignof(max_align_t) in C11 and later) or 2 * sizeof(void *), whichever is smaller.
    /// </remarks>
    public static T* Calloc<T>(nuint length) where T : unmanaged => (T*)SDL_calloc(length, (nuint)Marshal.SizeOf<T>());
    
    /// <summary>
    /// Allocate uninitialized memory
    /// </summary>
    /// <param name="size"></param>
    /// <returns>Returns a pointer to the allocated memory, or 0 if allocation failed</returns>
    /// <remarks>
    /// The allocated memory returned by this function must be freed with <see cref="Free(IntPtr)"/>.
    /// <br/><br/>
    /// If size is 0, it will be set to 1.
    /// <br/><br/>
    /// If the allocation is successful, the returned pointer is guaranteed to be aligned to either the fundamental alignment (alignof(max_align_t) in C11 and later) or 2 * sizeof(void *), whichever is smaller.
    /// Use <see cref="AlignedAlloc"/> if you need to allocate memory aligned to an alignment greater than this guarantee.
    /// </remarks>
    public static nint Malloc(nuint size) => SDL_malloc(size);
    
    /// <summary>
    /// Change the size of allocated memory
    /// </summary>
    /// <param name="old">a pointer to allocated memory to reallocate, or null</param>
    /// <param name="size">the new size of the memory</param>
    /// <returns>Returns a pointer to the newly allocated memory, or 0 if allocation failed</returns>
    /// <remarks>
    /// The memory returned by this function must be freed with <see cref="Free(IntPtr)"/>.
    /// <br/><br/>
    /// If size is 0, it will be set to 1. Note that this is unlike some other C runtime realloc implementations, which may treat realloc(mem, 0) the same way as free(mem).
    /// <br/><br/>
    /// If mem is NULL, the behavior of this function is equivalent to SDL_malloc(). Otherwise, the function can have one of three possible outcomes:
    /// <ul>
    /// <li>If it returns the same pointer as mem, it means that mem was resized in place without freeing.</li>
    /// <li>If it returns a different non-NULL pointer, it means that mem was freed and cannot be dereferenced anymore.</li>
    /// <li>If it returns NULL (indicating failure), then mem will remain valid and must still be freed with SDL_free().</li>
    /// </ul>
    /// If the allocation is successfully resized, the returned pointer is guaranteed to be aligned to either the fundamental alignment (alignof(max_align_t) in C11 and later) or 2 * sizeof(void *), whichever is smaller.
    /// </remarks>
    public static nint Realloc(nint old, nuint size) => SDL_realloc(old, size);
    
    /// <inheritdoc cref="Realloc(IntPtr,UIntPtr)"/>
    public static nint Realloc(void* old, nuint size) => SDL_realloc((nint)old, size);
    
    /// <summary>
    /// Free allocated memory
    /// </summary>
    /// <param name="ptr">a pointer to allocated memory, or 0</param>
    /// <remarks>
    /// The pointer is no longer valid after this call and cannot be dereferenced anymore.
    /// <br/><br/>
    /// If mem is null, this function does nothing.
    /// </remarks>
    public static void Free(nint ptr) => SDL_free(ptr);
    
    /// <inheritdoc cref="Realloc(IntPtr,UIntPtr)"/>
    public static void Free(void* ptr) => SDL_free(ptr);
    
    
    /// <summary>
    /// Allocate memory aligned to a specific alignment
    /// </summary>
    /// <param name="alignment">the alignment of the memory</param>
    /// <param name="size">the size to allocate</param>
    /// <returns>Returns a pointer to the aligned memory, or NULL if allocation failed</returns>
    /// <remarks>
    /// The memory returned by this function must be freed with <see cref="AlignedFree(IntPtr)"/>, not <see cref="Free(IntPtr)"/>.
    /// <br/><br/>
    /// If alignment is less than the size of void *, it will be increased to match that.
    /// <br/><br/>
    /// The returned memory address will be a multiple of the alignment value, and the size of the memory allocated will be a multiple of the alignment value.
    /// </remarks>
    public static nint AlignedAlloc(nuint alignment, nuint size) => SDL_aligned_alloc(alignment, size);
    
    /// <summary>
    /// Free memory allocated by <see cref="AlignedAlloc"/>
    /// </summary>
    /// <param name="ptr">a pointer previously returned by <see cref="AlignedAlloc"/>, or NULL</param>
    /// <remarks>
    /// The pointer is no longer valid after this call and cannot be dereferenced anymore.
    /// <br/><br/>
    /// If mem is 0, this function does nothing.
    /// </remarks>
    public static void AlignedFree(nint ptr) => SDL_aligned_free(ptr);
    
    /// <inheritdoc cref="AlignedFree(UIntPtr)"/>
    public static void AlignedFree(void* ptr) => SDL_aligned_free((nint)ptr);
}