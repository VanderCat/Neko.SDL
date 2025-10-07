using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl;

/// <summary>
/// SDL provides access to the system clipboard, both for reading information from other processes
/// and publishing information of its own.
/// <br/><br/>
/// This is not just text! SDL apps can access and publish data by mimetype.
/// <br/><br/>
/// <b>Basic use (text)</b>
/// <br/><br/>
/// Obtaining and publishing simple text to the system clipboard is as using
/// <see cref="Text"/> property. These deal with C strings
/// in UTF-8 encoding. Data transmission and encoding conversion is completely managed by SDL.
/// <br/><br/>
/// <b>Clipboard callbacks (data other than text)</b>
/// <br/><br/>
/// Things get more complicated when the clipboard contains something other than text. Not
/// only can the system clipboard contain data of any type, in some cases it can contain
/// the same data in different formats! For example, an image painting app might let the
/// user copy a graphic to the clipboard, and offers it in .BMP, .JPG, or .PNG format
/// for other apps to consume.
/// <br/><br/>
/// Obtaining clipboard data ("pasting") like this is a matter of calling <see cref="GetData"/> 
/// and telling it the mimetype of the data you want. But how does one know if that format is
/// available? <see cref="HasData"/>  can report if a specific mimetype is offered, and
/// <see cref="GetMimeTypes"/>  can provide the entire list of mimetypes available, so the app
/// can decide what to do with the data and what formats it can support.
/// <br/><br/>
/// Setting the clipboard ("copying") to arbitrary data is done with <see cref="SetData"/> . The app
/// does not provide the data in this call, but rather the mimetypes it is willing to provide and
/// a clipboard provider that will generate the data. This allows massive
/// data sets to be provided to the clipboard, without any data being copied before it is explicitly
/// requested. More specifically, it allows an app to offer data in multiple formats without providing
/// a copy of all of them upfront. If the app has an image that it could provide in PNG or JPG format,
/// it doesn't have to encode it to either of those unless and until something tries to paste it.
/// <br/><br/>
/// <b>Primary Selection</b>
/// <br/><br/>
/// The X11 and Wayland video targets have a concept of the "primary selection" in addition to the
/// usual clipboard. This is generally highlighted (but not explicitly copied) text from various apps.
/// SDL offers APIs for this through <see cref="PrimarySelectionText"/> property.
/// SDL offers these APIs on platforms without this concept, too, but only so far that it will keep a
/// copy of a string that the app sets for later retrieval; the operating system will not ever attempt
/// to change the string externally if it doesn't support a primary selection.
/// </summary>
public static unsafe class Clipboard {
    
    /// <summary>
    /// Retrieve the list of mime types available in the clipboard
    /// </summary>
    /// <returns>array of strings with mime types</returns>
    public static string[] GetMimeTypes() {
        UIntPtr size = 0;
        var a = SDL_GetClipboardMimeTypes(&size);
        if (a is null) 
            throw new SdlException();
        var arr = new string[size];
        var span = new Span<IntPtr>(a, (int)size);
        for (var i = 0; i < span.Length; i++) {
            arr[i] = Marshal.PtrToStringUTF8(span[i]) ?? String.Empty;
        }
        UnmanagedMemory.Free(a);
        return arr;
    }

    /// <summary>
    /// Clear the clipboard data
    /// </summary>
    public static void Clear() => SDL_ClearClipboardData().ThrowIfError();

    //TODO: this is limited to 32bit, do we need to get around that, or not?
    /// <summary>
    /// Get the data from the clipboard for a given mime type
    /// </summary>
    /// <param name="mimeType">the mime type to read from the clipboard</param>
    /// <returns>the retrieved data buffer</returns>
    /// <exception cref="SdlException"></exception>
    public static byte[] GetData(string mimeType) {
        var size = (UIntPtr)0;
        var a = SDL_GetClipboardData(mimeType, (UIntPtr*)Unsafe.AsPointer(ref size));
        if (a == 0) {
            if (!SdlException.HasError)
                return [];
            throw new SdlException();
        }
        var arr = new byte[size];
        Marshal.Copy(a, arr, 0, (int)size);
        UnmanagedMemory.Free(a);
        return arr;
    }

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
            var ptr = UnmanagedMemory.Malloc((nuint)result.Length);
            Marshal.Copy(result, 0, ptr, result.Length);
            *size = (nuint)result.Length;
            return ptr;
        }
        *size = 0;
        return 0;
    }

    /// <summary>
    /// Offer clipboard data to the OS
    /// </summary>
    /// <param name="dataProvider"></param>
    /// <remarks>
    /// Tell the operating system that the application is offering clipboard data for each of the provided mime-types.
    /// Once another application requests the data the callback function will be called, allowing it to generate
    /// and respond with the data for the requested mime-type.
    /// <br/><br/>
    /// The size of text data does not include any terminator, and the text does not need to be
    /// null-terminated (e.g., you can directly copy a portion of a document)
    /// </remarks>
    public static void SetData(IClipboardDataProvider dataProvider) {
        var pin = dataProvider.Pin(GCHandleType.Normal);
        var mimeTypes = dataProvider.MimeTypes;
        SDL_SetClipboardData(&NativeCallback, &NativeCleanup, pin.Pointer, mimeTypes);
    }
    
    /// <summary>
    /// Query whether there is data in the clipboard for the provided mime type
    /// </summary>
    /// <param name="mimeType">the mime type to check for data</param>
    /// <returns>true if data exists in the clipboard for the provided mime type, false if it does not</returns>
    public static bool HasData(string mimeType) => SDL_HasClipboardData(mimeType);
    
    /// <summary>
    /// Text from the clipboard
    /// </summary>
    /// <remarks>
    /// This returns an empty string if there is not enough memory left for a copy of the clipboard's content.
    /// </remarks>
    public static string? Text {
        get => SDL_GetClipboardText();
        set => SDL_SetClipboardText(value);
    }
    
    /// <summary>
    /// Whether the clipboard exists and contains a non-empty text string
    /// </summary>
    public static bool HasText => SDL_HasClipboardText();

    /// <summary>
    /// Text from the primary selection
    /// </summary>
    /// <remarks>This returns an empty string if there is not enough memory left for a copy of the primary selection's content.</remarks>
    public static string? PrimarySelectionText {
        get => SDL_GetPrimarySelectionText();
        set => SDL_SetPrimarySelectionText(value);
    }

    /// <summary>
    /// whether the primary selection exists and contains a non-empty text string
    /// </summary>
    public static bool HasPrimarySelectionText => SDL_HasPrimarySelectionText();
}