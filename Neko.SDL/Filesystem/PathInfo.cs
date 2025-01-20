using System.Runtime.InteropServices;

namespace Neko.Sdl.Filesystem;

[StructLayout(LayoutKind.Sequential)]
public struct PathInfo {
    public PathType Type;
    public ulong Size;
    public long CreateTime;
    public long ModifyTime;
    public long AccessTime;
}