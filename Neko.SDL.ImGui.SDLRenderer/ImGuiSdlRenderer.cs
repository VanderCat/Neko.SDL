using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using Neko.Sdl;
using Neko.Sdl.Video;
using SDL;
using Color = Neko.Sdl.Color;
using Rectangle = Neko.Sdl.Rectangle;

namespace Neko.Sdl.ImGuiBackend;

public static unsafe class ImGuiSdlRenderer {
    // SDL_Renderer data
    private class Data {
        public Renderer Renderer;       // Main viewport's renderer
        public Texture? FontTexture;
        public List<ColorF> ColorBuffer = new();
    }
    
    public class RenderState {
        public Renderer Renderer;
    }

    // Backend data stored in io.BackendRendererUserData to allow support for multiple Dear ImGui contexts
    // It is STRONGLY preferred that you use docking branch with multi-viewports (== single Dear ImGui context + multiple windows) instead of multiple Dear ImGui contexts.
    private static Data _data;
    private static Pin<Data> _pin;

    // Functions
    public static void Init(Renderer renderer) {
        var io = ImGui.GetIO();
        if (io.BackendRendererUserData != 0)
            throw new InvalidOperationException("Already initialized a renderer backend!");
        if (renderer is null)
            throw new InvalidOperationException("renderer is not initialized!");

        // Setup backend capabilities flags
        _data = new Data();
        _pin = _data.Pin(GCHandleType.Normal);
        io.BackendRendererUserData = _pin.Pointer;
        ((ImGuiIO*)io)->BackendRendererName = (byte*)Marshal.StringToHGlobalAnsi("imgui_impl_nekosdlrenderer");
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;  // We can honor the ImDrawCmd::VtxOffset field, allowing for large meshes.

        _data.Renderer = renderer;
    }

    public static void Shutdown() {
        
        if (_data == null)
            throw new InvalidOperationException("No renderer backend to shutdown, or already shutdown?");
        var io = ImGui.GetIO();

        DestroyDeviceObjects();

        ((ImGuiIO*)io)->BackendRendererName = null;
        new Pin<Data>(io.BackendRendererUserData, true).Dispose();
        io.BackendRendererUserData = 0;
        io.BackendFlags &= ~ImGuiBackendFlags.RendererHasVtxOffset;
    }

    private static void SetupRenderState(Renderer renderer) {
	    // Clear out any viewports and cliprect set by the user
        // FIXME: Technically speaking there are lots of other things we could backup/setup/restore during our render process.
        renderer.Viewport = null;
        renderer.ClipRect = null;
    }

    public static void NewFrame() {
        if (_data == null)
            throw new InvalidOperationException(
                "Context or backend not initialized! Did you call Init()?");

        if (_data.FontTexture is null)
            CreateDeviceObjects();
    }

    // https://github.com/libsdl-org/SDL/issues/9009
    // https://github.com/slouken/SDL/commit/416b9a93d61693c73cebaa8f0b37c50ce0bb732b
    public static void GeometryRaw8BitColor(this Renderer renderer, 
        Texture texture, 
        float *xy,
        int xyStride, 
        Color* color, 
        int colorStride, 
        float *uv, 
        int uvStride, 
        int numVertices, 
        IntPtr indices, 
        int numIndices, 
        int sizeIndices)
    {
        if (numVertices <= 0) 
            throw new ArgumentException(null, nameof(numVertices));
        if (color is null)
            throw new ArgumentException(null, nameof(color));
        
        // Resize the color buffer like in native code
        _data.ColorBuffer.Capacity = Math.Max(_data.ColorBuffer.Capacity, numVertices);
        _data.ColorBuffer.Clear();
    
        var color2 = (byte*)color;
        for (var i = 0; i < numVertices; ++i) {
            var c = (Color*)color2;
            _data.ColorBuffer.Add(new ColorF(*c));
            color2 += colorStride;
        }

        // Pin the color buffer for the native call
        fixed (ColorF* color3 = CollectionsMarshal.AsSpan(_data.ColorBuffer)) {
            /*if (!SDL_RenderGeometryRaw(renderer, texture,
                xy, xyStride,
                (SDL_FColor*)color3, Marshal.SizeOf<ColorF>(),
                uv, uvStride,
                numVertices,
                indices, numIndices, sizeIndices))Log.Warning($"geometry rendered with error: {SDL_GetError()}");*/
            renderer.GeometryRaw(
                texture,
                xy,
                xyStride,
                color3,
                Marshal.SizeOf<ColorF>(),
                uv,
                uvStride,
                numVertices,
                indices,
                numIndices,
                sizeIndices);
        }
    }


    private class BackupSdlRendererState {
        public Rectangle? Viewport;
        public bool ViewportEnabled;
        public bool ClipEnabled;
        public Rectangle? ClipRect;
    };
    
    static void DebugVertexData(ImDrawVert* vtxBuffer, int count, ushort* idxBuffer, int idxCount) {
        Console.WriteLine("Vertex Data:");
        for (int i = 0; i < Math.Min(count, 5); i++)
        {
            var vert = vtxBuffer[i];
            Console.WriteLine($"Vertex {i}: pos=({vert.pos.X}, {vert.pos.Y}), uv=({vert.uv.X}, {vert.uv.Y}), col=0x{vert.col:X8}");
        }

        Console.WriteLine("\nIndex Data:");
        for (int i = 0; i < Math.Min(idxCount, 5); i++)
        {
            Console.WriteLine($"Index {i}: {idxBuffer[i]} (0x{idxBuffer[i]:X4})");
        }
    }

    public static void RenderDrawData(ImDrawDataPtr drawData, Renderer renderer) {
	    // If there's a scale factor set by the user, use that instead
        // If the user has specified a scale factor to SDL_Renderer already via SDL_RenderSetScale(), SDL will scale whatever we pass
        // to SDL_RenderGeometryRaw() by that scale factor. In that case we don't want to be also scaling it ourselves here.

        var renderScale = renderer.Scale;
        renderScale = new Vector2 {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            X = (renderScale.X == 1.0f) ? drawData.FramebufferScale.X : 1.0f,
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Y = (renderScale.Y == 1.0f) ? drawData.FramebufferScale.Y : 1.0f,
        };

        // Avoid rendering when minimized, scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
	    var fbWidth = (int)(drawData.DisplaySize.X * renderScale.X);
	    var fbHeight = (int)(drawData.DisplaySize.Y * renderScale.Y);
	    if (fbWidth == 0 || fbHeight == 0)
		    return;

        // Backup SDL_Renderer state that will be modified to restore it afterwards
        var old = new BackupSdlRendererState {
            ViewportEnabled = renderer.RenderViewportSet(),
            ClipEnabled = renderer.ClipEnabled,
            Viewport = renderer.Viewport,
            ClipRect = renderer.ClipRect,
        };

        // Setup desired state
        SetupRenderState(renderer);

        // Setup render state structure (for callbacks and custom texture bindings)
        var platformIo = ImGui.GetPlatformIO();
        var renderState = new RenderState {
            Renderer = renderer,
        };
        var renderStatePin = renderState.Pin(GCHandleType.Normal);
        platformIo.Renderer_RenderState = renderStatePin.Pointer;

	    // Will project scissor/clipping rectangles into framebuffer space
	    var clipOff = drawData.DisplayPos;         // (0,0) unless using multi-viewports
	    var clipScale = renderScale;

        // Render command lists
        for (var n = 0; n < drawData.CmdListsCount; n++) {
            var drawList = drawData.CmdLists[n];
            var vtxBuffer = drawList.VtxBuffer.Data;
            var idxBuffer = drawList.IdxBuffer.Data;
            
            //Console.WriteLine($"DrawList {n}: VtxBuffer size: {drawList.VtxBuffer.Size}, IdxBuffer size: {drawList.IdxBuffer.Size}");

            for (var cmdI = 0; cmdI < drawList.CmdBuffer.Size; cmdI++) {
                var pcmd = drawList.CmdBuffer[cmdI];
                
                for (int i = 0; i < Math.Min(3, pcmd.ElemCount); i++) {
                    var idx = drawList.IdxBuffer[(int)pcmd.IdxOffset + i];
                    var vtx = drawList.VtxBuffer[(int)pcmd.VtxOffset + idx];
                    //Console.WriteLine($"Vertex {i}: Pos: ({vtx.pos.X}, {vtx.pos.Y}), UV: ({vtx.uv.X}, {vtx.uv.Y}), Col: {vtx.col:X8}");
                }
                
                //Console.WriteLine($"Command {cmdI}: ElemCount: {pcmd.ElemCount}, TextureId: {pcmd.TextureId}");
                if (pcmd.UserCallback != 0) {
                    // User callback, registered via ImDrawList::AddCallback()
                    // (ImDrawCallback_ResetRenderState is a special callback value used by the user to request the renderer to reset render state.)
                    if (pcmd.UserCallback == -8)
                        SetupRenderState(renderer);
                    else
                        ((delegate*unmanaged[Cdecl]<ImDrawList*, ImDrawCmd*, void>)pcmd.UserCallback)(
                            drawList, pcmd);
                }
                else {
                    // Project scissor/clipping rectangles into framebuffer space
                    var clipMin = new Vector2((pcmd.ClipRect.X - clipOff.X) * clipScale.X, (pcmd.ClipRect.Y - clipOff.Y) * clipScale.Y);
                    var clipMax = new Vector2((pcmd.ClipRect.Z - clipOff.X) * clipScale.X, (pcmd.ClipRect.W - clipOff.Y) * clipScale.Y);
                    if (clipMin.X < 0.0f) clipMin.X = 0.0f;
                    if (clipMin.Y < 0.0f) clipMin.Y = 0.0f;
                    if (clipMax.X > fbWidth) clipMax.X = fbWidth;
                    if (clipMax.Y > fbHeight) clipMax.Y = fbHeight;
                    if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y) continue;

                    var r = new Rectangle((int)clipMin.X, (int)clipMin.Y, (int)(clipMax.X - clipMin.X), (int)(clipMax.Y - clipMin.Y));
                    renderer.ClipRect = r;

                    var vtxOffsetBytes = pcmd.VtxOffset * Unsafe.SizeOf<ImDrawVert>();
                    var vtxBase = vtxBuffer + vtxOffsetBytes;

                    var xy = (float*)(vtxBase + Marshal.OffsetOf<ImDrawVert>("pos"));
                    var uv = (float*)(vtxBase + Marshal.OffsetOf<ImDrawVert>("uv"));
                    var color = (SDL_Color*)(vtxBase + Marshal.OffsetOf<ImDrawVert>("col"));

                    // Corrected index buffer offset calculation
                    var idxOffsetBytes = pcmd.IdxOffset * sizeof(ushort);
                    var idxData = idxBuffer + idxOffsetBytes;

                    var tex = (SDL_Texture*)pcmd.TextureId;
                    try {
                        renderer.GeometryRaw8BitColor(tex,
                            xy, Unsafe.SizeOf<ImDrawVert>(),
                            (Color*)color, Unsafe.SizeOf<ImDrawVert>(),
                            uv, Unsafe.SizeOf<ImDrawVert>(),
                            (int)(drawList.VtxBuffer.Size - pcmd.VtxOffset),
                            (IntPtr)idxData,
                            (int)pcmd.ElemCount, Unsafe.SizeOf<ushort>());
                    }
                    catch (SdlException e) { }
                }
            }
        }
        platformIo.Renderer_RenderState = 0;
        renderStatePin.Dispose();

        // Restore modified SDL_Renderer state
        renderer.Viewport = old.ViewportEnabled ? old.Viewport : null;
        renderer.ClipRect = old.ClipEnabled ? old.ClipRect : null;
    }

    // Called by Init/NewFrame/Shutdown
    private static void CreateFontsTexture() {
        var io = ImGui.GetIO();

        // Build texture atlas
        // Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small)
        // because it is more likely to be compatible with user's existing shaders. If your
        // ImTextureId represent a higher-level concept than just a GL texture id, consider
        // calling GetTexDataAsAlpha8() instead to save on GPU memory.
        io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height);

        // Upload texture to graphics system
        // (Bilinear sampling is required by default. Set 'io.Fonts->Flags |= ImFontAtlasFlags_NoBakedLines' or 'style.AntiAliasedLinesUseTex = false' to allow point/nearest sampling)
        // fixme: little endian support only
        _data.FontTexture = new Texture(_data.Renderer, PixelFormat.Abgr8888, TextureAccess.SdlTextureaccessStatic, width, height);
        if (_data.FontTexture is null) 
            throw new Exception("error creating texture");
        
        _data.FontTexture.Update((IntPtr)pixels, 4 * width);
        _data.FontTexture.BlendMode = BlendMode.Blend;
        _data.FontTexture.ScaleMode = ScaleMode.Linear;
        // Store our identifier
        io.Fonts.SetTexID((IntPtr)_data.FontTexture.Handle);
    }

    private static void DestroyFontsTexture() {
        var io = ImGui.GetIO();
        
        if (_data.FontTexture is null) return;
        io.Fonts.SetTexID(0);
        _data.FontTexture.Dispose();
        _data.FontTexture = null;
    }

    private static void CreateDeviceObjects() => CreateFontsTexture();

    private static void DestroyDeviceObjects() =>
        DestroyFontsTexture();
}