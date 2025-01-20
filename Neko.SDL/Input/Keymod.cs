using Neko.Sdl.CodeGen;

namespace Neko.Sdl.Input;

[Flags]
[GenEnum(nameof(SDL_Keymod), "SDL_KMOD_")]
file enum Keymod { }