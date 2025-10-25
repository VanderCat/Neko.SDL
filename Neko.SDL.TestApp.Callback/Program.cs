using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Neko.Sdl;
using Neko.Sdl.EntryPoints;
using Neko.Sdl.Events;
using Neko.Sdl.ImGuiBackend;
using Neko.Sdl.Video;
using SDL;

namespace Neko.SDL.TestApp.Callback;

public class Program : IApplication {
    public static void Main(string[] args) => NekoSDL.Run(args, new Program());

    public Window Window;
    public Renderer Renderer;
    
    public bool showDemoWindow = true;
    public bool showAnotherWindow = false;
    public Vector4 clearColor = new Vector4(0.45f, 0.55f, 0.60f, 1.00f);
    public AppResult Event(Event @event) {
        //Log.Verbose(0,$"got event {@event.EventType}");
        if (@event.Type == EventType.Quit)
            return AppResult.Success;
        ImGuiSdl.ProcessEvent(@event);
        return AppResult.Continue;
    }
    public AppResult Init(string[] args) {
        AppMetadata.Set(AppDomain.CurrentDomain.FriendlyName, Assembly.GetEntryAssembly()!.GetName().Version!.ToString(), "nekosdl.testapp.callback");

        NekoSDL.Init(InitFlags.Video);

        const WindowFlags windowFlags = WindowFlags.Opengl | WindowFlags.Resizable | WindowFlags.Hidden;
        Window = Window.Create(1280, 720, "Dear ImGui SDL3+SDL_Renderer example", windowFlags);
        Renderer = Window.CreateRenderer();
        Renderer.VSync = 0;
        Window.Position = new Point((int)SDL3.SDL_WINDOWPOS_CENTERED, (int)SDL3.SDL_WINDOWPOS_CENTERED);
        Window.Show();
        
         // Setup Dear ImGui context
        ImGui.CreateContext();
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;     // Enable Keyboard Controls
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;      // Enable Gamepad Controls

        // Setup Dear ImGui style
        ImGui.StyleColorsDark();
        //ImGui.StyleColorsLight();

        // Setup Platform/Renderer backends
        //ImGuiSdl.InitForSDLRenderer(Window, Renderer);
        ImGuiSdlRenderer.Init(Renderer);

        // Load Fonts
        // - If no fonts are loaded, dear imgui will use the default font. You can also load multiple fonts and use ImGui.PushFont()/PopFont() to select them.
        // - AddFontFromFileTTF() will return the ImFont* so you can store it if you need to select the font among multiple.
        // - If the file cannot be loaded, the function will return a nullptr. Please handle those errors in your application (e.g. use an assertion, or display an error and quit).
        // - The fonts will be rasterized at a given size (w/ oversampling) and stored into a texture when calling ImFontAtlas::Build()/GetTexDataAsXXXX(), which ImGui_ImplXXXX_NewFrame below will call.
        // - Use '#define IMGUI_ENABLE_FREETYPE' in your imconfig file to use Freetype for higher quality font rendering.
        // - Read 'docs/FONTS.md' for more instructions and details.
        // - Remember that in C/C++ if you want to include a backslash \ in a string literal you need to write a double backslash \\ !
        // - Our Emscripten build process allows embedding fonts to be accessible at runtime from the "fonts/" folder. See Makefile.emscripten for details.
        //io.Fonts->AddFontDefault();
        //io.Fonts->AddFontFromFileTTF("c:\\Windows\\Fonts\\segoeui.ttf", 18.0f);
        //io.Fonts->AddFontFromFileTTF("../../misc/fonts/DroidSans.ttf", 16.0f);
        //io.Fonts->AddFontFromFileTTF("../../misc/fonts/Roboto-Medium.ttf", 16.0f);
        //io.Fonts->AddFontFromFileTTF("../../misc/fonts/Cousine-Regular.ttf", 15.0f);
        //ImFont* font = io.Fonts->AddFontFromFileTTF("c:\\Windows\\Fonts\\ArialUni.ttf", 18.0f, nullptr, io.Fonts->GetGlyphRangesJapanese());
        //IM_ASSERT(font != nullptr);
        
        return AppResult.Continue;
    }
    public AppResult Iterate() {
        if (Window.Flags.HasFlag(WindowFlags.Minimized)) {
            Neko.Sdl.Time.Timer.Delay(10);
            return AppResult.Continue;
        }
        // Start the Dear ImGui frame
        ImGuiSdlRenderer.NewFrame();
        try {
            ImGuiSdl.NewFrame();
        }
        catch (Exception e) {
            Log.Critical(0, e.ToString());
        }
        ImGui.NewFrame();

        // 1. Show the big demo window (Most of the sample code is in ImGui.ShowDemoWindow()! You can browse its code to learn more about Dear ImGui!).
        if (showDemoWindow)
            ImGui.ShowDemoWindow(ref showDemoWindow);

        // 2. Show a simple window that we create ourselves. We use a Begin/End pair to create a named window.
        {
            var f = 0.0f;
            var counter = 0;

            ImGui.Begin("Hello, world!");                          // Create a window called "Hello, world!" and append into it.

            ImGui.Text("This is some useful text.");               // Display some text (you can use a format strings too)
            ImGui.Checkbox("Demo Window", ref showDemoWindow);      // Edit bools storing our window open/close state
            if (ImGui.Button("Toggle Vsync")) {
                Debugger.Break();
                var vsync = Renderer.VSync;
                Renderer.VSync = vsync == 0 ? 1 : 0;
            }
            ImGui.Checkbox("Another Window", ref showAnotherWindow);

            ImGui.SliderFloat("float", ref f, 0.0f, 1.0f);            // Edit 1 float using a slider from 0.0f to 1.0f
            ImGui.ColorEdit4("clear color", ref clearColor); // Edit 3 floats representing a color

            if (ImGui.Button("Button"))                            // Buttons return true when clicked (most widgets return true when edited/activated)
                counter++;
            ImGui.SameLine();
            ImGui.Text("counter = ");
            ImGui.SameLine();
            ImGui.Text(counter.ToString());
            ImGui.Text("Running SDL ");
            ImGui.SameLine();
            ImGui.Text(NekoSDL.Version.ToString());
            ImGui.SameLine();
            ImGui.Text("(");
            ImGui.SameLine();
            ImGui.Text(NekoSDL.Revision);
            ImGui.SameLine();
            ImGui.Text(")");
            ImGui.SameLine();
            ImGui.Text("on bindings made for ");
            ImGui.SameLine();
            ImGui.Text(NekoSDL.BindingsVersion.ToString());
            ImGui.SameLine();
            ImGui.Text("(");
            ImGui.SameLine();
            ImGui.Text(NekoSDL.BindingsRevision);
            ImGui.SameLine();
            ImGui.Text(")");
            ImGui.Text("Application average ");
            ImGui.SameLine();
            ImGui.Text((1000.0f / ImGui.GetIO().Framerate).ToString(CultureInfo.InvariantCulture));
            ImGui.SameLine();
            ImGui.Text(" ms/frame (");
            ImGui.SameLine();
            ImGui.Text(ImGui.GetIO().Framerate.ToString(CultureInfo.InvariantCulture));
            ImGui.SameLine();
            ImGui.Text(" FPS)");
            ImGui.End();
        }

        // 3. Show another simple window.
        if (showAnotherWindow) {
            ImGui.Begin("Another Window", ref showAnotherWindow);   // Pass a pointer to our bool variable (the window will have a closing button that will clear the bool when clicked)
            ImGui.Text("Hello from another window!");
            if (ImGui.Button("Close Me"))
                showAnotherWindow = false;
            ImGui.End();
        }

        // Rendering
        ImGui.Render();
        //SDL_RenderSetScale(renderer, io.DisplayFramebufferScale.x, io.DisplayFramebufferScale.y);
        Renderer.DrawColorF = new ColorF(clearColor);
        Renderer.Clear();
        ImGuiSdlRenderer.RenderDrawData(ImGui.GetDrawData(), Renderer);
        Renderer.Present();
        return AppResult.Continue;
    }
    public void Quit(AppResult result) {
        if (result == AppResult.Failure) {
            Log.Critical(0, "App exited with failure");
        }
        ImGuiSdlRenderer.Shutdown();
        ImGuiSdl.Shutdown();
        ImGui.DestroyContext();
    }
}