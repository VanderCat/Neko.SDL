using Neko.Sdl.CodeGen;

namespace Neko.Sdl.Extra.MessageBox;


public class MessageBoxData {
    public MessageBoxButtonFlags Flags = MessageBoxButtonFlags.None;
    public int ButtonID;
    public string Text;
}