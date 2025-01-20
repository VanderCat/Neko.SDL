using Neko.Sdl.CodeGen;

namespace Neko.Sdl.Events;

public partial class Event : SdlWrapper<SDL_Event> {
    [GenAccessor("type", true)]
    public partial EventType EventType { get; set; }
}