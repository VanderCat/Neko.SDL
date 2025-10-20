using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.CodeGen;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

/// <summary>
/// The structure for all events in SDL
/// </summary>
/// <remarks>
/// The <see cref="Event"/> structure is the core of all event handling in SDL. <see cref="Event"/> is a union of all
/// event structures used in SDL.
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
public unsafe partial struct Event {
      [FieldOffset(0)]
      public EventType Type;
      [FieldOffset(0)]
      public CommonEvent Common;
      [FieldOffset(0)]
      public DisplayEvent Display;
      [FieldOffset(0)]
      public WindowEvent Window;
      [FieldOffset(0)]
      public KeyboardDeviceEvent KDevice;
      [FieldOffset(0)]
      public KeyboardEvent Key;
      [FieldOffset(0)]
      public TextEditingEvent Edit;
      [FieldOffset(0)]
      public TextEditingCandidatesEvent EditCandidates;
      [FieldOffset(0)]
      public TextInputEvent Text;
      [FieldOffset(0)]
      public MouseDeviceEvent MDevice;
      [FieldOffset(0)]
      public MouseMotionEvent Motion;
      [FieldOffset(0)]
      public MouseButtonEvent Button;
      [FieldOffset(0)]
      public MouseWheelEvent Wheel;
      [FieldOffset(0)]
      public JoyDeviceEvent JDevice;
      // [FieldOffset(0)]
      // public JoyAxisEvent JAxis;
      // [FieldOffset(0)]
      // public JoyBallEvent JBall;
      // [FieldOffset(0)]
      // public JoyHatEvent JHat;
      // [FieldOffset(0)]
      // public JoyButtonEvent JButton;
      // [FieldOffset(0)]
      // public JoyBatteryEvent JBattery;
      // [FieldOffset(0)]
      // public GamepadDeviceEvent GDevice;
      // [FieldOffset(0)]
      // public GamepadAxisEvent GAxis;
      // [FieldOffset(0)]
      // public GamepadButtonEvent GButton;
      // [FieldOffset(0)]
      // public GamepadTouchpadEvent GTouchpad;
      // [FieldOffset(0)]
      // public GamepadSensorEvent GSensor;
      // [FieldOffset(0)]
      // public AudioDeviceEvent ADevice;
      // [FieldOffset(0)]
      // public CameraDeviceEvent CDevice;
      // [FieldOffset(0)]
      // public SensorEvent Sensor;
      // [FieldOffset(0)]
      // public QuitEvent Quit;
      // [FieldOffset(0)]
      // public UserEvent User;
      // [FieldOffset(0)]
      // public TouchFingerEvent TFinger;
      // [FieldOffset(0)]
      // public PenProximityEvent PProximity;
      // [FieldOffset(0)]
      // public PenTouchEvent PTouch;
      // [FieldOffset(0)]
      // public PenMotionEvent PMotion;
      // [FieldOffset(0)]
      // public PenButtonEvent PButton;
      // [FieldOffset(0)]
      // public PenAxisEvent PAxis;
      // [FieldOffset(0)]
      // public RenderEvent Render;
      // [FieldOffset(0)]
      // public DropEvent Drop;
      // [FieldOffset(0)]
      // public ClipboardEvent Clipboard;
      [FieldOffset(0)]
      public _padding_e__FixedBuffer padding;

      [InlineArray(128 /*0x80*/)]
      public struct _padding_e__FixedBuffer
      {
        public byte e0;
      }
}