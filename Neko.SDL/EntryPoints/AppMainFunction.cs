namespace Neko.Sdl.EntryPoints;

public delegate int AppMainFunction(string[] arg);
internal unsafe delegate int AppMainFunction2(int argc, byte** argv);