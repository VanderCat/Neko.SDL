# Curently wrapped functions

- ✅ - Ready to use
- 🚧 - Stub, not implemented
- ❌ - Missing

## Basics

### Application entry points

Not supported.

### Initialization and Shutdown

| Original Function          | Neko.SDL equivalent     |
|----------------------------|-------------------------|
| SDL_GetAppMetadataProperty | ✅ AppMetadata           |
| SDL_Init                   | ✅ NekoSDL.Init          |
| SDL_InitSubSystem          | ✅ NekoSDL.InitSubSystem |
| SDL_IsMainThread           | ❌ Use .NET stuff        |
| SDL_Quit                   | ✅ NekoSDL.Quit          |
| SDL_QuitSubSystem          | ✅ NekoSDL.QuitSubSystem |
| SDL_RunOnMainThread        | ❌ Use .NET stuff        |
| SDL_SetAppMetadata         | ✅ AppMetadata           |
| SDL_SetAppMetadataProperty | ✅ AppMetadata           |
| SDL_WasInit                | ✅ NekoSDL.WasInit       |

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
| SDL_HasProperty                   | ❌ MISSING                           |
| SDL_LockProperties                | ✅ Properties.Lock                   |
| SDL_SetBooleanProperty            | ✅ Properties.SetBoolean             |
| SDL_SetFloatProperty              | ✅ Properties.SetFloat               |
| SDL_SetNumberProperty             | ✅ Properties.SetNumber              |
| SDL_SetPointerProperty            | ✅ Properties.SetPointer             |
| SDL_SetPointerPropertyWithCleanup | 🚧 Properties.SetPointerWithCleanup |
| SDL_SetStringProperty             | ✅ Properties.SetString              |
| SDL_UnlockProperties              | ✅ Properties.Unlock                 |

### Error Handling

| Original Function | Neko.SDL equivalent |
|-------------------|---------------------|
| SDL_ClearError    | ❌ MISSING           |
| SDL_GetError      | new SdlException()  |
| SDL_OutOfMemory   | ❌ MISSING           |
| SDL_SetError      | ❌ MISSING           |
| SDL_SetErrorV     | ❌ MISSING           |

### Log

| Original Function               | Neko.SDL equivalent |
|---------------------------------|---------------------|
| SDL_GetDefaultLogOutputFunction | ❌ MISSING           |
| SDL_GetLogOutputFunction        | ❌ MISSING           |
| SDL_GetLogPriority              | ❌ MISSING           |
| SDL_Log                         | ❌ MISSING           |
| SDL_LogCritical                 | ❌ MISSING           |
| SDL_LogDebug                    | ❌ MISSING           |
| SDL_LogError                    | ❌ MISSING           |
| SDL_LogInfo                     | ❌ MISSING           |
| SDL_LogMessage                  | ❌ MISSING           |
| SDL_LogMessageV                 | ❌ MISSING           |
| SDL_LogTrace                    | ❌ MISSING           |
| SDL_LogVerbose                  | ❌ MISSING           |
| SDL_LogWarn                     | ❌ MISSING           |
| SDL_ResetLogPriorities          | ❌ MISSING           |
| SDL_SetLogOutputFunction        | ❌ MISSING           |
| SDL_SetLogPriorities            | ❌ MISSING           |
| SDL_SetLogPriority              | ❌ MISSING           |
| SDL_SetLogPriorityPrefix        | ❌ MISSING           |

### Assertions

| Original Function              | Neko.SDL equivalent |
|--------------------------------|---------------------|
| SDL_GetAssertionHandler        | ❌ MISSING           |
| SDL_GetAssertionReport         | ❌ MISSING           |
| SDL_GetDefaultAssertionHandler | ❌ MISSING           |
| SDL_ReportAssertion            | ❌ MISSING           |
| SDL_ResetAssertionReport       | ❌ MISSING           |
| SDL_SetAssertionHandler        | ❌ MISSING           |

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

| Original Function          | Neko.SDL equivalent |
|----------------------------|---------------------|
| SDL_CreatePalette          | ❌ MISSING           |
| SDL_DestroyPalette         | ❌ MISSING           |
| SDL_GetMasksForPixelFormat | ❌ MISSING           |
| SDL_GetPixelFormatDetails  | ❌ MISSING           |
| SDL_GetPixelFormatForMasks | ❌ MISSING           |
| SDL_GetPixelFormatName     | ❌ MISSING           |
| SDL_GetRGB                 | ❌ MISSING           |
| SDL_GetRGBA                | ❌ MISSING           |
| SDL_MapRGB                 | ❌ MISSING           |
| SDL_MapRGBA                | ❌ MISSING           |
| SDL_MapSurfaceRGB          | ❌ MISSING           |
| SDL_MapSurfaceRGBA         | ❌ MISSING           |
| SDL_SetPaletteColors       | ❌ MISSING           |

### Blend modes

| Original Function          | Neko.SDL equivalent |
|----------------------------|---------------------|
| SDL_ComposeCustomBlendMode | ❌ MISSING           |

### Rectangle Functions

| Original Function                   | Neko.SDL equivalent |
|-------------------------------------|---------------------|
| SDL_GetRectAndLineIntersection      | ❌ MISSING           |
| SDL_GetRectAndLineIntersectionFloat | ❌ MISSING           |
| SDL_GetRectEnclosingPoints          | ❌ MISSING           |
| SDL_GetRectEnclosingPointsFloat     | ❌ MISSING           |
| SDL_GetRectIntersection             | ❌ MISSING           |
| SDL_GetRectIntersectionFloat        | ❌ MISSING           |
| SDL_GetRectUnion                    | ❌ MISSING           |
| SDL_GetRectUnionFloat               | ❌ MISSING           |
| SDL_HasRectIntersection             | ❌ MISSING           |
| SDL_HasRectIntersectionFloat        | ❌ MISSING           |
| SDL_PointInRect                     | ❌ MISSING           |
| SDL_PointInRectFloat                | ❌ MISSING           |
| SDL_RectEmpty                       | ❌ MISSING           |
| SDL_RectEmptyFloat                  | ❌ MISSING           |
| SDL_RectsEqual                      | ❌ MISSING           |
| SDL_RectsEqualEpsilon               | ❌ MISSING           |
| SDL_RectsEqualFloat                 | ❌ MISSING           |
| SDL_RectToFRect                     | ❌ MISSING           |

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

| Original Function           | Neko.SDL equivalent               |
|-----------------------------|-----------------------------------|
| SDL_ClearClipboardData      | Clipboard.Clear                   |
| SDL_GetClipboardData        | Clipboard.GetData                 |
| SDL_GetClipboardMimeTypes   | Clipboard.GetMimeTypes            |
| SDL_GetClipboardText        | Clipboard.Text                    |
| SDL_GetPrimarySelectionText | Clipboard.PrimarySelectionText    |
| SDL_HasClipboardData        | Clipboard.HasData                 |
| SDL_HasClipboardText        | Clipboard.HasText                 |
| SDL_HasPrimarySelectionText | Clipboard.HasPrimarySelectionText |
| SDL_SetClipboardData        | Clipboard.SetData                 |
| SDL_SetClipboardText        | Clipboard.Text                    |
| SDL_SetPrimarySelectionText | Clipboard.PrimarySelectionText    |

### Vulkan Support

| Original Function                   | Neko.SDL equivalent                 |
|-------------------------------------|-------------------------------------|
| SDL_Vulkan_CreateSurface            | ❌ MISSING                           |
| SDL_Vulkan_DestroySurface           | ❌ MISSING                           |
| SDL_Vulkan_GetInstanceExtensions    | ❌ MISSING                           |
| SDL_Vulkan_GetPresentationSupport   | Video.Vulkan.GetPresentationSupport |
| SDL_Vulkan_GetVkGetInstanceProcAddr | Video.Vulkan.GetInstanceProcAddr    |
| SDL_Vulkan_LoadLibrary              | Video.Vulkan.LoadLibrary            |
| SDL_Vulkan_UnloadLibrary            | Video.Vulkan.UnloadLibrary          |

### Metal Support 

NYI

### Camera

NYI

//TODO: Add more