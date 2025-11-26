using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.Filesystem;

/// <summary>
/// The storage API is a high-level API designed to abstract away the portability issues that come up when using something
/// lower-level (in SDL's case, this sits on top of the Filesystem and IOStream subsystems). 
/// </summary>
public unsafe partial class Storage : SdlWrapper<SDL_Storage> {
    private static SDL_StorageInterface _interface;
    private Pin<IStorage>? _managedPin;

    #region Native Interop
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
            info = (SDL_PathInfo*)UnmanagedMemory.Malloc((nuint)sizeof(SDL_PathInfo));
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
    #endregion

    /// <summary>
    /// Opens up a container for local filesystem storage
    /// </summary>
    /// <param name="storage"></param>
    /// <returns></returns>
    /// <remarks>
    /// This is provided for development and tools.
    /// Portable applications should use <see cref="OpenTitle"/> for access to game data and
    /// <see cref="OpenUser"/> for access to user data.
    /// </remarks>
    public static Storage OpenFileStorage(string path) {
        var storage = SDL_OpenFileStorage(path);
        if (storage is null)
            throw new SdlException();
        return storage;
    }
    
    /// <summary>
    /// Opens up a container using a client-provided storage interface.
    /// </summary>
    /// <param name="storage">the object that implements this storage</param>
    /// <returns>a storage container</returns>
    /// <remarks>
    /// <para>
    /// Applications do not need to use this function unless they are providing their own Storage
    /// implementation. If you just need a Storage, you should use the built-in implementations
    /// in SDL, like <see cref="OpenTitle"/> or <see cref="OpenUser"/>.
    /// </para>
    /// </remarks>
    public static Storage Open(IStorage storage) {
        var pin = storage.Pin(GCHandleType.Normal);
        fixed (SDL_StorageInterface* iface = &_interface) {
            var storagePtr = SDL_OpenStorage(iface, pin.Pointer);
            if (storagePtr is null) {
                pin.Dispose();
                throw new SdlException();
            }
            var storageImpl = new Storage(storagePtr);
            storageImpl._managedPin = pin;
            return storageImpl;
        }
    }

    /// <summary>
    /// Opens up a read-only container for the application's filesystem
    /// </summary>
    /// <param name="override">a path to override the backend's default title root</param>
    /// <param name="properties">a property list that may contain backend-specific information</param>
    /// <returns>a title storage container</returns>
    public static Storage OpenTitle(string @override, Properties properties) {
        var storage = SDL_OpenTitleStorage(@override, properties);
        if (storage is null)
            throw new SdlException();
        return storage;
    }

    /// <summary>
    /// Opens up a container for a user's unique read/write filesystem
    /// </summary>
    /// <param name="org">the name of your organization</param>
    /// <param name="app">the name of your application</param>
    /// <param name="properties">a property list that may contain backend-specific information</param>
    /// <returns>a user storage container</returns>
    /// <remarks>
    /// While title storage can generally be kept open throughout runtime, user storage should
    /// only be opened when the client is ready to read/write files. This allows the backend
    /// to properly batch file operations and flush them when the container has been closed;
    /// ensuring safe and optimal save I/O.
    /// </remarks>
    public static Storage OpenUser(string org, string app, Properties properties) {
        var storage = SDL_OpenUserStorage(org, app, properties);
        if (storage is null)
            throw new SdlException();
        return storage;
    }

    /// <summary>
    /// Closes and frees a storage container.
    /// </summary>
    /// <remarks>
    /// Even if the function throws, the container data will be freed; the error is only for informational purposes.
    /// </remarks>
    public override void Dispose() {
        base.Dispose();
        SDL_CloseStorage(this).ThrowIfError();
        _managedPin?.Dispose();
    }

    /// <summary>
    /// Checks if the storage container is ready to use
    /// </summary>
    /// <remarks>
    /// This property should be checked in regular intervals until it returns true - however,
    /// it is not recommended to spinwait on this call, as the backend may depend on a synchronous message loop.
    /// You might instead poll this in your game's main loop while processing events and drawing a loading screen.
    /// </remarks>
    public bool IsReady => SDL_StorageReady(this);

    
    public void EnumerateDirectory(string path) {
        throw new NotImplementedException();
    }

    public PathInfo Info(string path) {
        var info = new PathInfo();
        SDL_GetStoragePathInfo(this, path, (SDL_PathInfo*)&info).ThrowIfError();
        return info;
    }

    /// <summary>
    /// Synchronously read a file from a storage container into a client-provided buffer
    /// </summary>
    /// <param name="path">the relative path of the file to read</param>
    /// <param name="destination">a client-provided buffer to read the file into</param>
    /// <param name="length">the length of the destination buffer</param>
    public void ReadFile(string path, Span<byte> destination, ulong length) {
        fixed (byte* dstPtr = destination)
            SDL_ReadStorageFile(this, path, (IntPtr)dstPtr, length).ThrowIfError();
    }
    
    /// <summary>
    /// Synchronously read a file from a storage container into a client-provided buffer
    /// </summary>
    /// <param name="path">the relative path of the file to read</param>
    /// <param name="destination">a client-provided buffer to read the file into</param>
    /// <remarks>
    /// The value of length must match the length of the file exactly;
    /// call <see cref="GetFileSize"/> to get this value. This behavior may be relaxed in a future release.
    /// </remarks>
    public void ReadFile(string path, Span<byte> destination) => ReadFile(path, destination, (ulong)destination.Length);

    /// <summary>
    /// Synchronously read a file from a storage container into a client-provided buffer
    /// </summary>
    /// <param name="path">the relative path of the file to read</param>
    /// <returns>file contents</returns>
    public byte[] ReadFile(string path) {
        var file = new byte[GetFileSize(path)];
        ReadFile(path, file);
        return file;
    }

    /// <summary>
    /// Query the size of a file within a storage container
    /// </summary>
    /// <param name="path">the relative path of the file to query</param>
    /// <returns>the file's length</returns>
    public ulong GetFileSize(string path) {
        ulong nya;
        SDL_GetStorageFileSize(this, path, &nya).ThrowIfError();
        return nya;
    }
    
    /// <summary>
    /// Get information about a filesystem path in a storage container
    /// </summary>
    /// <param name="path">the path to query</param>
    /// <returns>a pointer filled in with information about the path, or NULL to check for the existence of a file</returns>
    public PathInfo GetPathInfo(string path) {
        PathInfo nya;
        SDL_GetStoragePathInfo(this, path, (SDL_PathInfo*)&nya).ThrowIfError();
        return nya;
    }

    public void WriteFile(string path, Span<byte> source, ulong length) {
        fixed (byte* dstPtr = source)
            SDL_WriteStorageFile(this, path, (IntPtr)dstPtr, length).ThrowIfError();
    }
    public void WriteFile(string path, Span<byte> source) => WriteFile(path, source, (ulong)source.Length);


    /// <summary>
    /// Create a directory in a writable storage container
    /// </summary>
    /// <param name="path">the path of the directory to create</param>
    public void CreateDirectory(string path) =>
        SDL_CreateStorageDirectory(this, path).ThrowIfError();
    
    /// <summary>
    /// Remove a file or an empty directory in a writable storage container
    /// </summary>
    /// <param name="path">the path of the directory to enumerate</param>
    public void RemovePath(string path) =>
        SDL_RemoveStoragePath(this, path).ThrowIfError();
    
    /// <summary>
    /// Rename a file or directory in a writable storage container
    /// </summary>
    /// <param name="oldpath">the old path</param>
    /// <param name="newpath">the new path</param>
    public void RenamePath(string oldpath, string newpath) =>
        SDL_RenameStoragePath(this, oldpath, newpath).ThrowIfError();
    
    /// <summary>
    /// Copy a file in a writable storage container
    /// </summary>
    /// <param name="oldpath">the old path</param>
    /// <param name="newpath">the new path</param>
    public void Copy(string oldpath, string newpath) =>
        SDL_CopyStorageFile(this, oldpath, newpath).ThrowIfError();

    /// <summary>
    /// The remaining space in a storage container in bytes
    /// </summary>
    public ulong SpaceRemaining => SDL_GetStorageSpaceRemaining(this);

    public void GlobDirectory() => throw new NotImplementedException();
}