using System.Runtime.InteropServices;
using System.Text;

namespace Neko.Sdl.Extra.StandardLibrary;

public unsafe partial class Environment : SdlWrapper<SDL_Environment> {
    private static Environment? _instance;
    /// <summary>
    /// The process environment
    /// </summary>
    /// <remarks>
    /// This is initialized at application start and is not affected by setenv() and unsetenv() calls after that point.
    /// Use <see cref="SetVariable"/> and <see cref="UnsetVariable"/> if you want to modify this environment, or
    /// SDL_setenv_unsafe() or SDL_unsetenv_unsafe() if you want changes to persist in the C runtime environment after
    /// SDL_Quit().
    /// </remarks>
    public static Environment Instance {
        get {
            if (_instance is null) {
                var result = SDL_GetEnvironment();
                if (result is null)
                    throw new SdlException();
                _instance = result;
            }
            return _instance;
        }
    } 
    
    /// <summary>
    /// Create a set of environment variables
    /// </summary>
    /// <param name="populated">true to initialize it from the C runtime environment, false to create an empty environment</param>
    /// <returns>new environment</returns>
    public static Environment Create(bool populated) {
        var result = SDL_CreateEnvironment(populated);
        if (result == null)
            throw new SdlException();
        return result;
    }

    public string? GetVariable(string name) => SDL_GetEnvironmentVariable(this, name);
    public void SetVariable(string name, string value, bool overwrite = true) => 
        SDL_SetEnvironmentVariable(this, name, value, overwrite).ThrowIfError();

    public void UnsetVariable(string name) => SDL_UnsetEnvironmentVariable(this, name).ThrowIfError();

    public string? this[string index] {
        get => GetVariable(index);
        set {
            if (value is null) {
                UnsetVariable(index);
                return;
            }
            SetVariable(index, value);
        }
    }

    public string[] GetVariables() {
        var result = SDL_GetEnvironmentVariables(this);
        if (result == null)
            throw new SdlException();
        var len = -1;
        while (result[++len] is not null);
        var strings = new string[len];
        for (int i = 0; i < len; i++) 
            strings[i] = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(result[i]));
        UnmanagedMemory.Free(result);
        return strings;
    }

    public override void Dispose() {
        SDL_DestroyEnvironment(this);
        base.Dispose();
    }
}