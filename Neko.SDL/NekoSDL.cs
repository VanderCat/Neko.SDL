using System.Text;
using Neko.Sdl.Extra.System;

namespace Neko.Sdl;

/// <summary>
/// All SDL programs need to initialize the library before starting to work with it.
/// Almost everything can simply call <see cref="Init"/> near startup, with a handful of flags to specify subsystems to touch.
/// These are here to make sure SDL does not even attempt to touch low-level pieces of the operating system that you
/// don't intend to use. For example, you might be using SDL for video and input but chose an external library for
/// audio, and in this case you would just need to leave off the <see cref="InitFlags.Audio"/> flag to make sure that
/// external library has complete control.
/// <br/><br/>
/// Most apps, when terminating, should call <see cref="Quit"/>. This will clean up (nearly) everything that SDL might have
/// allocated, and crucially, it'll make sure that the display's resolution is back to what the user expects if you had
/// previously changed it for your game.
/// </summary>
public static class NekoSDL {
    /// <summary>
    /// Initialize the SDL library
    /// </summary>
    /// <param name="flags">subsystem initialization flags</param>
    /// <remarks>
    /// <see cref="Init"/> simply forwards to calling <see cref="InitSubSystem"/>. Therefore, the two may be used interchangeably.
    /// Though for readability of your code <see cref="InitSubSystem"/> might be preferred.
    /// <br/><br/>
    /// The file I/O (for example: SDL_IOFromFile) and threading (SDL_CreateThread) subsystems are initialized by default.
    /// Message boxes (<see cref="Extra.MessageBox.MessageBox"/>) also attempt to work without initializing the video subsystem, in hopes
    /// of being useful in showing an error dialog when <see cref="Init"/> fails. You must specifically initialize other
    /// subsystems if you use them in your application.
    /// <br/><br/>
    /// Logging (such as SDL_Log) works without initialization, too.
    /// <br/><br/>
    /// flags may be any of the following OR'd together:
    /// <ul>
    /// <li><see cref="InitFlags.Audio"/>: audio subsystem; automatically initializes the events subsystem</li>
    /// <li><see cref="InitFlags.Video"/>: video subsystem; automatically initializes the events subsystem, should be initialized on the main thread.</li>
    /// <li><see cref="InitFlags.Joystick"/>: joystick subsystem; automatically initializes the events subsystem</li>
    /// <li><see cref="InitFlags.Haptic"/>: haptic (force feedback) subsystem</li>
    /// <li><see cref="InitFlags.Gamepad"/>: gamepad subsystem; automatically initializes the joystick subsystem</li>
    /// <li><see cref="InitFlags.Events"/>: events subsystem</li>
    /// <li><see cref="InitFlags.Sensor"/>: sensor subsystem; automatically initializes the events subsystem</li>
    /// <li><see cref="InitFlags.Camera"/>: camera subsystem; automatically initializes the events subsystem</li>
    /// </ul>
    /// Subsystem initialization is ref-counted, you must call <see cref="QuitSubSystem"/> for each <see cref="InitSubSystem"/> to
    /// correctly shutdown a subsystem manually (or call <see cref="Quit"/> to force shutdown). If a subsystem is already loaded
    /// then this call will increase the ref-count and return.
    /// Consider reporting some basic metadata about your application before calling <see cref="Init"/>, using
    /// <see cref="AppMetadata"/>.
    /// </remarks>
    public static void Init(InitFlags flags) => InitSubSystem(flags);
    
    /// <summary>
    /// Compatibility function to initialize the SDL library.
    /// </summary>
    /// <param name="flags">any of the flags used by <see cref="Init"/>; see <see cref="Init"/> for details</param>
    /// <seealso cref="Init"/>
    /// <remarks>
    /// This function and SDL_Init() are interchangeable.
    /// </remarks>
    public static void InitSubSystem(InitFlags flags) {
        SDL_InitSubSystem((SDL_InitFlags)flags).ThrowIfError("Failed to initialize SDL due to");
    }

    /// <summary>
    /// Get a mask of the specified subsystems which are currently initialized.
    /// </summary>
    /// <param name="flags">any of the flags used by <see cref="Init"/>; see <see cref="Init"/> for details</param>
    /// <returns>initialization status of the specified subsystems</returns>
    public static InitFlags WasInit(InitFlags flags) => (InitFlags)SDL_WasInit((SDL_InitFlags)(uint)flags);
    
    /// <summary>
    /// Get subsystems which are currently initialized.
    /// </summary>
    /// <param name="flags">any of the flags used by <see cref="Init"/>; see <see cref="Init"/> for details</param>
    /// <returns>mask of all initialized subsystems</returns>
    public static InitFlags WasInit() => (InitFlags)SDL_WasInit((SDL_InitFlags)(uint)0);

    /// <summary>
    /// Shut down specific SDL subsystems.
    /// </summary>
    /// <param name="flags">any of the flags used by <see cref="Init"/>; see <see cref="Init"/> for details</param>
    /// <remarks>You still need to call <see cref="Quit"/> even if you close all open subsystems with <see cref="QuitSubSystem"/></remarks>
    public static void QuitSubSystem(InitFlags flags) => SDL_QuitSubSystem((SDL_InitFlags)(uint)flags);

    /// <summary>
    /// Clean up all initialized subsystems.
    /// </summary>
    /// <remarks>
    /// You should call this function even if you have already shutdown each initialized subsystem with
    /// <see cref="QuitSubSystem"/>. It is safe to call this function even in the case of errors in initialization.
    /// <br/><br/>
    /// You can use this function with <see cref="AppDomain.ProcessExit"/> to ensure that it is run when your
    /// application is shutdown, but it is not wise to do this from a library or other dynamically loaded code.
    /// </remarks>
    public static void Quit() => SDL_Quit();

    /// <summary>
    /// Get the version of SDL that is linked against your program.
    /// </summary>
    /// <remarks>
    /// If you are linking to SDL dynamically, then it is possible that the current version will be different than
    /// the version you compiled against. This function returns the current version, while <see cref="BindingsVersion"/>
    /// is the version this wrapper used.
    ///
    /// This function may be called safely at any time, even before <see cref="Init"/>
    /// </remarks>
    public static int Version => SDL_GetVersion();
    
    /// <summary>
    /// Version of SDL bindings were generated against
    /// </summary>
    public static int BindingsVersion => SDL_VERSION;
    
    /// <summary>
    /// An arbitrary string, uniquely identifying the exact revision of the SDL library in use.
    /// </summary>
    /// <remarks>
    /// This value is the revision of the code you are linked with and may be different from the code you are
    /// compiling with, which is found in the constant SDL_REVISION.
    /// <br/><br/>
    /// The revision is arbitrary string (a hash value) uniquely identifying the exact revision of the SDL
    /// library in use, and is only useful in comparing against other revisions. It is NOT an incrementing number.
    /// <br/><br/>
    /// If SDL wasn't built from a git repository with the appropriate tools, this will return an empty string.
    /// <br/><br/>
    /// You shouldn't use this function for anything but logging it for debugging purposes. The string is not
    /// intended to be reliable in any way.
    /// </remarks>
    public static string Revision => SDL_GetRevision();
    
    /// <summary>
    /// Revision of SDL bindings were generated against
    /// </summary>
    public static string BindingsRevision => Encoding.UTF8.GetString(SDL_REVISION);

    /// <summary>
    /// Get the application sandbox environment, if any.
    /// </summary>
    public static Sandbox Sandbox => (Sandbox)SDL_GetSandbox();
    
    /// <summary>
    /// Open a URL/URI in the browser or other appropriate external application.
    /// </summary>
    /// <param name="url">a valid URL/URI to open. Use file:///full/path/to/file for local files, if supported.</param>
    /// <remarks>
    /// Open a URL in a separate, system-provided application. How this works will vary wildly depending on the
    /// platform. This will likely launch what makes sense to handle a specific URL's protocol (a web browser for
    /// http://, etc), but it might also be able to launch file managers for directories and other things.
    /// <br/><br/>
    /// What happens when you open a URL varies wildly as well: your game window may lose focus (and may or may not
    /// lose focus if your game was fullscreen or grabbing input at the time). On mobile devices, your app will likely
    /// move to the background or your process might be paused. Any given platform may or may not handle a given URL.
    /// <br/><br/>
    /// If this is unimplemented (or simply unavailable) for a platform, this will fail with an error. A successful
    /// result does not mean the URL loaded, just that we launched something to handle it (or at least believe we did).
    /// <br/><br/>
    /// All this to say: this function can be useful, but you should definitely test it on every platform you target.
    /// </remarks>
    public static void OpenUrl(string url) => SDL_OpenURL(url).ThrowIfError();
}