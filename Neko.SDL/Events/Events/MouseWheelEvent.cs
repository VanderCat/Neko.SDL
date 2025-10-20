using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Neko.Sdl.CodeGen;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public unsafe partial struct MouseWheelEvent {
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
    public Vector2 Pos;
    public float X => Pos.X;
    public float Y => Pos.Y;
    public SDL_MouseWheelDirection Direction; //TODO:
    public Vector2 MousePos;
    public int IntegerX;
    public int IntegerY;
}