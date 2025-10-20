using Neko.Sdl.CodeGen;
using Neko.Sdl.Input;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public partial struct KeyboardEvent {
    /// <summary>
    /// Event type, shared with all events, Uint32 to cover user events which are not in the SDL_EventType enumeration
    /// </summary>
    public uint Type;
    public uint Reserved;
    /// <summary>
    /// In nanoseconds, populated using <see cref="Neko.Sdl.Time.Timer.GetTicksNS"/>
    /// </summary>
    public ulong Timestamp;
    public uint WindowId;
    public Window Window => Window.GetById(WindowId);
    public uint Which;
    public Scancode Scancode;
    public Keycode Key;
    public Keymod Mod;
    public ushort Raw;
    private SDLBool _down;
    private SDLBool _repeat;
    public bool Down {
        get => _down;
        set => _down = value;
    }
    public bool Repeat {
        get => _repeat;
        set => _repeat = value;
    }
}