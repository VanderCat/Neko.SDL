# quick ref
1. Basics - **99%** (All essential stuff done)
2. Video - ~50%
3. Input - Unknown
4. Force Feeback - ~0%
5. Audio - ~0%
6. GPU - ~0%
7. Threads - ~100%
8. Filesystem - ~75%
9. Platform and CPU Information - Unknown
10. Additional Functionality - ~30%

# Curently wrapped functions

- ✅ - Ready to use
- 🚧 - Stub, not implemented
- ❌ - Missing

## Basics

### Application entry points
Limited support, see remarks section of NekoSDL.EnterApp

| Original Function         | Neko.SDL equivalent                  |
|---------------------------|--------------------------------------|
| SDL_AppEvent              | ✅ IApplication.Event                 |
| SDL_AppInit               | ✅ IApplication.Init                  |
| SDL_AppIterate            | ✅ IApplication.Iterate               |
| SDL_AppQuit               | ✅ IApplication.Quit                  |
| SDL_EnterAppMainCallbacks | ✅ NekoSDL.EnterApp                   |
| SDL_GDKSuspendComplete    | ✅ Extra.System.GDK.SuspendComplete   |
| SDL_main                  | ❌ NotSupported                       |
| SDL_RegisterApp           | ✅ Extra.System.Windows.RegisterApp   |
| SDL_RunApp                | ✅ NekoSDL.Run                        |
| SDL_SetMainReady          | ❌ MISSING                            |
| SDL_UnregisterApp         | ✅ Extra.System.Windows.UnregisterApp |


### Initialization and Shutdown

| Original Function          | Neko.SDL equivalent       |
|----------------------------|---------------------------|
| SDL_GetAppMetadataProperty | ✅ AppMetadata             |
| SDL_Init                   | ✅ NekoSDL.Init            |
| SDL_InitSubSystem          | ✅ NekoSDL.InitSubSystem   |
| SDL_IsMainThread           | ✅ NekoSDL.IsMainThread    |
| SDL_Quit                   | ✅ NekoSDL.Quit            |
| SDL_QuitSubSystem          | ✅ NekoSDL.QuitSubSystem   |
| SDL_RunOnMainThread        | ✅ NekoSDL.RunOnMainThread |
| SDL_SetAppMetadata         | ✅ AppMetadata             |
| SDL_SetAppMetadataProperty | ✅ AppMetadata             |
| SDL_WasInit                | ✅ NekoSDL.WasInit         |

### Configuration Variables

| Original Function       | Neko.SDL equivalent    |
|-------------------------|------------------------|
| SDL_AddHintCallback     | ✅ Hints.Hint.Callbacks |
| SDL_GetHint             | ✅ Hints.Hint.GetValue  |
| SDL_GetHintBoolean      | ✅ Hints.Hint.GetBool   |
| SDL_RemoveHintCallback  | ✅ Hints.Hint.Callbacks |
| SDL_ResetHint           | ✅ Hints.Hint.Reset     |
| SDL_ResetHints          | ✅ Hints.ResetAll       |
| SDL_SetHint             | ✅ Hints.Hint.SetValue  |
| SDL_SetHintWithPriority | ✅ Hints.Hint.SetValue  |

### Object Properties

| Original Function                 | Neko.SDL equivalent                 |
|-----------------------------------|-------------------------------------|
| SDL_ClearProperty                 | ✅ Properties.Clear                  |
| SDL_CopyProperties                | ✅ Properties.CopyTo                 |
| SDL_CreateProperties              | ✅ new Properties()                  |
| SDL_DestroyProperties             | ✅ Properties.Dispose                |
| SDL_EnumerateProperties           | ✅ .NET IEnumerable                  |
| SDL_GetBooleanProperty            | ✅ Properties.GetBoolean             |
| SDL_GetFloatProperty              | ✅ Properties.GetFloat               |
| SDL_GetGlobalProperties           | ✅ Properties.Global                 |
| SDL_GetNumberProperty             | ✅ Properties.GetNumber              |
| SDL_GetPointerProperty            | ✅ Properties.GetPointer             |
| SDL_GetPropertyType               | ✅ Properties.GetUnderlyingType      |
| SDL_GetStringProperty             | ✅ Properties.SetString              |
| SDL_HasProperty                   | ✅ Properties.HasProperty            |
| SDL_LockProperties                | ✅ Properties.Lock                   |
| SDL_SetBooleanProperty            | ✅ Properties.SetBoolean             |
| SDL_SetFloatProperty              | ✅ Properties.SetFloat               |
| SDL_SetNumberProperty             | ✅ Properties.SetNumber              |
| SDL_SetPointerProperty            | ✅ Properties.SetPointer             |
| SDL_SetPointerPropertyWithCleanup | ✅ Properties.SetPointerWithCleanup |
| SDL_SetStringProperty             | ✅ Properties.SetString              |
| SDL_UnlockProperties              | ✅ Properties.Unlock                 |

### Error Handling

| Original Function | Neko.SDL equivalent        |
|-------------------|----------------------------|
| SDL_ClearError    | ✅ SdlException.Error       |
| SDL_GetError      | ✅ SdlException.Error       |
| SDL_OutOfMemory   | ✅ SdlException.OutOfMemory |
| SDL_SetError      | ❌ VarArgs Not Supported    |
| SDL_SetErrorV     | ✅ SdlException.Error       |

### Log

| Original Function               | Neko.SDL equivalent              |
|---------------------------------|----------------------------------|
| SDL_GetDefaultLogOutputFunction | ❌ Won't be added                 |
| SDL_GetLogOutputFunction        | ✅ Logging.Log.OutputFunction     |
| SDL_GetLogPriority              | ✅ Logging.Log.Priorities[]       |
| SDL_Log                         | ✅ Logging.Log.LogApp             |
| SDL_LogCritical                 | ✅ Logging.Log.Critical           |
| SDL_LogDebug                    | ✅ Logging.Log.Debug              |
| SDL_LogError                    | ✅ Logging.Log.Error              |
| SDL_LogInfo                     | ✅ Logging.Log.Info               |
| SDL_LogMessage                  | ❌ VarArgs Not Supported          |
| SDL_LogMessageV                 | ✅ Logging.Log.Message            |
| SDL_LogTrace                    | ✅ Logging.Log.Trace              |
| SDL_LogVerbose                  | ✅ Logging.Log.Verbose            |
| SDL_LogWarn                     | ✅ Logging.Log.Warn               |
| SDL_ResetLogPriorities          | ✅ Logging.Log.Priorities.Reset() |
| SDL_SetLogOutputFunction        | ✅ Logging.Log.OutputFunction     |
| SDL_SetLogPriorities            | ✅ Logging.Log.Priorities.Set()   |
| SDL_SetLogPriority              | ✅ Logging.Log.Priorities[]       |
| SDL_SetLogPriorityPrefix        | ✅ Logging.Log.SetPriorityPrefix  |

### Assertions
| Original Function              | Neko.SDL equivalent                         |
|--------------------------------|---------------------------------------------|
| SDL_GetAssertionHandler        | ✅ Diagnostics.Debug.AssertionHandler        |
| SDL_GetAssertionReport         | ✅ Diagnostics.Debug.AssertionReport         |
| SDL_GetDefaultAssertionHandler | ✅ Diagnostics.Debug.DefaultAssertionHandler |
| SDL_ReportAssertion            | ❌ Internal                                  |
| SDL_ResetAssertionReport       | ✅ Diagnostics.Debug.ResetAssertionReport    |
| SDL_SetAssertionHandler        | ✅ Diagnostics.Debug.AssertionHandler        |
| SDL_assert                     | ✅ Diagnostics.Debug.Assert                  |
| SDL_assert_always              | ✅ Diagnostics.Debug.AssertRelease           |

### Querying SDL Version

| Original Function | Neko.SDL equivalent |
|-------------------|---------------------|
| SDL_GetRevision   | ✅ NekoSDL.Revision  |
| SDL_GetVersion    | ✅ NekoSDL.Version   |

## Video

### Display and Window Management

| Original Function                   | Neko.SDL equivalent                                   |
|-------------------------------------|-------------------------------------------------------|
| SDL_CreatePopupWindow               | ❌ MISSING                                             |
| SDL_CreateWindow                    | ✅ Video.Window.Create                                 |
| SDL_CreateWindowWithProperties      | ❌ MISSING                                             |
| SDL_DestroyWindow                   | ✅ Video.Window.Dispose                                |
| SDL_DestroyWindowSurface            | ❌ MISSING                                             |
| SDL_DisableScreenSaver              | ✅ Video.Display.ScreenSaverEnabled                    |
| SDL_EGL_GetCurrentConfig            | ❌ MISSING                                             |
| SDL_EGL_GetCurrentDisplay           | ❌ MISSING                                             |
| SDL_EGL_GetProcAddress              | ❌ MISSING                                             |
| SDL_EGL_GetWindowSurface            | ❌ MISSING                                             |
| SDL_EGL_SetAttributeCallbacks       | ❌ MISSING                                             |
| SDL_EnableScreenSaver               | ✅ Video.Display.ScreenSaverEnabled                    |
| SDL_FlashWindow                     | ✅ Video.Window.Flash()                                |
| SDL_GetClosestFullscreenDisplayMode | ✅ Video.Display.GetClosestFullScreenMode              |
| SDL_GetCurrentDisplayMode           | ✅ Video.Display.GetCurrentMode                        |
| SDL_GetCurrentDisplayOrientation    | ✅ Video.Display.GetCurrentOrientation                 |
| SDL_GetCurrentVideoDriver           | ✅ Video.VideoDriver.Current                           |
| SDL_GetDesktopDisplayMode           | ❌ MISSING                                             |
| SDL_GetDisplayBounds                | ❌ MISSING                                             |
| SDL_GetDisplayContentScale          | ✅ Video.Display.GetContentScale                       |
| SDL_GetDisplayForPoint              | ✅ Video.Display.GetForPoint                           |
| SDL_GetDisplayForRect               | ✅ Video.Display.GetForRect                            |
| SDL_GetDisplayForWindow             | ✅ Video.Window.Display                                |
| SDL_GetDisplayName                  | ✅ Video.Display.GetName                               |
| SDL_GetDisplayProperties            | 🚧 Video.Display.GetProperties                        |
| SDL_GetDisplays                     | ✅ Video.Display.GetIds()                              |
| SDL_GetDisplayUsableBounds          | ✅ Video.Display.GetUsableBounds                       |
| SDL_GetFullscreenDisplayModes       | ✅ Video.Display.GetFullscreenMode                     |
| SDL_GetGrabbedWindow                | ❌ MISSING                                             |
| SDL_GetNaturalDisplayOrientation    | ✅ Video.Display.GetNaturalOrientation                 |
| SDL_GetNumVideoDrivers              | ✅ Video.VideoDriver.Count                             |
| SDL_GetPrimaryDisplay               | ✅ Video.Display.PrimaryDisplay                        |
| SDL_GetSystemTheme                  | ✅ Video.Display.CurrentTheme                          |
| SDL_GetVideoDriver                  | ✅ Video.VideoDriver.Get                               |
| SDL_GetWindowAspectRatio            | ✅ Video.Window.GetAspectRatio                         |
| SDL_GetWindowBordersSize            | ✅ Video.Window.GetBordersSize                         |
| SDL_GetWindowDisplayScale           | ✅ Video.Window.DisplayScale                           |
| SDL_GetWindowFlags                  | ✅ Video.Window.Flags                                  |
| SDL_GetWindowFromID                 | ✅ Video.Window.GetById                                |
| SDL_GetWindowFullscreenMode         | ✅ Video.Window.FullscreenMode                         |
| SDL_GetWindowICCProfile             | 🚧 Video.Window.IccProfile                            |
| SDL_GetWindowID                     | ✅ Video.Window.Id                                     |
| SDL_GetWindowKeyboardGrab           | ✅ Video.Window.KeyboardGrab                           |
| SDL_GetWindowMaximumSize            | ✅ Video.Window.MaximumSize                            |
| SDL_GetWindowMinimumSize            | ✅ Video.Window.MinimumSize                            |
| SDL_GetWindowMouseGrab              | ✅ Video.Window.MouseGrab                              |
| SDL_GetWindowMouseRect              | ✅ Video.Window.MouseRect                              |
| SDL_GetWindowOpacity                | ✅ Video.Window.Opacity                                |
| SDL_GetWindowParent                 | ✅ Video.Window.Parent                                 |
| SDL_GetWindowPixelDensity           | ✅ Video.Window.PixelDensity                           |
| SDL_GetWindowPixelFormat            | ✅ Video.Window.PixelFormat                            |
| SDL_GetWindowPosition               | ✅ Video.Window.Position                               |
| SDL_GetWindowProgressState          | ❌ MISSING                                             |
| SDL_GetWindowProgressValue          | ❌ MISSING                                             |
| SDL_GetWindowProperties             | ✅ Video.Window.Properties                             |
| SDL_GetWindows                      | ✅ Video.Window.GetWindows                             |
| SDL_GetWindowSafeArea               | ✅ Video.Window.SafeArea                               |
| SDL_GetWindowSize                   | ✅ Video.Window.Size                                   |
| SDL_GetWindowSizeInPixels           | ✅ Video.Window.SizeInPixels                           |
| SDL_GetWindowSurface                | 🚧 Video.Window.Surface                               |
| SDL_GetWindowSurfaceVSync           | ✅ Video.Window.SurfaceVSync                           |
| SDL_GetWindowTitle                  | ✅ Video.Window.Title                                  |
| SDL_GL_CreateContext                | ✅ Video.Gl.CreateContext                              |
| SDL_GL_DestroyContext               | ✅ Video.Gl.DestroyContext                             |
| SDL_GL_ExtensionSupported           | ❌ MISSING                                             |
| SDL_GL_GetAttribute                 | ✅ Video.Gl.Attributes[GlAttr]                         |
| SDL_GL_GetCurrentContext            | ✅ Video.Gl.CurrentContext                             |
| SDL_GL_GetCurrentWindow             | ✅ Video.Gl.CurrentWindow                              |
| SDL_GL_GetProcAddress               | ✅ Video.Gl.GetProcAddr                                |
| SDL_GL_GetSwapInterval              | ✅ Video.Gl.SwapInterval                               |
| SDL_GL_LoadLibrary                  | ✅ Video.Gl.LoadLibrary                                |
| SDL_GL_MakeCurrent                  | ✅ Video.Gl.MakeGlCurrent (Video.Window.MakeGlCurrent) |
| SDL_GL_ResetAttributes              | ✅ Video.Gl.Attributes.Reset                           |
| SDL_GL_SetAttribute                 | ✅ Video.Gl.Attributes[GlAttr]                         |
| SDL_GL_SetSwapInterval              | ✅ Video.Gl.SwapInterval                               |
| SDL_GL_SwapWindow                   | ✅ Video.Gl.SwapWindows                                |
| SDL_GL_UnloadLibrary                | ✅ Video.Gl.UnloadLibrary                              |
| SDL_HideWindow                      | ✅ Video.Window.Hide                                   |
| SDL_MaximizeWindow                  | ✅ Video.Window.Maximize                               |
| SDL_MinimizeWindow                  | ✅ Video.Window.Minimize                               |
| SDL_RaiseWindow                     | ✅ Video.Window.Raise                                  |
| SDL_RestoreWindow                   | ✅ Video.Window.Restore                                |
| SDL_ScreenSaverEnabled              | ✅ Video.Display.ScreenSaverEnabled                    |
| SDL_SetWindowAlwaysOnTop            | ✅ Video.Window.AlwaysOnTop                            |
| SDL_SetWindowAspectRatio            | ✅ Video.Window.AspectRatio                            |
| SDL_SetWindowBordered               | ✅ Video.Window.Bordered                               |
| SDL_SetWindowFocusable              | ✅ Video.Window.Focusable                              |
| SDL_SetWindowFullscreen             | ✅ Video.Window.Fullscreen                             |
| SDL_SetWindowFullscreenMode         | ✅ Video.Window.FullscreenMode                         |
| SDL_SetWindowHitTest                | 🚧 Video.Window.SetHitTest                            |
| SDL_SetWindowIcon                   | 🚧 Video.Window.SetIcon                               |
| SDL_SetWindowKeyboardGrab           | ✅ Video.Window.KeyboardGrab                           |
| SDL_SetWindowMaximumSize            | ✅ Video.Window.MaximumSize                            |
| SDL_SetWindowMinimumSize            | ✅ Video.Window.MinimumSize                            |
| SDL_SetWindowModal                  | ✅ Video.Window.Modal                                  |
| SDL_SetWindowMouseGrab              | ✅ Video.Window.MouseGrab                              |
| SDL_SetWindowMouseRect              | ✅ Video.Window.MouseRect                              |
| SDL_SetWindowOpacity                | ✅ Video.Window.Opacity                                |
| SDL_SetWindowParent                 | ✅ Video.Window.Parent                                 |
| SDL_SetWindowPosition               | ✅ Video.Window.Position                               |
| SDL_SetWindowProgressState          | ❌ MISSING                                             |
| SDL_SetWindowProgressValue          | ❌ MISSING                                             |
| SDL_SetWindowResizable              | ✅ Video.Window.Resizable                              |
| SDL_SetWindowShape                  | 🚧 Video.Window.SetShape                              |
| SDL_SetWindowSize                   | ✅ Video.Window.Size                                   |
| SDL_SetWindowSurfaceVSync           | ✅ Video.Window.SurfaceVsync                           |
| SDL_SetWindowTitle                  | ✅ Video.Window.Title                                  |
| SDL_ShowWindow                      | ✅ Video.Window.Show                                   |
| SDL_ShowWindowSystemMenu            | ✅ Video.Window.ShowSystemMenu                         |
| SDL_SyncWindow                      | ✅ Video.Window.Sync                                   |
| SDL_UpdateWindowSurface             | ✅ Video.Window.UpdateWindowSurface                    |
| SDL_UpdateWindowSurfaceRects        | ✅ Video.Window.UpdateWindowSurfaceRects               |
| SDL_WindowHasSurface                | ✅ Video.Window.HasSurface                             |

### 2D Accelerated Rendering

| Original Function                     | Neko.SDL equivalent                              |
|---------------------------------------|--------------------------------------------------|
| SDL_AddVulkanRenderSemaphores         | ✅ Video.Renderer.AddVulkanRenderSemaphores       |
| SDL_ConvertEventToRenderCoordinates   | ✅ Video.Renderer.ConvertEventToRenderCoordinates |
| SDL_CreateGPURenderer                 | ❌ MISSING                                        |
| SDL_CreateGPURenderState              | ❌ MISSING                                        |
| SDL_CreateRenderer                    | ✅ new Renderer()                                 |
| SDL_CreateRendererWithProperties      | ✅ new Renderer()                                 |
| SDL_CreateSoftwareRenderer            | ✅ Video.Renderer.CreateSoftware                  |
| SDL_CreateTexture                     | ✅ Video.Renderer.CreateTexture                   |
| SDL_CreateTextureFromSurface          | ✅ Video.Renderer.CreateTextureFromSurface        |
| SDL_CreateTextureWithProperties       | ✅ Video.Renderer.CreateTextureWithProperties     |
| SDL_CreateWindowAndRenderer           | ✅ Video.Window.CreateWindowAndRenderer           |
| SDL_DestroyGPURenderState             | ❌ MISSING                                        |
| SDL_DestroyRenderer                   | ✅ Video.Renderer.Dispose                         |
| SDL_DestroyTexture                    | ✅ Video.Texture.Dispose                          |
| SDL_FlushRenderer                     | ✅ Video.Renderer.Flush                           |
| SDL_GetCurrentRenderOutputSize        | ✅ Video.Renderer.CurrentRenderOutputSize         |
| SDL_GetDefaultTextureScaleMode        | ❌ MISSING                                        |
| SDL_GetNumRenderDrivers               | ✅ Video.Renderer.DriversCount                    |
| SDL_GetRenderClipRect                 | ✅ Video.Renderer.ClipRect                        |
| SDL_GetRenderColorScale               | ✅ Video.Renderer.ColorScale                      |
| SDL_GetRenderDrawBlendMode            | ✅ Video.Renderer.DrawBlendMode                   |
| SDL_GetRenderDrawColor                | ✅ Video.Renderer.DrawColor                       |
| SDL_GetRenderDrawColorFloat           | ✅ Video.Renderer.DrawColorF                      |
| SDL_GetRenderDriver                   | ✅ Video.Renderer.GetRenderDriver                 |
| SDL_GetRenderer                       | ❌ MISSING                                        |
| SDL_GetRendererFromTexture            | ✅ Video.Texture.Renderer                         |
| SDL_GetRendererName                   | ✅ Video.Renderer.Name                            |
| SDL_GetRendererProperties             | ✅ Video.Renderer.RendererProperties              |
| SDL_GetRenderLogicalPresentation      | ✅ Video.Renderer.GetLogicalPresentation          |
| SDL_GetRenderLogicalPresentationRect  | ✅ Video.Renderer.GetLogicalPresentationRect      |
| SDL_GetRenderMetalCommandEncoder      | ✅ Video.Renderer.MetalCommandEncoder             |
| SDL_GetRenderMetalLayer               | ✅ Video.Renderer.MetalLayer                      |
| SDL_GetRenderOutputSize               | ✅ Video.Renderer.OutputSize                      |
| SDL_GetRenderSafeArea                 | ✅ Video.Renderer.SafeArea                        |
| SDL_GetRenderScale                    | ✅ Video.Renderer.Scale                           |
| SDL_GetRenderTarget                   | ✅ Video.Renderer.Target                          |
| SDL_GetRenderTextureAddressMode       | ❌ MISSING                                        |
| SDL_GetRenderViewport                 | ✅ Video.Renderer.Viewport                        |
| SDL_GetRenderVSync                    | ✅ Video.Renderer.Vsync                           |
| SDL_GetRenderWindow                   | ✅ Video.Renderer.Window                          |
| SDL_GetTextureAlphaMod                | ✅ Video.Texture.Alpha                            |
| SDL_GetTextureAlphaModFloat           | ✅ Video.Texture.AlpaF                            |
| SDL_GetTextureBlendMode               | ✅ Video.Texture.BlendMode                        |
| SDL_GetTextureColorMod                | ✅ Video.Texture.Color                            |
| SDL_GetTextureColorModFloat           | ✅ Video.Texture.ColorF                           |
| SDL_GetTextureProperties              | ✅ Video.Texture.Properties                       |
| SDL_GetTextureScaleMode               | ✅ Video.Texture.ScaleMode                        |
| SDL_GetTextureSize                    | ✅ Video.Texture.Size                             |
| SDL_LockTexture                       | ✅ Video.Texture.Lock                             |
| SDL_LockTextureToSurface              | ✅ Video.Texture.LockToSurface                    |
| SDL_RenderClear                       | ✅ Video.Renderer.Clear                           |
| SDL_RenderClipEnabled                 | ✅ Video.Renderer.ClipEnabled                     |
| SDL_RenderCoordinatesFromWindow       | ✅ Video.Renderer.CoordinatesFromWindow           |
| SDL_RenderCoordinatesToWindow         | ✅ Video.Renderer.CoordinatesToWindow             |
| SDL_RenderDebugText                   | ✅ Video.Renderer.DebugText                       |
| SDL_RenderDebugTextFormat             | ❌ No varargs in .NET                             |
| SDL_RenderFillRect                    | ✅ Video.Renderer.FillRect                        |
| SDL_RenderFillRects                   | ✅ Video.Renderer.FillRects                       |
| SDL_RenderGeometry                    | ✅ Video.Renderer.Geometry                        |
| SDL_RenderGeometryRaw                 | ✅ Video.Renderer.GeometryRaw                     |
| SDL_RenderLine                        | ✅ Video.Renderer.Line                            |
| SDL_RenderLines                       | ✅ Video.Renderer.Lines                           |
| SDL_RenderPoint                       | ✅ Video.Renderer.Point                           |
| SDL_RenderPoints                      | ✅ Video.Renderer.Points                          |
| SDL_RenderPresent                     | ✅ Video.Renderer.Present                         |
| SDL_RenderReadPixels                  | ✅ Video.Renderer.ReadPixels                      |
| SDL_RenderRect                        | ✅ Video.Renderer.Rect                            |
| SDL_RenderRects                       | ✅ Video.Renderer.Rects                           |
| SDL_RenderTexture                     | ✅ Video.Renderer.Texture                         |
| SDL_RenderTexture9Grid                | ✅ Video.Renderer.Texture9Grid                    |
| SDL_RenderTexture9GridTiled           | ❌ MISSING                                        |
| SDL_RenderTextureAffine               | ✅ Video.Renderer.TextureAffine                   |
| SDL_RenderTextureRotated              | ✅ Video.Renderer.TextureRotated                  |
| SDL_RenderTextureTiled                | ✅ Video.Renderer.TextureTiled                    |
| SDL_RenderViewportSet                 | ✅ Video.Renderer.RenderViewportSet               |
| SDL_SetDefaultTextureScaleMode        | ❌ MISSING                                        |
| SDL_SetGPURenderStateFragmentUniforms | ❌ MISSING                                        |
| SDL_SetRenderClipRect                 | ✅ Video.Renderer.ClipRect                        |
| SDL_SetRenderColorScale               | ✅ Video.Renderer.ColorScale                      |
| SDL_SetRenderDrawBlendMode            | ✅ Video.Renderer.DrawBlendMode                   |
| SDL_SetRenderDrawColor                | ✅ Video.Renderer.DrawColor                       |
| SDL_SetRenderDrawColorFloat           | ✅ Video.Renderer.DrawColorF                      |
| SDL_SetRenderGPUState                 | ❌ MISSING                                        |
| SDL_SetRenderLogicalPresentation      | ✅ Video.Renderer.SetRenderLogicalPresentation    |
| SDL_SetRenderScale                    | ✅ Video.Renderer.Scale                           |
| SDL_SetRenderTarget                   | ✅ Video.Renderer.Target                          |
| SDL_SetRenderTextureAddressMode       | ❌ MISSING                                        |
| SDL_SetRenderViewport                 | ✅ Video.Renderer.Viewport                        |
| SDL_SetRenderVSync                    | ✅ Video.Renderer.VSync                           |
| SDL_SetTextureAlphaMod                | ✅ Video.Texture.Alpha                            |
| SDL_SetTextureAlphaModFloat           | ✅ Video.Texture.AlphaF                           |
| SDL_SetTextureBlendMode               | ✅ Video.Texture.BlendMode                        |
| SDL_SetTextureColorMod                | ✅ Video.Texture.Color                            |
| SDL_SetTextureColorModFloat           | ✅ Video.Texture.ColorF                           |
| SDL_SetTextureScaleMode               | ✅ Video.Texture.Scale                            |
| SDL_UnlockTexture                     | ✅ Video.Texture.Unlock                           |
| SDL_UpdateNVTexture                   | 🚧 Video.Texture.UpdateNv                        |
| SDL_UpdateTexture                     | ✅ Video.Renderer.Update                          |
| SDL_UpdateYUVTexture                  | 🚧 Video.Renderer.UpdateYUV                      |

### Pixel Formats and Conversion Routines

| Original Function          | Neko.SDL equivalent                      |
|----------------------------|------------------------------------------|
| SDL_CreatePalette          | ✅ Video.Palette.Create                   |
| SDL_DestroyPalette         | ✅ Video.Palette.Dispose                  |
| SDL_GetMasksForPixelFormat | ✅ Video.PixelFormatExtensions.GetMasks   |
| SDL_GetPixelFormatDetails  | ✅ Video.PixelFormatExtensions.GetDetails |
| SDL_GetPixelFormatForMasks | ✅ Video.PixelFormatExtensions.GetForMask |
| SDL_GetPixelFormatName     | ❌ Use PixelFormat.ToString               |
| SDL_GetRGB                 | ✅ Color.GetRGB                           |
| SDL_GetRGBA                | ✅ Color.GetRGBA                          |
| SDL_MapRGB                 | ❌ MISSING                                |
| SDL_MapRGBA                | ❌ MISSING                                |
| SDL_MapSurfaceRGB          | ❌ MISSING                                |
| SDL_MapSurfaceRGBA         | ❌ MISSING                                |
| SDL_SetPaletteColors       | ✅ Video.Palette.SetColors                |

### Blend modes

| Original Function          | Neko.SDL equivalent |
|----------------------------|---------------------|
| SDL_ComposeCustomBlendMode | ❌ MISSING           |

### Rectangle Functions

| Original Function                   | Neko.SDL equivalent            |
|-------------------------------------|--------------------------------|
| SDL_GetRectAndLineIntersection      | ✅ Rectangle.IntersectLine      |
| SDL_GetRectAndLineIntersectionFloat | ❌ MISSING                      |
| SDL_GetRectEnclosingPoints          | ✅ Rectangle.GetEnclosingPoints |
| SDL_GetRectEnclosingPointsFloat     | ❌ MISSING                      |
| SDL_GetRectIntersection             | ❌ MISSING                      |
| SDL_GetRectIntersectionFloat        | ❌ MISSING                      |
| SDL_GetRectUnion                    | ✅ Rectangle.Union              |
| SDL_GetRectUnionFloat               | ❌ MISSING                      |
| SDL_HasRectIntersection             | ✅ Rectangle.HasIntersection    |
| SDL_HasRectIntersectionFloat        | ❌ MISSING                      |
| SDL_PointInRect                     | ✅ Rectangle.IsPointIn          |
| SDL_PointInRectFloat                | ❌ MISSING                      |
| SDL_RectEmpty                       | ✅ Rectangle.Empty              |
| SDL_RectEmptyFloat                  | ❌ MISSING                      |
| SDL_RectsEqual                      | ✅ Rectangle.Equals             |
| SDL_RectsEqualEpsilon               | ❌ MISSING                      |
| SDL_RectsEqualFloat                 | ❌ MISSING                      |
| SDL_RectToFRect                     | ❌ MISSING                      |

### Surface Creation and Simple Drawing

| Original Function                | Neko.SDL equivalent |
|----------------------------------|---------------------|
| SDL_AddSurfaceAlternateImage     | ❌ MISSING           |
| SDL_BlitSurface                  | ❌ MISSING           |
| SDL_BlitSurface9Grid             | ❌ MISSING           |
| SDL_BlitSurfaceScaled            | ❌ MISSING           |
| SDL_BlitSurfaceTiled             | ❌ MISSING           |
| SDL_BlitSurfaceTiledWithScale    | ❌ MISSING           |
| SDL_BlitSurfaceUnchecked         | ❌ MISSING           |
| SDL_BlitSurfaceUncheckedScaled   | ❌ MISSING           |
| SDL_ClearSurface                 | ❌ MISSING           |
| SDL_ConvertPixels                | ❌ MISSING           |
| SDL_ConvertPixelsAndColorspace   | ❌ MISSING           |
| SDL_ConvertSurface               | ❌ MISSING           |
| SDL_ConvertSurfaceAndColorspace  | ❌ MISSING           |
| SDL_CreateSurface                | ❌ MISSING           |
| SDL_CreateSurfaceFrom            | ❌ MISSING           |
| SDL_CreateSurfacePalette         | ❌ MISSING           |
| SDL_DestroySurface               | ❌ MISSING           |
| SDL_DuplicateSurface             | ❌ MISSING           |
| SDL_FillSurfaceRect              | ❌ MISSING           |
| SDL_FillSurfaceRects             | ❌ MISSING           |
| SDL_FlipSurface                  | ❌ MISSING           |
| SDL_GetSurfaceAlphaMod           | ❌ MISSING           |
| SDL_GetSurfaceBlendMode          | ❌ MISSING           |
| SDL_GetSurfaceClipRect           | ❌ MISSING           |
| SDL_GetSurfaceColorKey           | ❌ MISSING           |
| SDL_GetSurfaceColorMod           | ❌ MISSING           |
| SDL_GetSurfaceColorspace         | ❌ MISSING           |
| SDL_GetSurfaceImages             | ❌ MISSING           |
| SDL_GetSurfacePalette            | ❌ MISSING           |
| SDL_GetSurfaceProperties         | ❌ MISSING           |
| SDL_LoadBMP                      | ❌ MISSING           |
| SDL_LoadBMP_IO                   | ❌ MISSING           |
| SDL_LockSurface                  | ❌ MISSING           |
| SDL_MapSurfaceRGB                | ❌ MISSING           |
| SDL_MapSurfaceRGBA               | ❌ MISSING           |
| SDL_PremultiplyAlpha             | ❌ MISSING           |
| SDL_PremultiplySurfaceAlpha      | ❌ MISSING           |
| SDL_ReadSurfacePixel             | ❌ MISSING           |
| SDL_ReadSurfacePixelFloat        | ❌ MISSING           |
| SDL_RemoveSurfaceAlternateImages | ❌ MISSING           |
| SDL_SaveBMP                      | ❌ MISSING           |
| SDL_SaveBMP_IO                   | ❌ MISSING           |
| SDL_ScaleSurface                 | ❌ MISSING           |
| SDL_SetSurfaceAlphaMod           | ❌ MISSING           |
| SDL_SetSurfaceBlendMode          | ❌ MISSING           |
| SDL_SetSurfaceClipRect           | ❌ MISSING           |
| SDL_SetSurfaceColorKey           | ❌ MISSING           |
| SDL_SetSurfaceColorMod           | ❌ MISSING           |
| SDL_SetSurfaceColorspace         | ❌ MISSING           |
| SDL_SetSurfacePalette            | ❌ MISSING           |
| SDL_SetSurfaceRLE                | ❌ MISSING           |
| SDL_StretchSurface               | ❌ MISSING           |
| SDL_SurfaceHasAlternateImages    | ❌ MISSING           |
| SDL_SurfaceHasColorKey           | ❌ MISSING           |
| SDL_SurfaceHasRLE                | ❌ MISSING           |
| SDL_UnlockSurface                | ❌ MISSING           |
| SDL_WriteSurfacePixel            | ❌ MISSING           |
| SDL_WriteSurfacePixelFloat       | ❌ MISSING           |

### Clipboard Handling

| Original Function           | Neko.SDL equivalent                 |
|-----------------------------|-------------------------------------|
| SDL_ClearClipboardData      | ✅ Clipboard.Clear                   |
| SDL_GetClipboardData        | ✅ Clipboard.GetData                 |
| SDL_GetClipboardMimeTypes   | ✅ Clipboard.GetMimeTypes            |
| SDL_GetClipboardText        | ✅ Clipboard.Text                    |
| SDL_GetPrimarySelectionText | ✅ Clipboard.PrimarySelectionText    |
| SDL_HasClipboardData        | ✅ Clipboard.HasData                 |
| SDL_HasClipboardText        | ✅ Clipboard.HasText                 |
| SDL_HasPrimarySelectionText | ✅ Clipboard.HasPrimarySelectionText |
| SDL_SetClipboardData        | ✅ Clipboard.SetData                 |
| SDL_SetClipboardText        | ✅ Clipboard.Text                    |
| SDL_SetPrimarySelectionText | ✅ Clipboard.PrimarySelectionText    |

### Vulkan Support

| Original Function                   | Neko.SDL equivalent                                          |
|-------------------------------------|--------------------------------------------------------------|
| SDL_Vulkan_CreateSurface            | ✅ Video.Vulkan.CreateVkSurface (Vide.Window.CreateVkSurface) |
| SDL_Vulkan_DestroySurface           | ✅ Video.Vulkan.DestroyVkSurface                              |
| SDL_Vulkan_GetInstanceExtensions    | ✅ Video.Vulkan.GetInstanceExtensions                         |
| SDL_Vulkan_GetPresentationSupport   | ✅ Video.Vulkan.GetPresentationSupport                        |
| SDL_Vulkan_GetVkGetInstanceProcAddr | ✅ Video.Vulkan.GetVkGetInstanceProcAddr                      |
| SDL_Vulkan_LoadLibrary              | ✅ Video.Vulkan.LoadLibrary                                   |
| SDL_Vulkan_UnloadLibrary            | ✅ Video.Vulkan.UnloadLibrary                                 |

### Metal Support
### Camera

## Input Events
### Event Handling
### Keyboard Support
### Keyboard Keycodes
### Keyboard Scancodes
### Mouse Support
### Joystick Support
### Gamepad Support
### Touch Support
### Pen Support
### Sensors
### HIDAPI

## Force Feedback ("Haptic")
### Force Feedback Support

## Audio
### Audio Playback, Recording, and Mixing

## GPU
### 3D Rendering and GPU Compute

## Threads
### Thread Management
Note: probably unnecessary to use this vs native C# threads, be sure to call CleanupTLS manually if use C# threads,
as noted in documentation

| Original Function              | Neko.SDL equivalent           |
|--------------------------------|-------------------------------|
| SDL_CleanupTLS                 | ✅ Threading.Thread.CleanupTLS |
| SDL_CreateThread               | ✅ Threading.Thread.Create     |
| SDL_CreateThreadWithProperties | ✅ Threading.Thread.Create     |
| SDL_DetachThread               | ✅ Threading.Thread.Dispose    |
| SDL_GetCurrentThreadID         | ✅ Threading.Thread.Id         |
| SDL_GetThreadID                | ✅ Threading.Thread.GetId      |
| SDL_GetThreadName              | ✅ Threading.Thread.Name       |
| SDL_GetThreadState             | ✅ Threading.Thread.State      |
| SDL_GetTLS                     | ✅ Threading.Thread.TLS        |
| SDL_SetCurrentThreadPriority   | ✅ Threading.Thread.Priority   |
| SDL_SetTLS                     | ✅ Threading.Thread.TLS        |
| SDL_WaitThread                 | ✅ Threading.Thread.Wait       |

### Thread Synchronization Primitives
| Original Function           | Neko.SDL equivalent                  |
|-----------------------------|--------------------------------------|
| SDL_BroadcastCondition      | ✅ Threading.Condition.Broadcast      |
| SDL_CreateCondition         | ✅ Threading.Condition.Create         |
| SDL_CreateMutex             | ✅ Threading.Mutex.Create             |
| SDL_CreateRWLock            | ✅ Threading.RWLock.Create            |
| SDL_CreateSemaphore         | ✅ Threading.Semaphore.Create         |
| SDL_DestroyCondition        | ✅ Threading.Condition.Dispose        |
| SDL_DestroyMutex            | ✅ Threading.Mutex.Dispose            |
| SDL_DestroyRWLock           | ✅ Threading.RWLock.Dispose           |
| SDL_DestroySemaphore        | ✅ Threading.Semaphore.Dispose        |
| SDL_GetSemaphoreValue       | ✅ Threading.Semaphore.Value          |
| SDL_LockMutex               | ✅ Threading.Mutex.Lock               |
| SDL_LockRWLockForReading    | ✅ Threading.RWLock.LockForReading    |
| SDL_LockRWLockForWriting    | ✅ Threading.RWLock.LockForWriting    |
| SDL_SetInitialized          | ✅ Threading.InitState.SetInitialized |
| SDL_ShouldInit              | ✅ Threading.InitState.ShouldInit     |
| SDL_ShouldQuit              | ✅ Threading.InitState.ShouldQuit     |
| SDL_SignalCondition         | ✅ Threading.Condition.Signal         |
| SDL_SignalSemaphore         | ✅ Threading.Semaphore.Signal         |
| SDL_TryLockMutex            | ✅ Threading.Mutex.TryLock            |
| SDL_TryLockRWLockForReading | ✅ Threading.RWLock.TryLockForReading |
| SDL_TryLockRWLockForWriting | ✅ Threading.RWLock.TryLockForWriting |
| SDL_TryWaitSemaphore        | ✅ Threading.Semaphore.TryWait        |
| SDL_UnlockMutex             | ✅ Threading.Mutex.Unlock             |
| SDL_UnlockRWLock            | ✅ Threading.RWLock.Unlock            |
| SDL_WaitCondition           | ✅ Threading.Condition.Wait           |
| SDL_WaitConditionTimeout    | ✅ Threading.Condition.WaitTimeout    |
| SDL_WaitSemaphore           | ✅ Threading.Semaphore.Wait           |
| SDL_WaitSemaphoreTimeout    | ✅ Threading.Semaphore.WaitTimeout    |

### Atomic Operations
| Original Function                | Neko.SDL equivalent                            |
|----------------------------------|------------------------------------------------|
| SDL_AddAtomicInt                 | ✅ Threading.Atomic.AtomicInt.Add               |
| SDL_CompareAndSwapAtomicInt      | ✅ Threading.Atomic.AtomicInt.CompareAndSwap    |
| SDL_CompareAndSwapAtomicPointer  | ✅ Threading.Atomic.AtomicIntPtr.CompareAndSwap |
| SDL_CompareAndSwapAtomicU32      | ✅ Threading.Atomic.AtomicUint.CompareAndSwap   |
| SDL_GetAtomicInt                 | ✅ Threading.Atomic.AtomicInt.Value             |
| SDL_GetAtomicPointer             | ✅ Threading.Atomic.AtomicIntPtr.Value          |
| SDL_GetAtomicU32                 | ✅ Threading.Atomic.AtomicU32.Value             |
| SDL_LockSpinlock                 | ✅ Threading.Atomic.Spinlock.Lock               |
| SDL_MemoryBarrierAcquireFunction | ✅ Threading.Atomic.MemoryBarrier.Acquire       |
| SDL_MemoryBarrierReleaseFunction | ✅ Threading.Atomic.MemoryBarrier.Release       |
| SDL_SetAtomicInt                 | ✅ Threading.Atomic.AtomicInt.Value             |
| SDL_SetAtomicPointer             | ✅ Threading.Atomic.AtomicIntPtr.Value          |
| SDL_SetAtomicU32                 | ✅ Threading.Atomic.AtomicUint.Value            |
| SDL_TryLockSpinlock              | ✅ Threading.Atomic.Atomic.TryLock              |
| SDL_UnlockSpinlock               | ✅ Threading.Atomic.Atomic.Unlock               |

## File and I/O Abstractions
### Filesystem Access
| Original Function        | Neko.SDL equivalent                        |
|--------------------------|--------------------------------------------|
| SDL_CopyFile             | ✅ Filesystem.Filesystem.CopyFile           |
| SDL_CreateDirectory      | ✅ Filesystem.Filesystem.CreateDirectory    |
| SDL_EnumerateDirectory   | ✅ Filesystem.Filesystem.EnumerateDirectory |
| SDL_GetBasePath          | ✅ Filesystem.Filesystem.BasePath           |
| SDL_GetCurrentDirectory  | ✅ Filesystem.Filesystem.CurrentDirectory   |
| SDL_GetPathInfo          | ✅ Filesystem.Filesystem.GetPathInfo        |
| SDL_GetPrefPath          | ✅ Filesystem.Filesystem.GetPrefPath        |
| SDL_GetUserFolder        | ✅ Filesystem.Filesystem.GetUserFolder      |
| SDL_GlobDirectory        | ✅ Filesystem.Filesystem.GlobDirectory      |
| SDL_RemovePath           | ✅ Filesystem.Filesystem.RemovePath         |
| SDL_RenamePath           | ✅ Filesystem.Filesystem.RenamePath         |

### Storage Abstraction
| Original Function             | Neko.SDL equivalent                      |
|-------------------------------|------------------------------------------|
| SDL_CloseStorage              | ✅ Filesystem.Storage.Dispose             |
| SDL_CopyStorageFile           | ✅ Filesystem.Storage.Copy                |
| SDL_CreateStorageDirectory    | ✅ Filesystem.Storage.CreateDirectory     |
| SDL_EnumerateStorageDirectory | 🚧 Filesystem.Storage.EnumerateDirectory |
| SDL_GetStorageFileSize        | ❌ MISSING                                |
| SDL_GetStoragePathInfo        | ✅ Filesystem.Storage.Info                |
| SDL_GetStorageSpaceRemaining  | ✅ Filesystem.Storage.SpaceRemaining      |
| SDL_GlobStorageDirectory      | ❌ MISSING                                |
| SDL_OpenFileStorage           | ❌ MISSING                                |
| SDL_OpenStorage               | ✅ Filesystem.Storage.Open                |
| SDL_OpenTitleStorage          | ✅ Filesystem.Storage.OpenTitle           |
| SDL_OpenUserStorage           | ✅ Filesystem.Storage.OpenUser            |
| SDL_ReadStorageFile           | ✅ Filesystem.Storage.ReadFile            |
| SDL_RemoveStoragePath         | ✅ Filesystem.Storage.RemovePath          |
| SDL_RenameStoragePath         | ✅ Filesystem.Storage.RenamePath          |
| SDL_StorageReady              | ✅ Filesystem.Storage.IsReady             |
| SDL_WriteStorageFile          | ✅ Filesystem.Storage.WriteFile           |

### I/O Streams
NOTE: this implements stream so you can do any C# stream shenanigans 

| Original Function    | Neko.SDL equivalent              |
|----------------------|----------------------------------|
| SDL_CloseIO          | ✅ IOStream.Close                 |
| SDL_FlushIO          | ✅ IOStream.Flush                 |
| SDL_GetIOProperties  | ✅ IOStream.Properties            |
| SDL_GetIOSize        | ✅ IOStream.Length                |
| SDL_GetIOStatus      | ✅ IOStream.Status                |
| SDL_IOFromConstMem   | 🚧 IOStream.FromConstMem         |
| SDL_IOFromDynamicMem | 🚧 IOStream.FromDynamicMem       |
| SDL_IOFromFile       | 🚧 IOStream.FromFile             |
| SDL_IOFromMem        | ❌ IOStream.FromMem               |
| SDL_IOprintf         | ❌ Not Supported                  |
| SDL_IOvprintf        | ❌ Not Supported                  |
| SDL_LoadFile         | ✅ Filesystem.Filesystem.LoadFile |
| SDL_LoadFile_IO      | ✅ IOStream.LoadFile              |
| SDL_OpenIO           | ✅ IOStream.Open                  |
| SDL_ReadIO           | ✅ IOStream.Read                  |
| SDL_ReadS16BE        | ✅ IOStream.ReadS16BE             |
| SDL_ReadS16LE        | ✅ IOStream.ReadS16LE             |
| SDL_ReadS32BE        | ✅ IOStream.ReadS32BE             |
| SDL_ReadS32LE        | ✅ IOStream.ReadS32LE             |
| SDL_ReadS64BE        | ✅ IOStream.ReadS64BE             |
| SDL_ReadS64LE        | ✅ IOStream.ReadS64LE             |
| SDL_ReadS8           | ✅ IOStream.ReadS8                |
| SDL_ReadU16BE        | ✅ IOStream.ReadU16BE             |
| SDL_ReadU16LE        | ✅ IOStream.ReadU16LE             |
| SDL_ReadU32BE        | ✅ IOStream.ReadU32BE             |
| SDL_ReadU32LE        | ✅ IOStream.ReadU32LE             |
| SDL_ReadU64BE        | ✅ IOStream.ReadU64BE             |
| SDL_ReadU64LE        | ✅ IOStream.ReadU64LE             |
| SDL_ReadU8           | ✅ IOStream.ReadU8                |
| SDL_SaveFile         | ✅ Filesystem.Filesystem.SaveFile |
| SDL_SaveFile_IO      | ✅ IOStream.SaveFile              |
| SDL_SeekIO           | ✅ IOStream.Seek                  |
| SDL_TellIO           | ✅ IOStream.Position              |
| SDL_WriteIO          | ✅ IOStream.Write                 |
| SDL_WriteS16BE       | ✅ IOStream.WriteS16BE            |
| SDL_WriteS16LE       | ✅ IOStream.WriteS16LE            |
| SDL_WriteS32BE       | ✅ IOStream.WriteS32BE            |
| SDL_WriteS32LE       | ✅ IOStream.WriteS32LE            |
| SDL_WriteS64BE       | ✅ IOStream.WriteS64BE            |
| SDL_WriteS64LE       | ✅ IOStream.WriteS64LE            |
| SDL_WriteS8          | ✅ IOStream.WriteS8               |
| SDL_WriteU16BE       | ✅ IOStream.WriteU16BE            |
| SDL_WriteU16LE       | ✅ IOStream.WriteU16LE            |
| SDL_WriteU32BE       | ✅ IOStream.WriteU32BE            |
| SDL_WriteU32LE       | ✅ IOStream.WriteU32LE            |
| SDL_WriteU64BE       | ✅ IOStream.WriteU64BE            |
| SDL_WriteU64LE       | ✅ IOStream.WriteU64LE            |
| SDL_WriteU8          | ✅ IOStream.WriteU8               |
### Async I/O

## Platform and CPU Information
### Platform Detection
### CPU Feature Detection
### Compiler Intrinsics Detection
### Byte Order and Byte Swapping
### Bit Manipulation

## Additional Functionality
### Shared Object/DLL Management
| Original Function  | Neko.SDL equivalent                     |
|--------------------|-----------------------------------------|
| SDL_LoadFunction   | ✅ Extra.NativeSharedObject.Load         |
| SDL_LoadObject     | ✅ Extra.NativeSharedObject.LoadFunction |
| SDL_UnloadObject   | ✅ Extra.NativeSharedObject.Dispose      |

### Process
| Original Function               | Neko.SDL equivalent  |
|---------------------------------|----------------------|
| SDL_CreateProcess               | ✅ Process.Create     |
| SDL_CreateProcessWithProperties | ✅ Process.Create     |
| SDL_DestroyProcess              | ✅ Process.Dispose    |
| SDL_GetProcessInput             | ✅ Process.Input      |
| SDL_GetProcessOutput            | ✅ Process.Output     |
| SDL_GetProcessProperties        | ✅ Process.Properties |
| SDL_KillProcess                 | ✅ Process.Kill       |
| SDL_ReadProcess                 | ✅ Process.Read       |
| SDL_WaitProcess                 | ✅ Process.Wait       |
### Power Management Status
| Original Function                | Neko.SDL equivalent        |
|----------------------------------|----------------------------|
| SDL_GetPowerInfo                 | ✅ BatteryStatus.PowerState |
### Message Boxes
| Original Function        | Neko.SDL equivalent                      |
|--------------------------|------------------------------------------|
| SDL_ShowMessageBox       | ✅ Extra.MessageBox.MessageBox.Show       |
| SDL_ShowSimpleMessageBox | ✅ Extra.MessageBox.MessageBox.ShowSimple |
### File Dialogs
| Original Function                | Neko.SDL equivalent         |
|----------------------------------|-----------------------------|
| SDL_ShowFileDialogWithProperties | ✅ FileDialog.Show           |
| SDL_ShowOpenFileDialog           | ✅ FileDialog.ShowOpen       |
| SDL_ShowOpenFolderDialog         | ✅ FileDialog.ShowOpenFolder |
| SDL_ShowSaveFileDialog           | ✅ FileDialog.ShowSave       |
### System Tray
### Locale Info
### Platform-specific Functionality
| Original Function                        | Neko.SDL equivalent                             |
|------------------------------------------|-------------------------------------------------|
| SDL_GetAndroidActivity                   | ✅ Extra.System.Android.Activity                 |
| SDL_GetAndroidCachePath                  | ✅ Extra.System.Android.CachePath                |
| SDL_GetAndroidExternalStoragePath        | ✅ Extra.System.Android.ExternalStoragePath      |
| SDL_GetAndroidExternalStorageState       | ✅ Extra.System.Android.ExternalStorageState     |
| SDL_GetAndroidInternalStoragePath        | ✅ Extra.System.Android.InternalStoragePath      |
| SDL_GetAndroidJNIEnv                     | ✅ Extra.System.Android.JniEnv                   |
| SDL_GetAndroidSDKVersion                 | ✅ Extra.System.Android.SdkVersion               |
| SDL_GetDeviceFormFactor                  | ❌ Missing                                       |
| SDL_GetDeviceFormFactorName              | ❌ Missing                                       |
| SDL_GetDirect3D9AdapterIndex             | ✅ Extra.System.Direct3D9.GetD3DAdapterIndex     |
| SDL_GetDXGIOutputInfo                    | ✅ Extra.System.Direct3D9.GetDxgiOutputInfo      |
| SDL_GetGDKDefaultUser                    | ✅ Extra.System.GDK.DefaultUser                  |
| SDL_GetGDKTaskQueue                      | ✅ Extra.System.GDK.TaskQueue                    |
| SDL_GetSandbox                           | ✅ Extra.System.OperatingSystemExtra.Sandbox     |
| SDL_IsChromebook                         | ✅ Extra.System.Android.IsChromebook             |
| SDL_IsDeXMode                            | ✅ Extra.System.Android.IsDeXMode                |
| SDL_IsTablet                             | ✅ Extra.System.OperatingSystemExtra.IsTablet    |
| SDL_IsTV                                 | ✅ Extra.System.OperatingSystemExtra.IsTV        |
| SDL_OnApplication*                       | ✅ Extra.System.iOS.OnApplication*               |
| SDL_RequestAndroidPermission             | ✅ Extra.System.Android.RequestPermission        |
| SDL_SendAndroidBackButton                | ✅ Extra.System.Android.SendBackButton           |
| SDL_SendAndroidMessage                   | ✅ Extra.System.Android.SendMessage              |
| SDL_SetiOSAnimationCallback              | ✅ Extra.System.iOS.SetAnimationCallback         |
| SDL_SetiOSEventPump                      | ✅ Extra.System.iOS.SetEventPump                 |
| SDL_SetLinuxThreadPriority               | ✅ Extra.System.Linux.SetThreadPriority          |
| SDL_SetLinuxThreadPriorityAndPolicy      | ✅ Extra.System.Linux.SetThreadPriorityAndPolicy |
| SDL_SetWindowsMessageHook                | ✅ Extra.System.Windows.SetMessageHook           |
| SDL_SetX11EventHook                      | ✅ Extra.System.Linux.SetX11EventHook            |
| SDL_ShowAndroidToast                     | ✅ Extra.System.Android.ShowToast                |

### Standard Library Functionality
### GUIDs
### Miscellaneous