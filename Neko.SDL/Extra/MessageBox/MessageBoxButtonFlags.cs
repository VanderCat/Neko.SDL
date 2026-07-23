using Neko.Sdl.CodeGen;

namespace Neko.Sdl.Extra.MessageBox;

[Flags]
public enum MessageBoxButtonFlags: uint {
    None = 0,
    ReturnKeyDefault = SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT,
    EscapeKeyDefault = SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_ESCAPEKEY_DEFAULT,
}