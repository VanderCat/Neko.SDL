using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Filesystem;

public unsafe partial class Storage : SdlWrapper<SDL_Storage> {
    private static SDL_StorageInterface _interface;
    private static Pin<SDL_StorageInterface> _pin;
    private Pin<IStorage>? _managedPin;
        
    static Storage() {
        _interface = new();
        _interface.version = (uint)Marshal.SizeOf<SDL_StorageInterface>();
        _interface.close = &NativeClose;
        _interface.ready = &NativeReady;
        _interface.enumerate = &NativeEnumerate;
        _interface.info = &NativeInfo;
        _interface.read_file = &NativeReadFile;
        _interface.write_file = &NativeWriteFile;
        _interface.mkdir = &NativeMkdir;
        _interface.remove = &NativeMkdir;
        _interface.rename = &NativeRename;
        _interface.copy = &NativeCopy;
        _interface.space_remaining = &NativeSpaceRemaining;
        _pin = _interface.Pin();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeClose(IntPtr userdata) {
        try {
            userdata.AsPin<IStorage>().Target.Close();
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeReady(IntPtr userdata) {
        try {
            userdata.AsPin<IStorage>().Target.Ready();
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeEnumerate(IntPtr userdata, byte* path, delegate*unmanaged[Cdecl]<IntPtr, byte*, byte*, SDL_EnumerationResult> callback,
        IntPtr callbackUserdata) {
        var pathStr = Marshal.PtrToStringUTF8((IntPtr)path);
        if (pathStr is null) return false;
        try {
            throw new NotImplementedException();
            //userdata.AsPin<IStorage>().Target.Enumerate(pathStr,);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeInfo(IntPtr userdata, byte* path, SDL_PathInfo* info) {
        var pathStr = Marshal.PtrToStringUTF8((IntPtr)path);
        if (pathStr is null) return false;
        try {
            var managedInfo = userdata.AsPin<IStorage>().Target.Info(pathStr);
            info = (SDL_PathInfo*)Marshal.AllocHGlobal(Marshal.SizeOf<SDL_PathInfo>());
            Unsafe.Copy(ref managedInfo, info);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeReadFile(IntPtr userdata, byte* path, IntPtr destination, ulong length) {
        var pathStr = Marshal.PtrToStringUTF8((IntPtr)path);
        if (pathStr is null) return false;
        try {
            var span = new Span<byte>((void*)destination, (int)length); //FIXME: 32bit only
            userdata.AsPin<IStorage>().Target.ReadFile(pathStr, ref span, length);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeWriteFile(IntPtr userdata, byte* path, IntPtr source, ulong length) {
        var pathStr = Marshal.PtrToStringUTF8((IntPtr)path);
        if (pathStr is null) return false;
        try {
            var span = new Span<byte>((void*)source, (int)length); //FIXME: 32bit only
            userdata.AsPin<IStorage>().Target.WriteFile(pathStr, ref span, length);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeMkdir(IntPtr userdata, byte* path) {
        var pathStr = Marshal.PtrToStringUTF8((IntPtr)path);
        if (pathStr is null) return false;
        try {
            userdata.AsPin<IStorage>().Target.Mkdir(pathStr);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeRemove(IntPtr userdata, byte* path) {
        var pathStr = Marshal.PtrToStringUTF8((IntPtr)path);
        if (pathStr is null) return false;
        try {
            userdata.AsPin<IStorage>().Target.Remove(pathStr);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeRename(IntPtr userdata, byte* oldpath, byte* newpath) {
        var oldpathStr = Marshal.PtrToStringUTF8((IntPtr)oldpath);
        var newpathStr = Marshal.PtrToStringUTF8((IntPtr)newpath);
        if (oldpathStr is null || newpathStr is null) return false;
        try {
            userdata.AsPin<IStorage>().Target.Rename(oldpathStr, newpathStr);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool NativeCopy(IntPtr userdata, byte* oldpath, byte* newpath) {
        var oldpathStr = Marshal.PtrToStringUTF8((IntPtr)oldpath);
        var newpathStr = Marshal.PtrToStringUTF8((IntPtr)newpath);
        if (oldpathStr is null || newpathStr is null) return false;
        try {
            userdata.AsPin<IStorage>().Target.Copy(oldpathStr, newpathStr);
        }
        catch (Exception e) {
            //TODO: errorlog
            return false;
        }
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static ulong NativeSpaceRemaining(IntPtr userdata) {
        return userdata.AsPin<IStorage>().Target.SpaceRemaining();
    }
    
    public static Storage Open(IStorage storage) {
        var pin = storage.Pin(GCHandleType.Normal);
        var storageImpl = new Storage(SDL_OpenStorage((SDL_StorageInterface*)_pin.Addr, pin.Pointer));
        storageImpl._managedPin = pin;
        return storageImpl;
    }

    public static Storage OpenTitle(string @override, Properties properties) {
        return SDL_OpenTitleStorage(@override, properties);
    }
    
    public static Storage OpenUserStorage(string org, string app, Properties properties) {
        return SDL_OpenUserStorage(org, app, properties);
    }

    public override void Dispose() {
        base.Dispose();
        SDL_CloseStorage(this);
        _managedPin?.Dispose();
    }

    public bool IsReady => SDL_StorageReady(this);

    public void EnumerateDirectory(string path) {
        throw new NotImplementedException();
    }

    public PathInfo Info(string path) {
        var info = new PathInfo();
        SDL_GetStoragePathInfo(this, path, (SDL_PathInfo*)&info).ThrowIfError();
        return info;
    }

    public void ReadFile(string path, ref Span<byte> destination, ulong length) {
        fixed (byte* dstPtr = destination)
            SDL_ReadStorageFile(this, path, (IntPtr)dstPtr, length).ThrowIfError();
    }
    
    public void WriteFile(string path, ref Span<byte> source, ulong length) {
        fixed (byte* dstPtr = source)
            SDL_WriteStorageFile(this, path, (IntPtr)dstPtr, length).ThrowIfError();
    }

    public void CreateDirectory(string path) =>
        SDL_CreateStorageDirectory(this, path).ThrowIfError();
    
    public void RemovePath(string path) =>
        SDL_RemoveStoragePath(this, path).ThrowIfError();
    
    public void RenamePath(string oldpath, string newpath) =>
        SDL_RenameStoragePath(this, oldpath, newpath).ThrowIfError();
    
    public void Copy(string oldpath, string newpath) =>
        SDL_CopyStorageFile(this, oldpath, newpath).ThrowIfError();

    public ulong SpaceRemaining() => SDL_GetStorageSpaceRemaining(this);

    public void GlobDirectory() => throw new NotImplementedException();
}