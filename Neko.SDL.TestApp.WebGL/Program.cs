using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using ImGuiNET;
using Neko.Sdl;
using Neko.Sdl.ImGuiBackend;
using Neko.Sdl.Video;
using SDL;

[assembly: SupportedOSPlatform("browser")]

namespace Neko.SDL.TestApp.WebGL;

public static partial class Program {
	public static Uri? BaseAddress { get; internal set; }

	public static bool ShowDemoWindow;
	public static bool ShowAnotherWindow;
	public static Vector4 ClearColor = new (0.45f, 0.55f, 0.60f, 1.00f);
	
	[JSExport]
	public static unsafe bool Frame() {
		// Poll and handle events (inputs, window resize, etc.)
        // You can read the io.WantCaptureMouse, io.WantCaptureKeyboard flags to tell if dear imgui wants to use your inputs.
        // - When io.WantCaptureMouse is true, do not dispatch mouse input data to your main application, or clear/overwrite your copy of the mouse data.
        // - When io.WantCaptureKeyboard is true, do not dispatch keyboard input data to your main application, or clear/overwrite your copy of the keyboard data.
        // Generally you may always pass all inputs to dear imgui, and hide them from your application based on those two flags.
        SDL_Event e;
        while (SDL3.SDL_PollEvent(&e)) {
            ImGuiSdl.ProcessEvent(&e);
            if (e.Type == SDL_EventType.SDL_EVENT_QUIT)
                return false;
            if (e.Type == SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED && e.window.windowID == (SDL_WindowID)Window.Id)
                return false;
        }

        // Start the Dear ImGui frame
        ImGuiSdlRenderer.NewFrame();
        ImGuiSdl.NewFrame();
        ImGui.NewFrame();

        // 1. Show the big demo window (Most of the sample code is in ImGui.ShowDemoWindow()! You can browse its code to learn more about Dear ImGui!).
        if (ShowDemoWindow)
            ImGui.ShowDemoWindow(ref ShowDemoWindow);

        // 2. Show a simple window that we create ourselves. We use a Begin/End pair to create a named window.
        {
            var f = 0.0f;
            var counter = 0;

            ImGui.Begin("Hello, world!");                          // Create a window called "Hello, world!" and append into it.

            ImGui.Text("This is some useful text.");               // Display some text (you can use a format strings too)
            ImGui.Checkbox("Demo Window", ref ShowDemoWindow);      // Edit bools storing our window open/close state
            ImGui.Checkbox("Another Window", ref ShowDemoWindow);

            ImGui.SliderFloat("float", ref f, 0.0f, 1.0f);            // Edit 1 float using a slider from 0.0f to 1.0f
            ImGui.ColorEdit4("clear color", ref ClearColor); // Edit 3 floats representing a color

            if (ImGui.Button("Button"))                            // Buttons return true when clicked (most widgets return true when edited/activated)
                counter++;
            ImGui.SameLine();
            ImGui.Text($"counter = {counter}");
            ImGui.Text($"Running SDL {NekoSDL.Version} ({NekoSDL.Revision}) on bindings made for {NekoSDL.BindingsVersion} ({NekoSDL.BindingsRevision})");
            var io = ImGui.GetIO();
            ImGui.Text($"Application average {1000.0f / io.Framerate:F3} ms/frame ({io.Framerate:F1} FPS)");
            ImGui.End();
        }

        // 3. Show another simple window.
        if (ShowAnotherWindow) {
            ImGui.Begin("Another Window", ref ShowAnotherWindow);   // Pass a pointer to our bool variable (the window will have a closing button that will clear the bool when clicked)
            ImGui.Text("Hello from another window!");
            if (ImGui.Button("Close Me"))
                ShowAnotherWindow = false;
            ImGui.End();
        }

        // Rendering
        ImGui.Render();
        //SDL_RenderSetScale(renderer, io.DisplayFramebufferScale.x, io.DisplayFramebufferScale.y);
        Renderer.DrawColorF = new ColorF(ClearColor);
        Renderer.Clear();
        ImGuiSdlRenderer.RenderDrawData(ImGui.GetDrawData(), Renderer);
        Renderer.Present();
        return true;
	}

	[JSExport]
	public static void Shutdown() {
		ImGuiSdlRenderer.Shutdown();
		ImGuiSdl.Shutdown();
		ImGui.DestroyContext();

		Renderer.Dispose();
		Window.Dispose();
		NekoSDL.Quit();
	}

	public static Renderer Renderer;
	public static Window Window;

	public static void Main(string[] args) {
		Console.WriteLine($"Hello from dotnet!");
		CreateCanvas();
		
		try {
			NekoSDL.Init(InitFlags.Video);
		}
		catch (Exception e) {
			Console.WriteLine(e);
			return;
		}
		Console.WriteLine($"sdl {NekoSDL.Version} successfully initialized!");
		try {
			Window.CreateWindowAndRenderer(1280, 720, "test", WindowFlags.Opengl, out Window, out Renderer);
		}
		catch (Exception e) {
			Console.WriteLine(e);
			return;
		}
		// Setup Dear ImGui context
		ImGui.CreateContext();
		var io = ImGui.GetIO();
		io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;     // Enable Keyboard Controls
		io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;      // Enable Gamepad Controls

		// Setup Dear ImGui style
		ImGui.StyleColorsDark();
		//ImGui.StyleColorsLight();

		// Setup Platform/Renderer backends
		ImGuiSdl.InitForSDLRenderer(Window, Renderer);
		ImGuiSdlRenderer.Init(Renderer);
		
		Init();
	}
	
	[JSImport("globalThis.init")]
	public static partial void Init();

	[JSImport("globalThis.createCanvas")]
	public static partial void CreateCanvas();
}