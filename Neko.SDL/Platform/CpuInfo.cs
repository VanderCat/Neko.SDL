namespace Neko.Sdl.Platform;

public class CpuInfo {
    static CpuInfo() {
        CacheLineSize = SDL_GetCPUCacheLineSize();
        LogicalCpuCoresCount = SDL_GetNumLogicalCPUCores();
        SimdAlignment = SDL_GetSIMDAlignment();
        SystemRam = SDL_GetSystemRAM();
        HasAltiVec = SDL_HasAltiVec();
        HasARMSIMD = SDL_HasARMSIMD();
        HasAVX = SDL_HasAVX();
        HasAVX2 = SDL_HasAVX2();
        HasAVX512F = SDL_HasAVX512F();
        HasLASX = SDL_HasLASX();
        HasLSX = SDL_HasLSX();
        HasMMX = SDL_HasMMX();
        HasNEON = SDL_HasNEON();
        HasSSE = SDL_HasSSE();
        HasSSE2 = SDL_HasSSE2();
        HasSSE3 = SDL_HasSSE3();
        HasSSE41 = SDL_HasSSE41();
        HasSSE42 = SDL_HasSSE42();
    }
    
    public static readonly int CacheLineSize;
    public static readonly int LogicalCpuCoresCount;
    public static readonly nuint SimdAlignment;
    public static readonly int SystemRam;
    public static readonly bool HasAltiVec;
    public static readonly bool HasARMSIMD;
    public static readonly bool HasAVX;
    public static readonly bool HasAVX2;
    public static readonly bool HasAVX512F;
    public static readonly bool HasLASX;
    public static readonly bool HasLSX;
    public static readonly bool HasMMX;
    public static readonly bool HasNEON;
    public static readonly bool HasSSE;
    public static readonly bool HasSSE2;
    public static readonly bool HasSSE3;
    public static readonly bool HasSSE41;
    public static readonly bool HasSSE42;
}