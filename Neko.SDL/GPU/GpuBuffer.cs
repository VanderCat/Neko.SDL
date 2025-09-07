using System.Runtime.CompilerServices;

namespace Neko.Sdl.GPU;

public unsafe partial class GpuBuffer : SdlWrapper<SDL_GPUBuffer> {
    public GpuBuffer(GpuDevice device, GpuBufferCreateInfo createInfo) {
        var create = new SDL_GPUBufferCreateInfo();
        if (createInfo.Properties is not null)
            create.props = (SDL_PropertiesID)createInfo.Properties.Id;
        else
            create.props = 0;
        create.size = createInfo.Size;
        create.usage = (SDL_GPUBufferUsageFlags)createInfo.UsageFlags;
        Handle = SDL_CreateGPUBuffer(device, &create);
    }
}