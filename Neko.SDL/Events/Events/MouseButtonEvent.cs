using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Neko.Sdl.CodeGen;
using Neko.Sdl.Input;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public unsafe partial struct MouseButtonEvent {
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
    private byte _button;
    private SDLBool Down;
    public byte Clicks;
    public byte Padding;
    public Vector2 Pos;
    public float X => Pos.X;
    public float Y => Pos.Y;

    public MouseButtonFlags Button {
        get => (MouseButtonFlags)_button;
        set => _button = (byte)value;
    }
}