namespace Neko.Sdl.Extra;

/// <summary>
/// System-dependent library loading routines.
/// <br/><br/>
/// Shared objects are code that is programmatically loadable at runtime. Windows calls these "DLLs", Linux calls them "shared libraries", etc.
/// <br/><br/>
/// To use them, build such a library, then call <see cref="Load"/> on it.
/// Once loaded, you can use <see cref="LoadFunction"/> on that object to find the address of its exported symbols.
/// When done with the object, call <see cref="Dispose"/> to dispose of it.
/// <br/><br/>
/// Some things to keep in mind:
/// <ul>
///     <li>
///     These functions only work on C function names.
///     Other languages may have name mangling and intrinsic language support that varies from compiler to compiler.
///     </li>
///     <li>
///     Make sure you declare your function pointers with the same calling convention as the actual library function.
///     Your code will crash mysteriously if you do not do this.
///     </li>
///     <li>
///     Avoid namespace collisions. If you load a symbol from the library, it is not defined whether or not
///     it goes into the global symbol namespace for the application. If it does and it conflicts with symbols
///     in your code or other shared libraries, you will not get the results you expect. :)
///     </li>
///     <li>
///     Once a library is unloaded, all pointers into it obtained through <see cref="LoadFunction"/> become invalid,
///     even if the library is later reloaded. Don't unload a library if you plan to use these pointers in the future.
///     Notably: beware of giving one of these pointers to atexit(), since it may call that pointer after the library unloads.
///     </li>
/// </ul>
/// </summary>
public unsafe partial class NativeSharedObject : SdlWrapper<SDL_SharedObject> {
    /// <summary>
    /// Dynamically load a shared object
    /// </summary>
    /// <param name="path">a system-dependent name of the object file</param>
    /// <returns></returns>
    /// <exception cref="SdlException"></exception>
    public static NativeSharedObject Load(string path) {
        var ptr = SDL_LoadObject(path);
        if (ptr is null) throw new SdlException("Failed to load Shared Object: ");
        return ptr;
    }

    /// <summary>
    /// Look up the address of the named function in a shared object
    /// </summary>
    /// <param name="name">the name of the function to look up</param>
    /// <returns></returns>
    /// <exception cref="SdlException">Function does not exist</exception>
    /// <remarks>
    /// This function pointer is no longer valid after calling <see cref="Dispose"/>.
    /// <br/><br/>
    /// This function can only look up C function names. Other languages may have name mangling and
    /// intrinsic language support that varies from compiler to compiler.
    /// <br/><br/>
    /// Make sure you declare your function pointers with the same calling convention as the actual
    /// library function. Your code will crash mysteriously if you do not do this.
    /// <br/><br/>
    /// If the requested function doesn't exist, exception is thrown.
    /// </remarks>
    public IntPtr LoadFunction(string name) {
        var ptr = SDL_LoadFunction(this, name);
        if (ptr == 0) throw new SdlException("Failed to load function: ");
        return ptr;
    }

    public override void Dispose() {
        base.Dispose();
        SDL_UnloadObject(this);
    }
}