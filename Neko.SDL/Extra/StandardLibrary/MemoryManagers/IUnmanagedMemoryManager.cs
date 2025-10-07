namespace Neko.Sdl.Extra.StandardLibrary;

public interface IUnmanagedMemoryManager {
    /// <param name="size">the size to allocate</param>
    /// <returns>a pointer to the allocated memory, or NULL if allocation failed</returns>
    /// <remarks>SDL will always ensure that the passed size is greater than 0</remarks>
    public nint Malloc(nuint size);
    /// <param name="nmemb">the number of elements in the array</param>
    /// <param name="size">the size of each element of the array</param>
    /// <returns>A pointer to the allocated array, or NULL if allocation failed</returns>
    /// <remarks>SDL will always ensure that the passed nmemb and size are both greater than 0</remarks>
    public nint Calloc(nuint nmemb, nuint size);
    /// <param name="mem">a pointer to allocated memory to reallocate, or NULL</param>
    /// <param name="size">the new size of the memory</param>
    /// <returns>A pointer to the newly allocated memory, or NULL if allocation failed</returns>
    /// <remarks>SDL will always ensure that the passed size is greater than 0</remarks>
    public nint ReAlloc(nint mem, nuint size);
    /// <param name="mem">a pointer to allocated memory</param>
    /// <remarks>SDL will always ensure that the passed mem is a non-NULL pointer</remarks>
    public void Free(nint mem);
}