namespace Neko.Sdl;

/// <summary>
/// Simple error message routines wrapper for SDL.
/// <br/><br/>
/// This exception is thrown automatically when any sdl error occurs. You can also use
/// the extension <see cref="Extensions.ThrowIfError"/>.
/// <br/><br/>
/// Most apps will interface with these APIs in exactly one function:
/// when almost any SDL function call reports failure, you can get a human-readable
/// string of the problem from <see cref="Error"/>.
/// <br/><br/>
/// These strings are maintained per-thread, and apps are welcome to set their own errors,
/// which is popular when building libraries on top of SDL for other apps to consume.
/// These strings are set by setting <see cref="Error"/>.
/// </summary>
/// <param name="message"></param>
public class SdlException(string message = "") : Exception(message + "\n" + Error) {
    
    /// <summary>
    /// Set or get the SDL error message for the current thread
    /// </summary>
    public static unsafe string? Error {
        get => SDL_GetError();
        set {
            if (value is null) {
                SDL_ClearError();
                return;
            }
            var handle = new RuntimeArgumentHandle();
            SDL_SetErrorV(value.Replace("%", "%%"), (byte*)&handle);
        }
    }

    public static bool HasError => Error is not null && Error != "";

    /// <summary>
    /// Set an error indicating that memory allocation failed
    /// </summary>
    /// <remarks>
    /// This function does not do any memory allocation
    /// </remarks>
    public static void OutOfMemory() {
        SDL_OutOfMemory();
    }
}