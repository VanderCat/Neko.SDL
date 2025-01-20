namespace Neko.Sdl.Extra.System;

//disabled due to dxvk native existance
#pragma warning disable CA1416
public static unsafe class Direct3D {
    public struct AdapterInfo {
        public int AdapterIndex;
        public int OutputIndex;
    }
    
    public static int GetD3D9AdapterIndex(uint displayId) {
        var adapter = SDL_GetDirect3D9AdapterIndex((SDL_DisplayID)displayId);
        if (adapter == -1) throw new SdlException("");
        return adapter;
    }

    public static AdapterInfo GetDxgiOutputInfo(uint displayId) {
        var adapterInfo = new AdapterInfo();
        SDL_GetDXGIOutputInfo((SDL_DisplayID)displayId, &adapterInfo.AdapterIndex, &adapterInfo.OutputIndex);
        return adapterInfo;
    }
    
}
#pragma warning restore CA1416