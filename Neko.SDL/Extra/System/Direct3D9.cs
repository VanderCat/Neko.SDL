namespace Neko.Sdl.Extra.System;

//disabled due to dxvk native existance
#pragma warning disable CA1416
public static unsafe class Direct3D {
    public struct AdapterInfo {
        public int AdapterIndex;
        public int OutputIndex;
    }
    
    /// <summary>
    /// Get the D3D9 adapter index that matches the specified display.
    /// </summary>
    /// <param name="displayId">the instance of the display to query.</param>
    /// <returns>Returns the D3D9 adapter index.</returns>
    /// <remarks>
    /// The returned adapter index can be passed to IDirect3D9::CreateDevice and controls on which monitor a full screen
    /// application will appear.
    /// </remarks>
    public static int GetD3D9AdapterIndex(uint displayId) {
        var adapter = SDL_GetDirect3D9AdapterIndex((SDL_DisplayID)displayId);
        if (adapter == -1) throw new SdlException();
        return adapter;
    }

    /// <summary>
    /// Get the DXGI Adapter and Output indices for the specified display.
    /// </summary>
    /// <param name="displayId">the instance of the display to query.</param>
    public static AdapterInfo GetDxgiOutputInfo(uint displayId) {
        var adapterInfo = new AdapterInfo();
        if (!SDL_GetDXGIOutputInfo((SDL_DisplayID)displayId, &adapterInfo.AdapterIndex, &adapterInfo.OutputIndex))
            throw new SdlException();
        return adapterInfo;
    }
    
}
#pragma warning restore CA1416