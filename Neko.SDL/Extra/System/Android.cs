using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Neko.Sdl.Extra.System;

[SupportedOSPlatform("Android")]
public static class Android {
    public static void ShowToast(string message, int duration, int gravity, int xoffset, int yoffset) => 
        SDL_ShowAndroidToast(message, duration, gravity, xoffset, yoffset).ThrowIfError();

    public static string GetCachePath() {
        var ptr = SDL_GetAndroidCachePath();
        if (ptr is null) throw new SdlException("");
        return ptr;
    }
    
    public static int GetSdkVersion() {
        var ptr = SDL_GetAndroidSDKVersion();
        if (ptr is 0) throw new SdlException("");
        return ptr;
    }
    
    public static string GetInternalStoragePath() {
        var ptr = SDL_GetAndroidInternalStoragePath();
        if (ptr is null) throw new SdlException("");
        return ptr;
    }
    
    public static string GetExternalStoragePath() {
        var ptr = SDL_GetAndroidExternalStoragePath();
        if (ptr is null) throw new SdlException("");
        return ptr;
    }

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
    
    public static bool IsChromebook() => SDL_IsChromebook();
    public static bool IsDeXMode() => SDL_IsDeXMode();
}