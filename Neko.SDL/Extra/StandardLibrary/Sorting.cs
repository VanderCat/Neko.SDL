using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Extra.StandardLibrary;

public static unsafe class Sorting {
    private struct Userdata {
        public Type Type;
        public Delegate func;
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int CompareInternal(IntPtr userdata, IntPtr a, IntPtr b) {
        var udata = GCHandle.FromIntPtr(userdata).Target;
        
        if (udata is not Userdata data) {
            throw new ArgumentException();
        }
        var value1 = Marshal.PtrToStructure(a, data.Type);
        if (value1 is null)
            throw new ArgumentException();
        var value2 = Marshal.PtrToStructure(b, data.Type);
        if (value2 is null)
            throw new ArgumentException();
        object?[] stuff = [value1, value2];
        var result = data.func.DynamicInvoke(stuff);
        if (result == null)
            throw new ArgumentException();
        return (int)result;
    }
    
    public delegate int Compare<T>(ref T a, ref T b);

    /// <summary>
    /// Sort an array
    /// </summary>
    /// <param name="array">An array</param>
    /// <param name="compare">a function used to compare elements in the array</param>
    /// <typeparam name="T">A type of elements in the array</typeparam>
    /// <remarks>This will hard crash your application if something went wrong. Use managed sorting instead.</remarks>
    public static void QSort<T>(this Span<T> array, Compare<T> compare) where T : unmanaged {
        using var ptr = new Userdata {
            Type = typeof(T),
            func = compare
        }.Pin(GCHandleType.Normal);
        fixed(T* arrayPtr = array)
            SDL_qsort_r((IntPtr)arrayPtr, (nuint)array.Length, (nuint)sizeof(T), &CompareInternal, ptr.Pointer);
    }

    /// <inheritdoc cref="QSort{T}(Span{T}, Compare{T})"/>
    public static void QSort<T>(this T[] array, Compare<T> compare) where T : unmanaged =>
        QSort(array.AsSpan(), compare);

    /// <summary>
    /// Perform a binary search on a previously sorted array
    /// </summary>
    /// <param name="array">An array</param>
    /// <param name="key">a key equal to the element being searched for</param>
    /// <param name="compare">a function used to compare elements in the array</param>
    /// <typeparam name="T">A type of elements in the array</typeparam>
    /// <remarks>This will hard crash your application if something went wrong. Use managed searching instead.</remarks>
    public static void BinarySearch<T>(this Span<T> array, ref T key, Compare<T> compare) where T : unmanaged {
        using var ptr = new Userdata {
            Type = typeof(T),
            func = compare
        }.Pin(GCHandleType.Normal);
        fixed(T* arrayPtr = array)
            SDL_bsearch_r((IntPtr)Unsafe.AsPointer(ref key), (IntPtr)arrayPtr, (nuint)array.Length, (nuint)sizeof(T), &CompareInternal, ptr.Pointer);
    }
}