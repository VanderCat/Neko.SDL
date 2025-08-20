using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;

namespace Neko.Sdl;

public static class Extensions {
    public static bool ThrowIfError(this SDLBool result, string message = "") {
        if (!result) throw new SdlException(message);
        return true;
    }

    public static SDL_Rect ToSdl(this Rectangle rectangle) => new(){
        x = rectangle.X,
        y = rectangle.Y,
        h = rectangle.Height,
        w = rectangle.Width,
    };
    
    public static SDL_FRect ToSdl(this RectangleF rectangle) => new(){
        x = rectangle.X,
        y = rectangle.Y,
        h = rectangle.Height,
        w = rectangle.Width,
    };

    public static Rectangle ToClr(this SDL_Rect rectangle) => 
        new(rectangle.x, rectangle.y, rectangle.w, rectangle.h);
    public static RectangleF ToClr(this SDL_FRect rectangle) => 
        new(rectangle.x, rectangle.y, rectangle.w, rectangle.h);


    internal unsafe class MarshalStringResult(byte** ptr) : IDisposable {
        public byte** Ptr = ptr;
        public static implicit operator byte**(MarshalStringResult o) => o.Ptr;
        public void Dispose() {
            if (Ptr == null) return;
            var start = Ptr;
            do UnmanagedMemory.Free(*Ptr);
            while (++Ptr != null);
            UnmanagedMemory.Free(start);
            Ptr = null;
        }
    }
     internal static unsafe MarshalStringResult MarshalToBytePtrPtr(this IEnumerable<string> strings) {
        ArgumentNullException.ThrowIfNull(strings);
        
        var list = strings as IList<string> ?? strings.ToList();
        var count = list.Count + 1;
        
        var arrayPtr = (byte**)UnmanagedMemory.Calloc<IntPtr>((nuint)count);
        
        for (var i = 0; i < count-1; i++) {
            var s = list[i];
            var utf8 = System.Text.Encoding.UTF8.GetBytes(s); 
            
            arrayPtr[i] = (byte*)UnmanagedMemory.Calloc((nuint)utf8.Length+1, 1);

            fixed (byte* ptr = utf8)
                Buffer.MemoryCopy(ptr, arrayPtr[i], utf8.Length, utf8.Length);
        }

        // trailing null already present because we cleared memory
        return new MarshalStringResult(arrayPtr);
    }
    
}