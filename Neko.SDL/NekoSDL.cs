using System.Text;
using Neko.Sdl.Extra.System;

namespace Neko.Sdl;

public static class NekoSDL {
    
    public static void Init(InitFlags flags) => InitSubSystem(flags);
    public static void InitSubSystem(InitFlags flags) {
        SDL_InitSubSystem((SDL_InitFlags)flags).ThrowIfError("Failed to initialize SDL due to");
    }

    public static void WasInit(InitFlags flags) => SDL_WasInit((SDL_InitFlags)(uint)flags);

    public static void QuitSubSystem(InitFlags flags) => SDL_QuitSubSystem((SDL_InitFlags)(uint)flags);

    public static void Quit() => SDL_Quit();

    public static int Version => SDL_GetVersion();
    public static int BindingsVersion => SDL_VERSION;
    
    public static string Revision => SDL_GetRevision();
    public static string BindingsRevision => Encoding.UTF8.GetString(SDL_REVISION);

    public static Sandbox GetSandbox() => (Sandbox)SDL_GetSandbox();
    public static void OpenUrl(string url) => SDL_OpenURL(url).ThrowIfError();
}