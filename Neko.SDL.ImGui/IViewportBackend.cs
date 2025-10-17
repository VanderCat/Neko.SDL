using System.Numerics;
using ImGuiNET;

namespace Neko.Sdl.ImGuiBackend;

public interface IViewportBackend : IDisposable  {
    public void CreateWindow(ImGuiViewportPtr viewport);
    public void DestroyWindow(ImGuiViewportPtr viewport);
    public void ShowWindow(ImGuiViewportPtr viewport);
    public void UpdateWindow(ImGuiViewportPtr viewport);

    public void SetWindowPos(ImGuiViewportPtr viewport, Vector2 pos);

    public Vector2 GetWindowPos(ImGuiViewportPtr viewport);

    public void SetWindowSize(ImGuiViewportPtr viewport, Vector2 size);

    public Vector2 GetWindowSize(ImGuiViewportPtr viewport);

    public void SetWindowFocus(ImGuiViewportPtr viewport, bool focus);

    public bool GetWindowFocus(ImGuiViewportPtr viewport);

    public bool GetWindowMinimized(ImGuiViewportPtr viewport);
    public void SetWindowTitle(ImGuiViewportPtr viewport, string title);

    public void SetWindowAlpha(ImGuiViewportPtr viewport, float alpha);

    public void RenderWindow(ImGuiViewportPtr viewport, nint unused);

    public void SwapBuffers(ImGuiViewportPtr viewport, nint unused);
    public float GetWindowDpiScale(ImGuiViewportPtr viewport);

    public void OnChangedViewport(ImGuiViewportPtr viewport);
    public Vector2 GetWindowWorkAreaInsets(ImGuiViewportPtr viewport);

    public int CreateVkSurface(ImGuiViewportPtr viewport, IntPtr vkInstance, IntPtr vkAllocator,
        ref IntPtr outVkSurface);
}