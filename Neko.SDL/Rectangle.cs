using System.Runtime.InteropServices;

namespace Neko.Sdl;

[StructLayout(LayoutKind.Sequential)]
public struct Rectangle(int x, int y, int width, int height) {
    public int X = x;
    public int Y = y;
    public int Width = width;
    public int Height = height;
}