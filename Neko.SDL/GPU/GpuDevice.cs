using Neko.Sdl.Video;

namespace Neko.Sdl.GPU;

public unsafe partial class GpuDevice : SdlWrapper<SDL_GPUDevice>  {
    /// <inheritdoc cref="GpuDevice(Neko.Sdl.GPU.GpuShaderFormat, bool)"/>
    /// <param name="driver">the preferred GPU driver</param>
    public GpuDevice(GpuShaderFormat shaderFormat, bool debugMode, string driver) {
        Handle = SDL_CreateGPUDevice((SDL_GPUShaderFormat)shaderFormat, debugMode, driver);
        if (Handle is null) throw new SdlException();
    }
    
    /// <summary>
    /// Creates a GPU context
    /// </summary>
    /// <param name="shaderFormat">which shader formats the app is able to provide</param>
    /// <param name="debugMode">enable debug mode properties and validations</param>
    /// <exception cref="SdlException"></exception>
    public GpuDevice(GpuShaderFormat shaderFormat, bool debugMode) {
        Handle = SDL_CreateGPUDevice((SDL_GPUShaderFormat)shaderFormat, debugMode, (byte*)0);
        if (Handle is null) throw new SdlException();
    }

    public void ClaimWindow(Window window) =>
        SDL_ClaimWindowForGPUDevice(Handle, window.Handle).ThrowIfError();
    
}