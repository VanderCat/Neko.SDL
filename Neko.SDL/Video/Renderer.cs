using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neko.Sdl.Video;

public sealed unsafe partial class Renderer : SdlWrapper<SDL_Renderer> {
    public Renderer(Window window, string? name) {
        Handle = SDL_CreateRenderer(window.Handle, name);
        if (Handle is null) throw new SdlException("Failed to create renderer: ");
    }

    public Renderer(Properties properties) {
        Handle = SDL_CreateRendererWithProperties(properties);
        if (Handle is null) throw new SdlException("Failed to create renderer: ");
    }
    
    public void AddVulkanRenderSemaphores(uint waitStageMask, long waitSemaphore, long signalSemaphore) => 
        SDL_AddVulkanRenderSemaphores(Handle, waitStageMask, waitSemaphore, signalSemaphore);
    public void ConvertEventToRenderCoordinates(SDL_Event* @event) => SDL_ConvertEventToRenderCoordinates(Handle, @event);
    
    public static Renderer CreateSoftware(Surface surface) => SDL_CreateSoftwareRenderer(surface);
    
    internal SDL_Texture* CreateTexture(PixelFormat format, TextureAccess access, int width, int height) => 
        SDL_CreateTexture(Handle, (SDL_PixelFormat)format, (SDL_TextureAccess)access, width, height);
    internal SDL_Texture* CreateTextureFromSurface(Surface surface) => 
        SDL_CreateTextureFromSurface(Handle, surface);
    
    internal SDL_Texture* CreateTextureWithProperties(Properties properties) => 
        SDL_CreateTextureWithProperties(Handle, (SDL_PropertiesID)properties.Id);

    public override void Dispose() {
        Destroy();
        base.Dispose();
    }
    internal void Destroy() => SDL_DestroyRenderer(Handle);
    
    public void Flush() => SDL_FlushRenderer(Handle).ThrowIfError();

    public Size CurrentRenderOutputSize {
        get {
            var w = 0;
            var h = 0;
            SDL_GetCurrentRenderOutputSize(Handle, &w, &w).ThrowIfError();
            return new Size(w, h);
        }
    }
    public static int DriversCount => SDL_GetNumRenderDrivers();

    [NotNull]
    public Rectangle? ClipRect {
        get {
            var rect = new Rectangle();
            SDL_GetRenderClipRect(Handle, (SDL_Rect*)&rect).ThrowIfError();
            return rect;
        }
        set {
            if (value is not null) {
                var rect = value.Value;
                SDL_SetRenderClipRect(Handle, (SDL_Rect*)&rect).ThrowIfError();
                return;
            }
            SDL_SetRenderClipRect(Handle, null).ThrowIfError();
        }
    }

    public float ColorScale {
        get {
            var f = 0f;
            SDL_GetRenderColorScale(Handle, &f).ThrowIfError();
            return f;
        }
        set => SDL_SetRenderColorScale(Handle, value).ThrowIfError();
    }

    public BlendMode DrawBlendMode {
        get {
            BlendMode f = default;
            SDL_GetRenderDrawBlendMode(Handle, (SDL_BlendMode*)&f).ThrowIfError();
            return f;
        }
        set => SDL_SetRenderDrawBlendMode(Handle, (SDL_BlendMode)value).ThrowIfError();
    }

    public Color DrawColor {
        get {
            var a = new Color();
            SDL_GetRenderDrawColor(Handle, &a.R, &a.G, &a.B, &a.A).ThrowIfError();
            return a;
        }
        set => SDL_SetRenderDrawColor(Handle, value.R, value.G, value.B, value.A).ThrowIfError();
    }
    public ColorF DrawColorF {
        get {
            var a = new ColorF();
            SDL_GetRenderDrawColorFloat(Handle, &a.R, &a.G, &a.B, &a.A).ThrowIfError();
            return a;
        }
        set => SDL_SetRenderDrawColorFloat(Handle, value.R, value.G, value.B, value.A).ThrowIfError();
    }

    public static string GetRenderDriver(int index) {
        var str = SDL_GetRenderDriver(index);
        if (str is null) throw new SdlException("");
        return str;
    }

    internal static Renderer GetRendererFromTexture(Texture texture) {
        var ptr = SDL_GetRendererFromTexture(texture);
        if (ptr is null) throw new SdlException("");
        return ptr;
    }

    public string Name {
        get {
            var str = SDL_GetRendererName(Handle);
            if (str is null) throw new SdlException("");
            return str;
        }
    }
    
    public Properties RendererProperties() => (Properties)SDL_GetRendererProperties(Handle);

    public Size GetLogicalPresentation(RendererLogicalPresentation rendererLogicalPresentation) {
        var size = new Size();
        SDL_GetRenderLogicalPresentation(Handle, 
            (int*)&size, 
            (int*)(&size+Unsafe.SizeOf<int>()), 
            (SDL_RendererLogicalPresentation*)&rendererLogicalPresentation).ThrowIfError();
        return size;
    }
    
    public RectangleF GetLogicalPresentationRect() {
        var rect = new RectangleF();
        SDL_GetRenderLogicalPresentationRect(Handle, 
            (SDL_FRect*)&rect).ThrowIfError();
        return rect;
    }

    public IntPtr MetalCommandEncoder {
        get {
            var ptr = SDL_GetRenderMetalCommandEncoder(Handle);
            if (ptr == 0) throw new SdlException("");
            return ptr;
        }
    }

    public IntPtr MetalLayer {
        get {
            var ptr = SDL_GetRenderMetalLayer(Handle);
            if (ptr == 0) throw new SdlException("");
            return ptr;
        }
    }

    public Size OutputSize {
        get {
            var size = new Size();
            SDL_GetRenderOutputSize(Handle, 
                (int*)&size, 
                (int*)(&size+Unsafe.SizeOf<int>())).ThrowIfError();
            return size;
        }
    }

    public Rectangle SafeArea {
        get {
            var rect = new Rectangle();
            SDL_GetRenderSafeArea(Handle, 
                (SDL_Rect*)&rect).ThrowIfError();
            return rect;
        }
    }

    public Vector2 Scale {
        get {
            var vector = new Vector2();
            SDL_GetRenderScale(Handle, &vector.X,&vector.Y).ThrowIfError();
            return vector;
        }
        set => SDL_SetRenderScale(Handle, value.X, value.Y);
    }

    public Texture? Target {
        get {
            var target =  SDL_GetRenderTarget(Handle);
            if (target is null) return null;
            return new Texture(target);
        }
        set => SDL_SetRenderTarget(Handle, value is null ? null : value.Handle);
    }
    
    [NotNull]
    public Rectangle? Viewport {
        get {
            var rect = new Rectangle();
            SDL_GetRenderViewport(Handle, (SDL_Rect*)&rect).ThrowIfError();
            return rect;
        }
        set {
            if (value is not null) {
                var rect = value.Value;
                SDL_SetRenderViewport(Handle, (SDL_Rect*)&rect).ThrowIfError();
                return;
            }
            SDL_SetRenderViewport(Handle, null).ThrowIfError();
        }
    }
    
    public int VSync {
        get {
            var value = 0;
            SDL_GetRenderVSync(Handle, &value).ThrowIfError();
            return value;
        }
        set => SDL_SetRenderVSync(Handle, value).ThrowIfError();
    }

    public Window Window {
        get {
            var ptr = SDL_GetRenderWindow(Handle);
            if (ptr is null) throw new SdlException("");
            return ptr;
        }
    }
    public void Clear() => SDL_RenderClear(Handle).ThrowIfError();
    public bool ClipEnabled => SDL_RenderClipEnabled(Handle);

    public Vector2 CoordinatesFromWindow(Vector2 position) {
        var windowPos = new Vector2();
        SDL_RenderCoordinatesFromWindow(Handle, 
            position.X, 
            position.Y, 
            &windowPos.X, 
            &windowPos.Y).ThrowIfError();
        return windowPos;
    }
    
    public Vector2 CoordinatesToWindow(Vector2 position) {
        var windowPos = new Vector2();
        SDL_RenderCoordinatesToWindow(Handle, 
            position.X, 
            position.Y, 
            &windowPos.X, 
            &windowPos.Y).ThrowIfError();
        return windowPos;
    }
    
    public void DebugText(string text, float x = 0f, float y = 0f) => SDL_RenderDebugText(Handle, x, y, text);
    public void DebugText(string text, Vector2 position) => DebugText(text, position.X, position.Y);

    public void FillRect(RectangleF rect) => //TODO: will it work?
        SDL_RenderFillRect(Handle, (SDL_FRect*)&rect).ThrowIfError();


    public void FillRects(RectangleF[] rect) {
        fixed (RectangleF* rectptr = rect)
            SDL_RenderFillRects(Handle, (SDL_FRect*)rectptr, rect.Length).ThrowIfError();
    }


    public void Geometry(Texture texture, Vertex[] vertex, int[]? indices = null) {
        fixed(Vertex* vertexPtr = vertex)
        fixed (int* indicesPtr = indices)
            SDL_RenderGeometry(Handle, texture, (SDL_Vertex*)vertexPtr, vertex.Length, indicesPtr, indices?.Length??0).ThrowIfError();
    }
    public void GeometryRaw(Texture texture,  float* xy, 
        int xyStride, 
        ColorF* color, 
        int colorStride, 
        float* uv, 
        int uvStride, 
        int numVertices, 
        nint indices, 
        int numIndices, 
        int sizeIndices) {
        SDL_RenderGeometryRaw(Handle, texture, 
            xy, xyStride, 
            (SDL_FColor*)color, colorStride, 
            uv, uvStride, 
            numVertices, 
            indices, numIndices, sizeIndices).ThrowIfError();
    }
    
    public void GeometryRaw(Texture texture,  float* xy, 
        int xyStride, 
        ColorF* color, 
        int colorStride, 
        float* uv, 
        int uvStride, 
        int numVertices, 
        int[]? indices = null) {
        fixed(int* indicesPtr = indices)
            GeometryRaw(texture, xy, xyStride, color, colorStride, uv, uvStride, numVertices, (IntPtr)indicesPtr, indices?.Length??0, 4);
    }
    
    public void GeometryRaw(Texture texture,  float* xy, 
        int xyStride, 
        ColorF* color, 
        int colorStride, 
        float* uv, 
        int uvStride, 
        int numVertices, 
        byte[]? indices = null) {
        fixed(byte* indicesPtr = indices)
            GeometryRaw(texture, xy, xyStride, color, colorStride, uv, uvStride, numVertices, (IntPtr)indicesPtr, indices?.Length??0, 1);
    }
    
    public void GeometryRaw(Texture texture,  float* xy, 
        int xyStride, 
        ColorF* color, 
        int colorStride, 
        float* uv, 
        int uvStride, 
        int numVertices, 
        short[]? indices = null) {
        fixed(short* indicesPtr = indices)
            GeometryRaw(texture, xy, xyStride, color, colorStride, uv, uvStride, numVertices, (IntPtr)indicesPtr, indices?.Length??0, 2);
    }
    
    public void Line(float x1, float y1, float x2, float y2) => SDL_RenderLine(Handle, x1, y1, x2, y2);
    public void Line(Vector2 start, Vector2 end) => Line(start.X, start.Y, end.X, end.Y);

    public void Lines(Vector2[] points) {
        fixed (Vector2* pointsptr = points)
            SDL_RenderLines(Handle, (SDL_FPoint*)pointsptr, points.Length);
    }
    
    public void Point(float x, float y) => SDL_RenderPoint(Handle, x, y);
    public void Point(Vector2 point) => Point(point.X, point.Y);

    public void Points(Vector2[] points) {
        fixed (Vector2* pointsptr = points)
            SDL_RenderPoints(Handle, (SDL_FPoint*)pointsptr, points.Length);
    }
    
    public void Present() => SDL_RenderPresent(Handle);
    public SDL_Surface* ReadPixels(Rectangle rect) => SDL_RenderReadPixels(Handle, (SDL_Rect*)&rect);
    public void Rect(RectangleF rect) => SDL_RenderRect(Handle, (SDL_FRect*)&rect);

    public void Rects(RectangleF[] rects) {
        fixed (RectangleF* rectptr = rects)
            SDL_RenderRects(Handle, (SDL_FRect*)rectptr, rects.Length);
    }
    
    public void Texture(SDL_Texture* texture, RectangleF src, RectangleF dst) => 
        SDL_RenderTexture(Handle, texture, (SDL_FRect*)&src, (SDL_FRect*)&dst);
    
    public void Texture9Grid(SDL_Texture* texture, 
        RectangleF src, 
        float leftWidth, 
        float rightWidth, 
        float topHeight, 
        float bottomHeight, 
        float scale, 
        RectangleF dst) => 
        SDL_RenderTexture9Grid(Handle, 
            texture, 
            (SDL_FRect*)&src, 
            leftWidth, 
            rightWidth, 
            topHeight, 
            bottomHeight, 
            scale, 
            (SDL_FRect*)&dst);
    
    public void TextureAffine(SDL_Texture* texture, 
        RectangleF src, 
        Vector2 origin, 
        Vector2 right, 
        Vector2 down) => 
        SDL_RenderTextureAffine(Handle, 
            texture, 
            (SDL_FRect*)&src, 
            (SDL_FPoint*)&origin, 
            (SDL_FPoint*)&right, 
            (SDL_FPoint*)&down);
    
    public void TextureRotated(SDL_Texture* texture, 
        RectangleF src, 
        RectangleF dst, 
        double angle, 
        Vector2 center, 
        FlipMode flipMode) => 
        SDL_RenderTextureRotated(Handle, 
            texture, 
            (SDL_FRect*)&src, 
            (SDL_FRect*)&dst, 
            angle, 
            (SDL_FPoint*)&center, 
            (SDL_FlipMode)flipMode);
    
    public void TextureTiled(SDL_Texture* texture, 
        RectangleF src,
        float scale,
        RectangleF dst) => 
        SDL_RenderTextureTiled(Handle, 
        texture, 
        (SDL_FRect*)&src, 
        scale, 
        (SDL_FRect*)&dst);
    
    public bool RenderViewportSet() => SDL_RenderViewportSet(Handle);
    
    public void SetRenderLogicalPresentation(Size size, RendererLogicalPresentation rendererLogicalPresentation) => 
        SDL_SetRenderLogicalPresentation(Handle, size.Width, size.Height, (SDL_RendererLogicalPresentation)rendererLogicalPresentation);
}