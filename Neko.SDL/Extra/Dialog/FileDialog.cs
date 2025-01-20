using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Video;

namespace Neko.Sdl.Extra.Dialog;

public static unsafe class FileDialog {
    public delegate void FileCallback(List<string>? filelist, int selectedFilterIndex);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void NativeCallback(IntPtr userdata, byte** filelist, int filter) {
        var managedCallbackPin = new Pin<FileCallback>(userdata, true);
        var managedCallback = managedCallbackPin.Target;
        List<string>? strings = null;
        if (filelist is not null) {
            strings = new List<string>();
            while (filelist is not null) {
                var str = Marshal.PtrToStringUTF8((IntPtr)filelist);
                if (str is not null)
                    strings.Add(str);
                filelist++;
            }
        }
        
        managedCallback(strings, filter);
        
        managedCallbackPin.Dispose();
    }

    public static void ShowFileOpen(FileCallback callback,
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
                var filter = new SDL_DialogFileFilter {
                    name = (byte*)Marshal.StringToHGlobalAnsi(name),
                    pattern = (byte*)Marshal.StringToHGlobalAnsi(pattern),
                };
                filtersArray[counter++] = filter;
            }
            fixed(SDL_DialogFileFilter* filter = filtersArray)
                SDL_ShowOpenFileDialog(nativeCallback, callbackPtr, windowPtr, filter, filtersArray.Length, defaultLocation, multiple);
            return;
        }
        SDL_ShowOpenFileDialog(nativeCallback, callbackPtr, windowPtr, null, 0, defaultLocation, multiple);
    }
    
    public static void ShowFileSave(FileCallback callback,
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
                var filter = new SDL_DialogFileFilter {
                    name = (byte*)Marshal.StringToHGlobalAnsi(name),
                    pattern = (byte*)Marshal.StringToHGlobalAnsi(pattern),
                };
                filtersArray[counter++] = filter;
            }
            fixed(SDL_DialogFileFilter* filter = filtersArray)
                SDL_ShowSaveFileDialog(nativeCallback, callbackPtr, windowPtr, filter, filtersArray.Length, defaultLocation);
            return;
        }
        SDL_ShowSaveFileDialog(nativeCallback, callbackPtr, windowPtr, null, 0, defaultLocation);
    }
    
    public static void ShowFolderOpen(FileCallback callback,
        Window? window = null,
        string? defaultLocation = null,
        bool multiple = false) {
        SDL_Window* windowPtr = null;
        var callbackPtr = callback.Pin(GCHandleType.Normal).Pointer;
        delegate*unmanaged[Cdecl]<IntPtr, byte**, int, void> nativeCallback = &NativeCallback;
        if (window is not null) windowPtr = window;
        SDL_ShowOpenFolderDialog(nativeCallback, callbackPtr, windowPtr, defaultLocation, multiple);
    }
    
    public static void ShowFileOpenWithProperties(FileDialogType type,
        FileCallback callback,
        Properties properties) {
        var callbackPtr = callback.Pin(GCHandleType.Normal).Pointer;
        delegate*unmanaged[Cdecl]<IntPtr, byte**, int, void> nativeCallback = &NativeCallback;
        
        SDL_ShowFileDialogWithProperties((SDL_FileDialogType)type, nativeCallback, callbackPtr, properties);
    }
}