using System.Numerics;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Video;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex {
    public Vector2 Position;
    public ColorF Color;
    public Vector2 TexCoord;
}