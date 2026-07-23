namespace Neko.Sdl.Extra.MessageBox;

public class MessageBoxButton {
    public MessageBoxButtonFlags Flags;
    /// <summary>
    /// User defined button id (value returned via <see cref="MessageBox.Show"/>)
    /// </summary>
    public int ButtonId;
    /// <summary>
    /// The button text
    /// </summary>
    public string Text;
}