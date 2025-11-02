using System.Text;
using Neko.Sdl.GPU;
using Neko.Sdl.Video;

namespace Neko.Sdl.Ttf;

public unsafe partial class TextEngine : SdlWrapper<TTF_TextEngine> {
    /// <summary>
    /// Create a text engine for drawing text on SDL surfaces.
    /// </summary>
    /// <returns></returns>
    public static TextEngine CreateSurface() {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Destroy a text engine created for drawing text on SDL surfaces.
    /// </summary>
    public void DestroySurface() {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Create a text engine for drawing text on an SDL renderer.
    /// </summary>
    /// <param name="renderer"></param>
    /// <returns></returns>
    public static TextEngine CreateRenderer(Renderer renderer) => TTF_CreateRendererTextEngine(renderer);
    
    /// <summary>
    /// Create a text engine for drawing text on an SDL renderer, with the specified properties.
    /// </summary>
    /// <param name="props"></param>
    /// <returns></returns>
    public TextEngine CreateRenderer(Properties props) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Destroy a text engine created for drawing text on an SDL renderer.
    /// </summary>
    public void DestroyRenderer() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create a text engine for drawing text with the SDL GPU API.
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    public static TextEngine CreateGPU(GpuDevice device) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create a text engine for drawing text with the SDL GPU API, with the specified properties.
    /// </summary>
    /// <param name="props"></param>
    /// <returns></returns>
    public static TextEngine CreateGPU(Properties props) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Destroy a text engine created for drawing text with the SDL GPU API.
    /// </summary>
    public static void DestroyGPU() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the winding order of the vertices returned by TTF_GetGPUTextDrawData for a particular GPU text engine.
    /// </summary>
    public TTF_GPUTextEngineWinding Winding { get; set; }
    
    /// <summary>
    /// Create a text object from UTF-8 text and a text engine.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public Text CreateText(Font font, string text) {
        using var utf8str = text.RentUtf8();
        fixed (byte* utf8ptr = utf8str.Rented) {
            var textObj = TTF_CreateText(this, font, utf8ptr, (nuint)utf8str.Length);
            if (textObj is null)
                throw new SdlException();
            return textObj;
        }
    }
}