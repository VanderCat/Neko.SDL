using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.Extra.StandardLibrary;
using Neko.Sdl.Video;

namespace Neko.Sdl.Extra.Dialog;

/// <summary>
/// File dialog support.
/// <br/><br/>
/// SDL offers file dialogs, to let users select files with native GUI interfaces. There are "open" dialogs, "save"
/// dialogs, and folder selection dialogs. The app can control some details, such as filtering to specific files, or
/// whether multiple files can be selected by the user.
/// <br/><br/>
/// Note that launching a file dialog is a non-blocking operation; control returns to the app immediately, and a
/// callback is called later (possibly in another thread) when the user makes a choice.
/// </summary>
public static unsafe class FileDialog {
    public delegate void FileCallback(List<string>? filelist, int selectedFilterIndex);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void NativeCallback(IntPtr userdata, byte** filelist, int filter) {
        using var managedCallbackPin = new Pin<FileCallback>(userdata, true);
        var managedCallback = managedCallbackPin.Target;
        List<string>? strings = null;
        if (filelist is not null) {
            strings = new List<string>();
            while (*filelist is not null) {
                var str = Marshal.PtrToStringUTF8((IntPtr)(*filelist));
                if (str is not null)
                    strings.Add(str);
                filelist++;
            }
        }
        
        managedCallback(strings, filter);
    }

    /// <summary>
    /// Displays a dialog that lets the user select a file on their filesystem
    /// </summary>
    /// <param name="callback">
    /// a function delegate to be invoked when the user selects a file and accepts, or cancels the dialog, or an error
    /// occurs
    /// </param>
    /// <param name="window">
    /// the window that the dialog should be modal for, may be NULL. Not all platforms support this option
    /// </param>
    /// <param name="filters">
    /// a list of filters, may be NULL. See the SDL_DialogFileFilter documentation for examples. Not all platforms
    /// support this option, and platforms that do support it may allow the user to ignore the filters. If non-NULL, it
    /// must remain valid at least until the callback is invoked.
    /// </param>
    /// <param name="defaultLocation">
    /// the default folder or file to start the dialog at, may be NULL. Not all platforms support this option
    /// </param>
    /// <param name="multiple">
    /// if true, the user will be allowed to select multiple entries. Not all platforms support this option
    /// </param>
    /// <remarks>
    /// This is an asynchronous function; it will return immediately, and the result will be passed to the callback.
    /// <br/><br/>
    /// The callback will be invoked with a list of files the user chose. The list will be empty if the
    /// user canceled the dialog, and it will be null if an error occurred.
    /// <br/><br/>
    /// Note that the callback may be called from a different thread than the one the function was invoked on.
    /// <br/><br/>
    /// Depending on the platform, the user may be allowed to input paths that don't yet exist.
    /// <br/><br/>
    /// On Linux, dialogs may require XDG Portals, which requires DBus, which requires an event-handling loop. Apps that
    /// do not use SDL to handle events should add a call to <see cref="SDL_PumpEvents"/> in their main loop.
    /// </remarks>
    public static void ShowOpen(FileCallback callback,
        Window? window = null,
        Dictionary<string, string>? filters = null,
        string? defaultLocation = null,
        bool multiple = false) {
        SDL_Window* windowPtr = null;
        SDL_DialogFileFilter[] filtersArray;
        var callbackPtr = callback.Pin(GCHandleType.Normal).Pointer;
        delegate*unmanaged[Cdecl]<IntPtr, byte**, int, void> nativeCallback = &NativeCallback;
        if (window is not null) windowPtr = window;
        if (filters is not null) {
            var counter = 0;
            filtersArray = new SDL_DialogFileFilter[filters.Count];
            foreach (var (name, pattern) in filters) {
                var nameBuf = Encoding.UTF8.GetBytes(name);
                var patternBuf = Encoding.UTF8.GetBytes(pattern);
                var filter = new SDL_DialogFileFilter {
                    name = (byte*)UnmanagedMemory.Malloc((nuint)nameBuf.Length),
                    pattern = (byte*)UnmanagedMemory.Malloc((nuint)patternBuf.Length),
                };
                UnmanagedMemory.Copy(nameBuf, new Span<byte>(filter.name, nameBuf.Length));
                UnmanagedMemory.Copy(patternBuf, new Span<byte>(filter.pattern, patternBuf.Length));
                filtersArray[counter++] = filter;
            }
            fixed(SDL_DialogFileFilter* filter = filtersArray)
                SDL_ShowOpenFileDialog(nativeCallback, callbackPtr, windowPtr, filter, filtersArray.Length, defaultLocation, multiple);
            foreach (var filter in filtersArray) {
                UnmanagedMemory.Free(filter.name);
                UnmanagedMemory.Free(filter.pattern);
            }
            return;
        }
        SDL_ShowOpenFileDialog(nativeCallback, callbackPtr, windowPtr, null, 0, defaultLocation, multiple);
    }
    
    /// <summary>
    /// Displays a dialog that lets the user choose a new or existing file on their filesystem
    /// </summary>
    /// <param name="callback">
    /// a function delegate to be invoked when the user selects a file and accepts, or cancels the dialog, or an error
    /// occurs
    /// </param>
    /// <param name="window">
    /// the window that the dialog should be modal for, may be NULL. Not all platforms support this option
    /// </param>
    /// <param name="filters">
    /// a list of filters, may be NULL. Not all platforms support this option, and platforms that do support it may
    /// allow the user to ignore the filters. If non-NULL, it must remain valid at least until the callback is invoked.
    /// </param>
    /// <param name="defaultLocation">
    /// the default folder or file to start the dialog at, may be NULL. Not all platforms support this option
    /// </param>
    /// <remarks>
    /// This is an asynchronous function; it will return immediately, and the result will be passed to the callback.
    /// <br/><br/>
    /// The callback will be invoked with a list of files the user chose. The list will be empty if the
    /// user canceled the dialog, and it will be null if an error occurred.
    /// <br/><br/>
    /// Note that the callback may be called from a different thread than the one the function was invoked on.
    /// <br/><br/>
    /// Depending on the platform, the user may be allowed to input paths that don't yet exist.
    /// <br/><br/>
    /// On Linux, dialogs may require XDG Portals, which requires DBus, which requires an event-handling loop. Apps that
    /// do not use SDL to handle events should add a call to <see cref="SDL_PumpEvents"/> in their main loop.
    /// </remarks>
    public static void ShowSave(FileCallback callback,
        Window? window = null,
        Dictionary<string, string>? filters = null,
        string? defaultLocation = null) {
        SDL_Window* windowPtr = null;
        SDL_DialogFileFilter[] filtersArray;
        var callbackPtr = callback.Pin(GCHandleType.Normal).Pointer;
        delegate*unmanaged[Cdecl]<IntPtr, byte**, int, void> nativeCallback = &NativeCallback;
        if (window is not null) windowPtr = window;
        if (filters is not null) {
            var counter = 0;
            filtersArray = new SDL_DialogFileFilter[filters.Count];
            foreach (var (name, pattern) in filters) {
                var nameBuf = Encoding.UTF8.GetBytes(name);
                var patternBuf = Encoding.UTF8.GetBytes(pattern);
                var filter = new SDL_DialogFileFilter {
                    name = (byte*)UnmanagedMemory.Malloc((nuint)nameBuf.Length),
                    pattern = (byte*)UnmanagedMemory.Malloc((nuint)patternBuf.Length),
                };
                UnmanagedMemory.Copy(nameBuf, new Span<byte>(filter.name, nameBuf.Length));
                UnmanagedMemory.Copy(patternBuf, new Span<byte>(filter.pattern, patternBuf.Length));
                filtersArray[counter++] = filter;
            }
            fixed(SDL_DialogFileFilter* filter = filtersArray)
                SDL_ShowSaveFileDialog(nativeCallback, callbackPtr, windowPtr, filter, filtersArray.Length, defaultLocation);
            foreach (var filter in filtersArray) {
                UnmanagedMemory.Free(filter.name);
                UnmanagedMemory.Free(filter.pattern);
            }
            return;
        }
        SDL_ShowSaveFileDialog(nativeCallback, callbackPtr, windowPtr, null, 0, defaultLocation);
    }
    
    /// <summary>
    /// Displays a dialog that lets the user select a folder on their filesystem
    /// </summary>
    /// <param name="callback">
    /// a function pointer to be invoked when the user selects a file and accepts, or cancels the dialog, or an error
    /// occurs
    /// </param>
    /// <param name="window">
    /// the window that the dialog should be modal for, may be NULL. Not all platforms support this option.
    /// </param>
    /// <param name="defaultLocation">
    /// the default folder or file to start the dialog at, may be NULL. Not all platforms support this option.
    /// </param>
    /// <param name="multiple">
    /// if non-zero, the user will be allowed to select multiple entries. Not all platforms support this option
    /// </param>
    /// <remarks>
    /// This is an asynchronous function; it will return immediately, and the result will be passed to the callback.
    /// <br/><br/>
    /// The callback will be invoked with a list of files the user chose. The list will be empty if the
    /// user canceled the dialog, and it will be null if an error occurred.
    /// <br/><br/>
    /// Note that the callback may be called from a different thread than the one the function was invoked on.
    /// <br/><br/>
    /// Depending on the platform, the user may be allowed to input paths that don't yet exist.
    /// <br/><br/>
    /// On Linux, dialogs may require XDG Portals, which requires DBus, which requires an event-handling loop. Apps that
    /// do not use SDL to handle events should add a call to <see cref="SDL_PumpEvents"/> in their main loop.
    /// </remarks>
    public static void ShowOpenFolder(FileCallback callback,
        Window? window = null,
        string? defaultLocation = null,
        bool multiple = false) {
        SDL_Window* windowPtr = null;
        var callbackPtr = callback.Pin(GCHandleType.Normal).Pointer;
        delegate*unmanaged[Cdecl]<IntPtr, byte**, int, void> nativeCallback = &NativeCallback;
        if (window is not null) windowPtr = window;
        SDL_ShowOpenFolderDialog(nativeCallback, callbackPtr, windowPtr, defaultLocation, multiple);
    }
    
    /// <summary>
    /// Create and launch a file dialog with the specified properties
    /// </summary>
    /// <param name="type">the type of file dialog</param>
    /// <param name="callback">
    /// a function pointer to be invoked when the user selects a file and accepts, or cancels the dialog, or an
    /// error occurs
    /// </param>
    /// <param name="properties">the properties to use</param>
    /// <remarks>
    /// These are the supported properties:
    /// <ul>
    /// <li>
    /// SDL_PROP_FILE_DIALOG_FILTERS_POINTER: a pointer to a list of SDL_DialogFileFilter structs, which will be
    /// used as filters for file-based selections. Ignored if the dialog is an "Open Folder" dialog. If non-NULL, the
    /// array of filters must remain valid at least until the callback is invoked.</li>
    /// <li>SDL_PROP_FILE_DIALOG_NFILTERS_NUMBER: the number of filters in the array of filters, if it exists.</li>
    /// <li>SDL_PROP_FILE_DIALOG_WINDOW_POINTER: the window that the dialog should be modal for.</li>
    /// <li>SDL_PROP_FILE_DIALOG_LOCATION_STRING: the default folder or file to start the dialog at.</li>
    /// <li>SDL_PROP_FILE_DIALOG_MANY_BOOLEAN: true to allow the user to select more than one entry.</li>
    /// <li>SDL_PROP_FILE_DIALOG_TITLE_STRING: the title for the dialog.</li>
    /// <li>SDL_PROP_FILE_DIALOG_ACCEPT_STRING: the label that the accept button should have.</li>
    /// <li>SDL_PROP_FILE_DIALOG_CANCEL_STRING: the label that the cancel button should have.</li>
    /// </ul>
    /// Note that each platform may or may not support any of the properties.
    /// </remarks>
    public static void Show(FileDialogType type,
        FileCallback callback,
        Properties properties) {
        var callbackPtr = callback.Pin(GCHandleType.Normal).Pointer;
        delegate*unmanaged[Cdecl]<IntPtr, byte**, int, void> nativeCallback = &NativeCallback;
        
        SDL_ShowFileDialogWithProperties((SDL_FileDialogType)type, nativeCallback, callbackPtr, properties);
    }
}