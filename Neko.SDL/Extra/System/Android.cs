using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Neko.Sdl.Extra.System;

[SupportedOSPlatform("Android")]
public static class Android {
    /// <summary>
    /// The Java instance of the Android activity class
    /// </summary>
    /// <returns>
    /// The jobject representing the instance of the Activity class of the Android application
    /// </returns>
    /// <remarks>
    /// <para>
    /// The prototype of the function in SDL's code actually declares a IntPtr return type,
    /// even if the implementation returns a jobject. The rationale being that the SDL can avoid referencing jni.
    /// </para>
    /// <para>
    /// The jobject returned by the function is a local reference and must be released by the caller.
    /// See the PushLocalFrame() and PopLocalFrame() or DeleteLocalRef() functions of the Java native interface:
    /// </para>
    /// <para>
    /// https://docs.oracle.com/javase/1.5.0/docs/guide/jni/spec/functions.html
    /// </para>
    /// </remarks>
    public static IntPtr GetActivity() {
        var result = SDL_GetAndroidActivity();
        if (result == IntPtr.Zero)
            throw new SdlException();
        return result;
    }
    
    public static void ShowToast(string message, int duration, int gravity, int xoffset, int yoffset) => 
        SDL_ShowAndroidToast(message, duration, gravity, xoffset, yoffset).ThrowIfError();

    /// <summary>
    /// Get the path used for caching data for this Android application
    /// </summary>
    /// <remarks>
    /// <para>
    /// This path is unique to your application, but is public and can be written to by other applications.
    /// </para>
    /// <para>
    /// Your cache path is typically: /data/data/your.app.package/cache/.
    /// </para>
    /// <para>
    /// This is a C# wrapper over android.content.Context.getCacheDir():
    /// </para>
    /// <para>
    /// https://developer.android.com/reference/android/content/Context#getCacheDir()
    /// </para>
    /// </remarks>
    public static string CachePath {
        get {
            var ptr = SDL_GetAndroidCachePath();
            if (ptr is null) throw new SdlException("");
            return ptr;
        }
    }
    
    public static int SdkVersion {
        get {
            var ptr = SDL_GetAndroidSDKVersion();
            if (ptr is 0) throw new SdlException("");
            return ptr;
        }
    }
    
    /// <summary>
    /// Get the path used for internal storage for this Android application
    /// </summary>
    /// <remarks>
    /// <para>
    /// This path is unique to your application and cannot be written to by other applications.
    /// </para>
    /// <para>
    /// Your internal storage path is typically: /data/data/your.app.package/files.
    /// </para>
    /// <para>
    /// This is a C wrapper over android.content.Context.getFilesDir():
    /// </para>
    /// <para>
    /// https://developer.android.com/reference/android/content/Context#getFilesDir()
    /// </para>
    /// </remarks>
    public static string InternalStoragePath {
        get {
            var ptr = SDL_GetAndroidInternalStoragePath();
            if (ptr is null) throw new SdlException("");
            return ptr;
        }
    }
    
    /// <summary>
    /// Get the path used for external storage for this Android application
    /// </summary>
    /// <remarks>
    /// <para>
    /// This path is unique to your application, but is public and can be written to by other applications.
    /// </para>
    /// <para>
    /// Your external storage path is typically: /storage/sdcard0/Android/data/your.app.package/files.
    /// </para>
    /// <para>
    /// This is a C wrapper over android.content.Context.getExternalFilesDir():
    /// </para>
    /// <para>
    /// https://developer.android.com/reference/android/content/Context#getExternalFilesDir()
    /// </para>
    /// </remarks>
    public static string ExternalStoragePath {
        get {
            var ptr = SDL_GetAndroidExternalStoragePath();
            if (ptr is null) throw new SdlException("");
            return ptr;
        }
    }

    /// <summary>
    /// The current state of external storage for this Android application
    /// </summary>
    /// <remarks>
    /// <para>
    /// The current state of external storage, a bitmask of these values: <see cref="StorageStateRead"/>, <see cref="StorageStateWrite"/>.
    /// </para>
    /// <para>
    /// If external storage is currently unavailable, this will return 0.
    /// </para>
    /// </remarks>
    public static uint ExternalStorageState => SDL_GetAndroidExternalStorageState();

    public const uint StorageStateRead = SDL_ANDROID_EXTERNAL_STORAGE_READ;
    public const uint StorageStateWrite = SDL_ANDROID_EXTERNAL_STORAGE_WRITE;
    
    

    public delegate void RequestPermissionCallback(string permission, bool granted);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void NativeCallback(IntPtr userdata, byte* permission, SDLBool granted) {
        using var pin = userdata.AsPin<RequestPermissionCallback>();
        var managedCallback = pin.Target;
        var permissionString = Marshal.PtrToStringUTF8((IntPtr)permission);
        managedCallback(permissionString, granted);
    }
    
    public static unsafe void RequestPermission(string permission, RequestPermissionCallback cb) {
        var pin = cb.Pin(GCHandleType.Normal);
        SDL_RequestAndroidPermission(permission, &NativeCallback, pin.Addr).ThrowIfError();
    }

    public static void SendBackButton() => SDL_SendAndroidBackButton();
    public static void SendMessage(uint command, int param) => SDL_SendAndroidMessage(command, param).ThrowIfError();
    
    public static bool IsChromebook => SDL_IsChromebook();
    public static bool IsDeXMode => SDL_IsDeXMode();
}