using System.Runtime.InteropServices;
using Neko.Sdl.CodeGen;

namespace Neko.Sdl.GPU;


//TODO: is wrapper necessary? could we create on demand from simple class/struct?
public unsafe partial class GpuShaderCreateInfo : SdlWrapper<SDL_GPUShaderCreateInfo>, IDisposable {
    private Pin<byte[]>? _code;
    
    /// <summary>
    /// Shader code
    /// </summary>
    public byte[]? Code {
        get => _code?.Target;
        set {
            _code?.Dispose();
            if (value is null || value.Length <= 0) {
                _code = null;
                Handle->code = (byte*)0;
                Handle->code_size = 0;
                return;
            }
            _code = value.Pin();
            Handle->code = (byte*)_code.Addr;
            Handle->code_size = (nuint)value.Length;
        }
    }

    public override void Dispose() {
        _code?.Dispose();
        GC.SuppressFinalize(this);
    }

    //FIXME: this is leaky
    /// <summary>
    /// String specifying the entry point function name for the shader
    /// </summary>
    public string? Entrypoint {
        get => Marshal.PtrToStringUTF8((IntPtr)Handle->entrypoint);
        set => Marshal.StringToCoTaskMemUTF8(value);
    }
    
    /// <summary>
    /// The format of the shader code
    /// </summary>
    [GenAccessor("format", true)]
    public partial GpuShaderFormat Format { get; set; }
    
    /// <summary>
    /// The stage the shader program corresponds to
    /// </summary>
    [GenAccessor("stage", true)]
    public partial GpuShaderStage Stage { get; set; }
    
    /// <summary>
    /// The number of samplers defined in the shader
    /// </summary>
    [GenAccessor("num_samplers")]
    public partial uint SamplerCount { get; set; }
    
    /// <summary>
    /// The number of storage textures defined in the shader
    /// </summary>
    [GenAccessor("num_storage_textures")]
    public partial uint StorageTextureCount { get; set; }
    
    /// <summary>
    /// The number of storage buffers defined in the shader
    /// </summary>
    [GenAccessor("num_storage_buffers")]
    public partial uint StorageBufferCount { get; set; }
    
    /// <summary>
    /// The number of uniform buffers defined in the shader
    /// </summary>
    [GenAccessor("num_uniform_buffers")]
    public partial uint UniformBufferCount { get; set; }

    /// <summary>
    /// A properties ID for extensions. Should be null if no extensions are needed
    /// </summary>
    private Properties? _properties;
    public Properties? Properties {
        get {
            if (_properties is null) {
                var id = Handle->props;
                if (id == 0) return null;
                _properties = (Properties)id;
            }
            return _properties;
        }
        set {
            _properties = value;
            if (_properties is null) {
                Handle->props = 0;
                return;
            }
            Handle->props = (SDL_PropertiesID)_properties.Id;
        }
    }
}