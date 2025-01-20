using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;

namespace Neko.Sdl;

public static unsafe class Clipboard {
    public static string[] GetMimeTypes() {
        var size = (UIntPtr)0;
        var a = SDL_GetClipboardMimeTypes((UIntPtr*)Unsafe.AsPointer(ref size));
        if (a is not null) throw new SdlException("");
        var arr = new string[size];
        var span = new Span<IntPtr>(a, (int)size);
        for (var i = 0; i < span.Length; i++) {
            arr[i] = Marshal.PtrToStringUTF8(span[i]) ?? string.Empty;
        }
        UnmanagedMemory.Free(a);
        return arr;
    }

    public static void Clear() => SDL_ClearClipboardData().ThrowIfError();

    //TODO: this is limited to 32bit, do we need to get around that, or not?
    public static byte[] GetData(string mimeType) {
        var size = (UIntPtr)0;
        var a = SDL_GetClipboardData(mimeType, (UIntPtr*)Unsafe.AsPointer(ref size));
        if (a == 0) throw new SdlException("");
        var arr = new byte[size];
        Marshal.Copy(a, arr, 0, (int)size);
        UnmanagedMemory.Free(a);
        return arr;
    }

    public delegate void Cleanup<in T>(T? userdata);
    public delegate byte[] Callback<in T>(T? userdata, string? mimeType);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void NativeCleanup(IntPtr userdata) {
        using var pin = new Pin<IClipboardDataProvider>(userdata);
        if (pin.TryGetTarget(out var target)) {
            target.CleanupClipboardData();
        }
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static IntPtr NativeCallback(IntPtr userdata, byte* mimeType, UIntPtr* size) {
        using var pin = new Pin<IClipboardDataProvider>(userdata);
        if (pin.TryGetTarget(out var target)) {
            var str = Marshal.PtrToStringUTF8((IntPtr)mimeType);
            var result = target.GetClipboardData(str);
            var ptr = Marshal.AllocHGlobal(result.Length);
            Marshal.Copy(result, 0, ptr, result.Length);
            *size = (nuint)result.Length;
            return ptr;
        }
        *size = 0;
        return 0;
    }

    public static void SetData(IClipboardDataProvider dataProvider) {
        var pin = dataProvider.Pin(GCHandleType.Normal);
        var mimeTypes = dataProvider.MimeTypes;
        SDL_SetClipboardData(&NativeCallback, &NativeCleanup, pin.Pointer, mimeTypes);
    }
    
    public static bool HasData(string mimeType) => SDL_HasClipboardData(mimeType);
    
    public static string? Text {
        get => SDL_GetClipboardText();
        set => SDL_SetClipboardText(value);
    }
    
    public static bool HasText => SDL_HasClipboardText();

    public static string? PrimarySelectionText {
        get => SDL_GetPrimarySelectionText();
        set => SDL_SetPrimarySelectionText(value);
    }

    public static bool HasPrimarySelectionText => SDL_HasPrimarySelectionText();
}