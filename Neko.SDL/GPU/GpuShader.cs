namespace Neko.Sdl.GPU;

public unsafe partial class GpuShader : SdlWrapper<SDL_GPUShader> {
    /// <summary>
    /// Creates a shader to be used when creating a graphics pipeline
    /// </summary>
    /// <param name="device">a GPU Context</param>
    /// <param name="info">a class describing the state of the shader to create</param>
    /// <remarks>
    /// Shader resource bindings must be authored to follow a particular order depending on the shader format.
    /// <br/><br/>
    /// For SPIR-V shaders, use the following resource sets:
    /// <br/><br/>
    /// For vertex shaders:
    /// <br/><br/>
    /// 0: Sampled textures, followed by storage textures, followed by storage buffers
    /// 1: Uniform buffers
    /// <br/><br/>
    /// For fragment shaders:
    /// <br/><br/>
    ///     2: Sampled textures, followed by storage textures, followed by storage buffers
    ///     3: Uniform buffers
    /// <br/><br/>
    /// For DXBC and DXIL shaders, use the following register order:
    /// <br/><br/>
    /// For vertex shaders:
    /// <br/><br/>
    ///     (t[n], space0): Sampled textures, followed by storage textures, followed by storage buffers
    ///     (s[n], space0): Samplers with indices corresponding to the sampled textures
    ///     (b[n], space1): Uniform buffers
    /// <br/><br/>
    /// For pixel shaders:
    /// <br/><br/>
    ///     (t[n], space2): Sampled textures, followed by storage textures, followed by storage buffers
    ///     (s[n], space2): Samplers with indices corresponding to the sampled textures
    ///     (b[n], space3): Uniform buffers
    /// <br/><br/>
    /// For MSL/metallib, use the following order:
    /// <br/><br/>
    ///     [[texture]]: Sampled textures, followed by storage textures
    ///     [[sampler]]: Samplers with indices corresponding to the sampled textures
    ///     [[buffer]]: Uniform buffers, followed by storage buffers. Vertex buffer 0 is bound at [[buffer(14)]],
    ///                 vertex buffer 1 at [[buffer(15)]], and so on. Rather than manually authoring vertex buffer
    ///                 indices, use the [[stage_in]] attribute which will automatically use the vertex input
    ///                 information from the SDL_GPUGraphicsPipeline.
    /// <br/><br/>
    /// Shader semantics other than system-value semantics do not matter in D3D12 and for ease of use the SDL implementation assumes that non system-value semantics will all be TEXCOORD. If you are using HLSL as the shader source language, your vertex semantics should start at TEXCOORD0 and increment like so: TEXCOORD1, TEXCOORD2, etc. If you wish to change the semantic prefix to something other than TEXCOORD you can use SDL_PROP_GPU_DEVICE_CREATE_D3D12_SEMANTIC_NAME_STRING with SDL_CreateGPUDeviceWithProperties().
    /// <br/><br/>
    /// There are optional properties that can be provided through props. These are the supported properties:
    /// <br/><br/>
    /// SDL_PROP_GPU_SHADER_CREATE_NAME_STRING: a name that can be displayed in debugging tools.
    /// </remarks>
    public GpuShader(GpuDevice device, GpuShaderCreateInfo info) {
        Handle = SDL_CreateGPUShader(device, info);
        if (Handle is null) throw new SdlException();
    }
}