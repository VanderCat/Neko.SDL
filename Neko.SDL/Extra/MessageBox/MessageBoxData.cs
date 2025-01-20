using Neko.Sdl.CodeGen;

namespace Neko.Sdl.Extra.MessageBox;

[Obsolete("unfinished")]
public unsafe partial class MessageBoxData : SdlWrapper<SDL_MessageBoxData> {
    private Pin<string>? _title;
    private Pin<string>? _message;

    [GenAccessor("flags", true)]
    public partial MessageBoxFlags Flags { get; set; }

    public string Title {
        get => _title?.Target!;
        set {
            _title?.Dispose();
            _title = value.Pin();
            Handle->title = (byte*)_title.Addr;
        }
    }

    public string Message {
        get => _message?.Target!;
        set {
            _message?.Dispose();
            _message = value.Pin();
            Handle->message = (byte*)_message.Addr;
        }
    }
}