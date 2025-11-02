using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.CodeGen;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

/// <summary>
/// The class for all events in SDL
/// </summary>
/// <remarks>
/// The <see cref="Event"/> class is the core of all event handling in SDL.
/// </remarks>
public abstract unsafe class Event {
      protected Event() {}
      public Event(ref SDL_Event @event) {
            _backingStruct = @event;
      }
      
      internal SDL_Event _backingStruct;

      public static Event Create(SDL_Event* @event) => Create(ref Unsafe.AsRef<SDL_Event>(@event));
      public static Event Create(ref SDL_Event @event) {
            switch ((EventType)@event.type) {
                  case EventType.First:
                  case EventType.Terminating:
                  case EventType.LowMemory:
                  case EventType.WillEnterBackground:
                  case EventType.DidEnterBackground:
                  case EventType.WillEnterForeground:
                  case EventType.DidEnterForeground:
                  case EventType.LocaleChanged:
                  case EventType.SystemThemeChanged:
                        return new CommonEvent(ref @event);
                  case EventType.Quit:
                        return new QuitEvent(ref @event);
                  case EventType.DisplayOrientation:
                  case EventType.DisplayAdded:
                  case EventType.DisplayRemoved:
                  case EventType.DisplayMoved:
                  case EventType.DisplayDesktopModeChanged:
                  case EventType.DisplayCurrentModeChanged:
                  case EventType.DisplayContentScaleChanged:
                        return new DisplayEvent(ref @event);
                  case EventType.WindowShown:
                  case EventType.WindowHidden:
                  case EventType.WindowExposed:
                  case EventType.WindowMoved:
                  case EventType.WindowResized:
                  case EventType.WindowPixelSizeChanged:
                  case EventType.WindowMetalViewResized:
                  case EventType.WindowMinimized:
                  case EventType.WindowMaximized:
                  case EventType.WindowRestored:
                  case EventType.WindowMouseEnter:
                  case EventType.WindowMouseLeave:
                  case EventType.WindowFocusGained:
                  case EventType.WindowFocusLost:
                  case EventType.WindowCloseRequested:
                  case EventType.WindowHitTest:
                  case EventType.WindowIccprofChanged:
                  case EventType.WindowDisplayChanged:
                  case EventType.WindowDisplayScaleChanged:
                  case EventType.WindowSafeAreaChanged:
                  case EventType.WindowOccluded:
                  case EventType.WindowEnterFullscreen:
                  case EventType.WindowLeaveFullscreen:
                  case EventType.WindowDestroyed:
                  case EventType.WindowHdrStateChanged:
                        return new WindowEvent(ref @event);
                  case EventType.KeyDown:
                  case EventType.KeyUp:
                        return new KeyboardEvent(ref @event);
                  case EventType.TextEditing:
                        return new TextEditingEvent(ref @event);
                  case EventType.TextInput:
                        return new TextInputEvent(ref @event);
                  case EventType.KeymapChanged:
                        return new CommonEvent(ref @event);
                  case EventType.KeyboardAdded:
                  case EventType.KeyboardRemoved:
                        return new KeyboardDeviceEvent(ref @event);
                  case EventType.TextEditingCandidates:
                        return new TextEditingCandidatesEvent(ref @event);
                  case EventType.MouseMotion:
                        return new MouseMotionEvent(ref @event);
                  case EventType.MouseButtonDown:
                  case EventType.MouseButtonUp:
                        return new MouseButtonEvent(ref @event);
                  case EventType.MouseWheel:
                        return new MouseWheelEvent(ref @event);
                  case EventType.MouseAdded:
                  case EventType.MouseRemoved:
                        return new MouseDeviceEvent(ref @event);
                  case EventType.JoystickAxisMotion:
                        return new JoyAxisEvent(ref @event);
                  case EventType.JoystickBallMotion:
                        return new JoyBallEvent(ref @event);
                  case EventType.JoystickHatMotion:
                        return new JoyHatEvent(ref @event);
                  case EventType.JoystickButtonDown:
                  case EventType.JoystickButtonUp:
                        return new JoyButtonEvent(ref @event);
                  case EventType.JoystickAdded:
                  case EventType.JoystickRemoved:
                        return new JoyDeviceEvent(ref @event);
                  case EventType.JoystickBatteryUpdated:
                        return new JoyBatteryEvent(ref @event);
                  case EventType.JoystickUpdateComplete:
                        return new JoyDeviceEvent(ref @event);
                  case EventType.GamepadAxisMotion:
                        return new GamepadAxisEvent(ref @event);
                  case EventType.GamepadButtonDown:
                  case EventType.GamepadButtonUp:
                        return new GamepadButtonEvent(ref @event);
                  case EventType.GamepadAdded:
                  case EventType.GamepadRemoved:
                  case EventType.GamepadRemapped:
                  case EventType.GamepadSensorUpdate:
                  case EventType.GamepadUpdateComplete:
                        return new GamepadDeviceEvent(ref @event);
                  case EventType.GamepadTouchpadDown:
                  case EventType.GamepadTouchpadUp:
                        return new GamepadTouchpadEvent(ref @event);
                  case EventType.GamepadTouchpadMotion:
                        return new GamepadTouchpadEvent(ref @event);
                  case EventType.FingerDown:
                  case EventType.FingerUp:
                        return new TouchFingerEvent(ref @event);
                  case EventType.FingerMotion:
                        return new TouchFingerEvent(ref @event);
                  case EventType.FingerCanceled:
                        return new TouchFingerEvent(ref @event);
                  case EventType.ClipboardUpdate:
                        return new ClipboardEvent(ref @event);
                  case EventType.DropFile:
                  case EventType.DropText:
                  case EventType.DropBegin:
                  case EventType.DropComplete:
                  case EventType.DropPosition:
                        return new DropEvent(ref @event);
                  case EventType.AudioDeviceAdded:
                  case EventType.AudioDeviceRemoved:
                  case EventType.AudioDeviceFormatChanged:
                        return new AudioDeviceEvent(ref @event);
                  case EventType.SensorUpdate:
                        return new SensorEvent(ref @event);
                  case EventType.PenProximityIn:
                  case EventType.PenProximityOut:
                        return new PenProximityEvent(ref @event);
                  case EventType.PenDown:
                  case EventType.PenUp:
                        return new PenTouchEvent(ref @event);
                  case EventType.PenButtonDown:
                  case EventType.PenButtonUp:
                        return new PenButtonEvent(ref @event);
                  case EventType.PenMotion:
                        return new PenMotionEvent(ref @event);
                  case EventType.PenAxis:
                        return new PenAxisEvent(ref @event);
                  case EventType.CameraDeviceAdded:
                  case EventType.CameraDeviceRemoved:
                  case EventType.CameraDeviceApproved:
                  case EventType.CameraDeviceDenied:
                        return new CameraDeviceEvent(ref @event);
                  case EventType.RenderTargetsReset:
                  case EventType.RenderDeviceReset:
                  case EventType.RenderDeviceLost:
                        return new CommonEvent(ref @event);
                  case EventType.Private0:
                  case EventType.Private1:
                  case EventType.Private2:
                  case EventType.Private3:
                        return new CommonEvent(ref @event);
                  case EventType.PollSentinel:
                        return new CommonEvent(ref @event);
                  case EventType.User:
                        return new UserEvent(ref @event);
                  default:
                        return new CommonEvent(ref @event);
            }
      }

      public static Event Create(EventType type) {
            var @event = new SDL_Event {
                  type = (uint)type
            };
            return Create(ref @event);
      }

      /// <summary>
      /// Event type, shared with all events, Uint32 to cover user events which are not in the EventType enumeration
      /// </summary>
      public EventType Type {
            get => (EventType)_backingStruct.type;
            protected set => _backingStruct.type = (uint)value;
      }
      
      public override string ToString() {
            using var pin = _backingStruct.Pin();
            var ptr = (SDL_Event*)pin.Addr;
            var len = SDL_GetEventDescription(ptr, null, 0);
            using var buf = Util.RentArray<byte>(len+1);
            buf.Rented[len] = 0;
            fixed(byte* bufPtr = buf.Rented)
                  SDL_GetEventDescription(ptr, bufPtr, buf.Length);
            return Encoding.UTF8.GetString(buf);
      }
}