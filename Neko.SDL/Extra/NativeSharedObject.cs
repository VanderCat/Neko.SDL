namespace Neko.Sdl.Extra;

public unsafe partial class NativeSharedObject : SdlWrapper<SDL_SharedObject> {
    public static NativeSharedObject Load(string path) {
        var ptr = SDL_LoadObject(path);
        if (ptr is null) throw new SdlException("Failed to load Shared Object: ");
        return ptr;
    }

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