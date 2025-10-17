using Neko.Sdl.Video;

namespace Neko.Sdl.ImGuiBackend;

internal class ViewportData {
    public Window?     Window;
    public Window?     ParentWindow;
    public UInt32          WindowID; // Stored in ImGuiViewport::PlatformHandle. Use SDL_GetWindowFromID() to get SDL_Window* from Uint32 WindowID.
    public bool            WindowOwned;
    public IntPtr GLContext;

    public ViewportData() {
        Window = ParentWindow = null; 
        WindowID = 0; 
        WindowOwned = false; 
        GLContext = 0;
    }

    //TODO: do we really need that?
    // ~ViewportData() {
    //     IM_ASSERT(Window == null && GLContext == null);
    // }
}