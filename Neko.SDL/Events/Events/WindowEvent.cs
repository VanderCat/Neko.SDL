using Neko.Sdl.CodeGen;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public partial struct WindowEvent {
    public EventType Type;
    public uint Reserved;
    /// <summary>
    /// In nanoseconds, populated using <see cref="Neko.Sdl.Time.Timer.GetTicksNS"/>
    /// </summary>
    public ulong Timestamp;
    public uint WindowId;
    /// <summary>
    /// The associated window
    /// </summary>
    public Window Window => Window.GetById(WindowId);
    public int Data1;
    public int Data2;
}