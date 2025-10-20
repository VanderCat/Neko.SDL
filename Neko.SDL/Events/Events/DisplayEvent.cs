using Neko.Sdl.CodeGen;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public partial struct DisplayEvent {
    public EventType Type;
    public uint Reserved;
    public ulong Timestamp;
    public uint DisplayID;
    public int Data1;
    public int Data2;
}