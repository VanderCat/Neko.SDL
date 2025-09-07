namespace Neko.Sdl.GPU;

public class GpuBufferCreateInfo {
    /// <summary>
    /// How the buffer is intended to be used by the client
    /// </summary>
    public GpuBufferUsageFlags UsageFlags;
    
    /// <summary>
    /// The size in bytes of the buffer
    /// </summary>
    public uint Size;

    /// <summary>
    /// Properties for extensions. Should be null if no extensions are needed. */
    /// </summary>
    public Properties? Properties;
}