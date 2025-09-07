namespace Neko.Sdl.GPU;

public static class GpuExtensions {
    public static GpuShader CreateShader(this GpuDevice device, GpuShaderCreateInfo createInfo) => 
        new (device, createInfo);
    
    public static GpuShader Create(this GpuShaderCreateInfo createInfo, GpuDevice device) => 
        new (device, createInfo);
    
    public static GpuBuffer CreateBuffer(this GpuDevice device, GpuBufferCreateInfo createInfo) =>
        new (device, createInfo);
    
    public static GpuBuffer Create(this GpuBufferCreateInfo createInfo, GpuDevice device) =>
        new (device, createInfo);
}