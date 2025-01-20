// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Neko.Sdl.ImGuiBackend;
using Neko.Sdl.Video;
using SDL;

namespace Neko.Sdl.Sample;

internal class Program {
    public static unsafe void Main(string[] args) {
        // Setup SDL
        NekoSDL.Init(InitFlags.Video | InitFlags.Gamepad);

        // Create window with SDL_Renderer graphics context
        const WindowFlags windowFlags = WindowFlags.Opengl | WindowFlags.Resizable | WindowFlags.Hidden;
        var window = new Window(1280, 720, "Dear ImGui SDL3+SDL_Renderer example", windowFlags);
        var renderer = window.CreateRenderer();
        renderer.VSync = 1;
        window.Position = new Point((int)SDL3.SDL_WINDOWPOS_CENTERED, (int)SDL3.SDL_WINDOWPOS_CENTERED);
        window.Show();

        // Setup Dear ImGui context
        ImGui.CreateContext();
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;     // Enable Keyboard Controls
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;      // Enable Gamepad Controls

        // Setup Dear ImGui style
        ImGui.StyleColorsDark();
        //ImGui.StyleColorsLight();

        // Setup Platform/Renderer backends
        ImGuiSdl.InitForSDLRenderer(window, renderer);
        ImGuiSdlRenderer.Init(renderer);

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

        // Our state
        var showDemoWindow = true;
        var showAnotherWindow = false;
        var clearColor = new Vector4(0.45f, 0.55f, 0.60f, 1.00f);

        // Main loop
        bool done = false;
        while (!done) {
            // Poll and handle events (inputs, window resize, etc.)
            // You can read the io.WantCaptureMouse, io.WantCaptureKeyboard flags to tell if dear imgui wants to use your inputs.
            // - When io.WantCaptureMouse is true, do not dispatch mouse input data to your main application, or clear/overwrite your copy of the mouse data.
            // - When io.WantCaptureKeyboard is true, do not dispatch keyboard input data to your main application, or clear/overwrite your copy of the keyboard data.
            // Generally you may always pass all inputs to dear imgui, and hide them from your application based on those two flags.
            SDL_Event e;
            while (SDL3.SDL_PollEvent(&e)) {
                ImGuiSdl.ProcessEvent(&e);
                if (e.Type == SDL_EventType.SDL_EVENT_QUIT)
                    done = true;
                if (e.Type == SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED && e.window.windowID == (SDL_WindowID)window.Id)
                    done = true;
            }
            if (window.Flags.HasFlag(WindowFlags.Minimized)) {
                Neko.Sdl.Time.Timer.Delay(10);
                continue;
            }

            // Start the Dear ImGui frame
            ImGuiSdlRenderer.NewFrame();
            ImGuiSdl.NewFrame();
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
                ImGui.Checkbox("Another Window", ref showAnotherWindow);

                ImGui.SliderFloat("float", ref f, 0.0f, 1.0f);            // Edit 1 float using a slider from 0.0f to 1.0f
                ImGui.ColorEdit4("clear color", ref clearColor); // Edit 3 floats representing a color

                if (ImGui.Button("Button"))                            // Buttons return true when clicked (most widgets return true when edited/activated)
                    counter++;
                ImGui.SameLine();
                ImGui.Text($"counter = {counter}");

                ImGui.Text($"Application average {1000.0f / io.Framerate:F3} ms/frame ({io.Framerate:F1} FPS)");
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
            renderer.DrawColorF = new ColorF(clearColor);
            renderer.Clear();
            ImGuiSdlRenderer.RenderDrawData(ImGui.GetDrawData(), renderer);
            renderer.Present();
        }

        // Cleanup
        ImGuiSdlRenderer.Shutdown();
        ImGuiSdl.Shutdown();
        ImGui.DestroyContext();

        renderer.Dispose();
        window.Dispose();
        NekoSDL.Quit();
    }
}