using System.Runtime.CompilerServices;

namespace Neko.Sdl.Extra.MessageBox;

public class MessageBoxColorScheme {
    public Color Background;
    public Color Text;
    public Color ButtonBorder;
    public Color ButtonBackground;
    public Color ButtonSelected;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PopulateColor(ref SDL_MessageBoxColor msgColor, ref Color color) {
        msgColor.r = color.R;
        msgColor.g = color.G;
        msgColor.b = color.B;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Populate(ref SDL_MessageBoxColorScheme msgColor, ref MessageBoxColorScheme color) {
        PopulateColor(ref msgColor.colors[0], ref color.Background);
        PopulateColor(ref msgColor.colors[1], ref color.Text);
        PopulateColor(ref msgColor.colors[2], ref color.ButtonBorder);
        PopulateColor(ref msgColor.colors[3], ref color.ButtonBackground);
        PopulateColor(ref msgColor.colors[4], ref color.ButtonSelected);
    }
}