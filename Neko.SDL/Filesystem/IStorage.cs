using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Filesystem;

public unsafe interface IStorage {
    public void Close();
    
    public void Ready();
    
    public void Enumerate(string path/*, SDL_EnumerateDirectoryCallback callback*/);
    
    public PathInfo Info(string path);
    
    public void ReadFile(string path, ref Span<byte> destination, ulong length);
    
    public void WriteFile(string path, ref Span<byte> destination, ulong length);
    
    public void Mkdir(string path);
    
    public void Remove(string path);
    
    public void Rename(string oldpath, string newpath);
    
    public void Copy(string oldpath, string newpath);

    public ulong SpaceRemaining();
}