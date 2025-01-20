using Neko.Sdl.CodeGen;

namespace Neko.Sdl.Video;

[Flags]
[GenEnum(nameof(SDL_WindowFlags), "SDL_WINDOW_")]
file enum WindowFlags { }