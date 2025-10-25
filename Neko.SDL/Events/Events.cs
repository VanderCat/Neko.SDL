using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;
using Neko.Sdl.Input;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

public class CommonEvent : Event {
    public CommonEvent() : base() { }

    public CommonEvent(EventType eventType) {
        _backingStruct.type = (uint)eventType;
    }
    public CommonEvent(ref SDL_Event @event) : base(ref @event) {}
    /// <summary>
    /// In nanoseconds, populated using <see cref="Neko.Sdl.Time.Timer.GetTicksNS"/>
    /// </summary>
    public ulong Timestamp {
        get => _backingStruct.common.timestamp;
        set => _backingStruct.common.timestamp = value;
    }
}

/// <summary>
/// Audio device event
/// </summary>
/// <remarks>
/// Note that SDL will send a <see cref="EventType.AudioDeviceAdded"/> event for every device it discovers during
/// initialization. After that, this event will only arrive when a device is hotplugged during the program's run.
/// </remarks>
public sealed class AudioDeviceEvent : CommonEvent {
    public AudioDeviceEvent(EventType eventType) : base(eventType) {
        Debug.Assert(
            eventType is 
                EventType.AudioDeviceAdded or 
                EventType.AudioDeviceRemoved or 
                EventType.AudioDeviceFormatChanged
        );
    }

    public AudioDeviceEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(
            @event.adevice.type is 
                SDL_EventType.SDL_EVENT_AUDIO_DEVICE_ADDED or 
                SDL_EventType.SDL_EVENT_AUDIO_DEVICE_REMOVED or 
                SDL_EventType.SDL_EVENT_AUDIO_DEVICE_FORMAT_CHANGED
            );
    }

    /// <summary>
    /// ID for the device being added or removed or changing
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.adevice.which;
        set => _backingStruct.adevice.which = (SDL_AudioDeviceID)value;
    }
    
    /// <summary>
    /// false if a playback device, true if a recording device
    /// </summary>
    public bool Recording {
        get => _backingStruct.adevice.recording;
        set => _backingStruct.adevice.recording = value;
    }
}

/// <summary>
/// Camera device event
/// </summary>
public class CameraDeviceEvent : CommonEvent {
    public CameraDeviceEvent(EventType eventType) : base(eventType) {
        Debug.Assert(
            eventType is 
                EventType.CameraDeviceAdded or 
                EventType.CameraDeviceRemoved or 
                EventType.CameraDeviceApproved or 
                EventType.CameraDeviceDenied
        );
    }

    public CameraDeviceEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(
            @event.cdevice.type is 
                SDL_EventType.SDL_EVENT_CAMERA_DEVICE_ADDED or 
                SDL_EventType.SDL_EVENT_CAMERA_DEVICE_REMOVED or 
                SDL_EventType.SDL_EVENT_CAMERA_DEVICE_APPROVED or 
                SDL_EventType.SDL_EVENT_CAMERA_DEVICE_DENIED
        );
    }
    
    /// <summary>
    /// ID for the device being added or removed or changing
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.adevice.which;
        set => _backingStruct.adevice.which = (SDL_AudioDeviceID)value;
    }
}

/// <summary>
/// An event triggered when the clipboard contents have changed
/// </summary>
public class ClipboardEvent : CommonEvent {
    public ClipboardEvent() {
        _backingStruct.type = (uint)EventType.ClipboardUpdate;
    }

    public ClipboardEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(
            @event.cdevice.type == SDL_EventType.SDL_EVENT_CLIPBOARD_UPDATE
            );
    }

    /// <summary>
    /// are we owning the clipboard (internal update)
    /// </summary>
    public bool Owner {
        get => _backingStruct.clipboard.owner;
        set => _backingStruct.clipboard.owner = value;
    }
    /// <summary>
    /// number of mime types
    /// </summary>
    public int NumMimeTypes {
        get => _backingStruct.clipboard.num_mime_types;
        set => _backingStruct.clipboard.num_mime_types = value;
    }

    private string[]? _mimeTypeCache = null;
    private unsafe byte** _mimeTypeCacheState = null;
    /// <summary>
    /// current mime types
    /// </summary>
    public unsafe string[]? MimeTypes {
        get {
            if (_mimeTypeCacheState == _backingStruct.clipboard.mime_types)
                return _mimeTypeCache;
            var buffer = new string[NumMimeTypes];
            var ptr = _backingStruct.clipboard.mime_types;
            var start = ptr;
            while (ptr is not null) {
                buffer[ptr - start] = Marshal.PtrToStringUTF8((IntPtr)ptr)??""; 
                ptr++;
            }

            _mimeTypeCacheState = _backingStruct.clipboard.mime_types;
            return _mimeTypeCache = buffer;
        }
        set => throw new NotImplementedException();
    }
}

/// <summary>
/// Gamepad device event
/// </summary>
/// <remarks>
/// Joysticks that are supported gamepads receive both an <see cref="JoyDeviceEvent"/> and an
/// <see cref="GamepadDeviceEvent"/>.
/// <br/><br/>
/// SDL will send <see cref="EventType.GamepadAdded"/> events for joysticks that are already plugged in during
/// <see cref="NekoSDL.Init"/> and are recognized as gamepads. It will also send events for joysticks that get gamepad
/// mappings at runtime.
/// </remarks>
public class GamepadDeviceEvent : CommonEvent {
    public GamepadDeviceEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.GamepadAdded or 
            EventType.GamepadRemoved or
            EventType.GamepadRemapped or
            EventType.GamepadUpdateComplete or
            EventType.GamepadSteamHandleUpdated);
    }

    public GamepadDeviceEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is 
            SDL_EventType.SDL_EVENT_GAMEPAD_ADDED or
            SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED or
            SDL_EventType.SDL_EVENT_GAMEPAD_REMAPPED or
            SDL_EventType.SDL_EVENT_GAMEPAD_UPDATE_COMPLETE or
            SDL_EventType.SDL_EVENT_GAMEPAD_STEAM_HANDLE_UPDATED);
    }

    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.gdevice.which;
        set => _backingStruct.gdevice.which = (SDL_JoystickID)value;
    }
}

/// <summary>
/// Gamepad axis motion event
/// </summary>
public class GamepadAxisEvent : CommonEvent {
    public GamepadAxisEvent() : base(EventType.GamepadAxisMotion) { }

    public GamepadAxisEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION);
    }
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.gaxis.which;
        set => _backingStruct.gaxis.which = (SDL_JoystickID)value;
    }

    /// <summary>
    /// The gamepad axis
    /// </summary>
    GamepadAxis Axis {
        get => (GamepadAxis)_backingStruct.gaxis.axis;
        set => _backingStruct.gaxis.axis = (byte)value;
    }
    
    /// <summary>
    /// The axis value (range: -32768 to 32767)
    /// </summary>
    short Value {
        get => _backingStruct.gaxis.value;
        set => _backingStruct.gaxis.value = value;
    }
}
/// <summary>
/// Gamepad button event
/// </summary>
public class GamepadButtonEvent : CommonEvent {
    public GamepadButtonEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is EventType.GamepadButtonUp or EventType.GamepadButtonDown);
    }

    public GamepadButtonEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is 
            SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP or 
            SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN);
    }
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.gbutton.which;
        set => _backingStruct.gbutton.which = (SDL_JoystickID)value;
    }
    /// <summary>
    /// The gamepad button
    /// </summary>
    public GamepadButton Button {
        get => (GamepadButton)_backingStruct.gbutton.button;
        set => _backingStruct.gbutton.button = (byte)value;
    }

    /// <summary>
    /// true if the button is pressed
    /// </summary>
    public bool Down {
        get => _backingStruct.gbutton.down;
        set => _backingStruct.gbutton.down = value;
    }
}

/// <summary>
/// Gamepad touchpad event
/// </summary>
public class GamepadTouchpadEvent : CommonEvent {
    public GamepadTouchpadEvent(EventType eventType) : base() {
        Debug.Assert(eventType is 
            EventType.GamepadTouchpadMotion or
            EventType.GamepadTouchpadUp or
            EventType.GamepadTouchpadDown);
    }

    public GamepadTouchpadEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is 
            SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_MOTION or 
            SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP or
            SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_DOWN);
    }
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.gtouchpad.which;
        set => _backingStruct.gtouchpad.which = (SDL_JoystickID)value;
    }
    /// <summary>
    /// The index of the touchpad
    /// </summary>
    public int Touchpad {
        get => _backingStruct.gtouchpad.touchpad;
        set => _backingStruct.gtouchpad.touchpad = value;
    }
    /// <summary>
    /// The index of the finger on the touchpad
    /// </summary>
    public int Finger {
        get => _backingStruct.gtouchpad.finger;
        set => _backingStruct.gtouchpad.finger = value;
    }
    /// <summary>
    /// Normalized in the range 0...1 with 0 being on the left
    /// </summary>
    public float X {
        get => _backingStruct.gtouchpad.x;
        set => _backingStruct.gtouchpad.x = value;
    }
    /// <summary>
    /// Normalized in the range 0...1 with 0 being at the top
    /// </summary>
    public float Y {
        get => _backingStruct.gtouchpad.y;
        set => _backingStruct.gtouchpad.y = value;
    }
    /// <summary>
    /// Normalized in the range 0...1
    /// </summary>
    public float Pressure {
        get => _backingStruct.gtouchpad.pressure;
        set => _backingStruct.gtouchpad.pressure = value;
    }
}
/// <summary>
/// Gamepad sensor event
/// </summary>
public class GamepadSensorEvent : CommonEvent {
    public GamepadSensorEvent() : base(EventType.GamepadSensorUpdate) { }

    public GamepadSensorEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type == SDL_EventType.SDL_EVENT_GAMEPAD_SENSOR_UPDATE);
    }
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.gsensor.which;
        set => _backingStruct.gsensor.which = (SDL_JoystickID)value;
    }

    /// <summary>
    /// The type of the sensor, one of the values of SensorType
    /// </summary>
    public SensorType Sensor {
        get => (SensorType)_backingStruct.gsensor.sensor;
        set => _backingStruct.gsensor.sensor = (int)value;
    }

    /// <summary>
    /// Up to 3 values from the sensor, as defined in SDL_sensor.h
    /// </summary>
    public Span<float> Data => MemoryMarshal.CreateSpan(ref Unsafe.As<SDL_GamepadSensorEvent._data_e__FixedBuffer, float>(ref _backingStruct.gsensor.data), 3);

    /// <summary>
    /// The timestamp of the sensor reading in nanoseconds, not necessarily synchronized with the system clock
    /// </summary>
    ulong SensorTimestamp {
        get => _backingStruct.gsensor.sensor_timestamp;
        set => _backingStruct.gsensor.sensor_timestamp = value;
    }
}

/// <summary>
/// An event used to drop text or request a file open by the system
/// </summary>
public class DropEvent : CommonEvent {
    public DropEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.DropBegin or
            EventType.DropFile or
            EventType.DropText or
            EventType.DropComplete or
            EventType.DropPosition);
    }

    public DropEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is 
            SDL_EventType.SDL_EVENT_DROP_BEGIN or
            SDL_EventType.SDL_EVENT_DROP_FILE or
            SDL_EventType.SDL_EVENT_DROP_TEXT or
            SDL_EventType.SDL_EVENT_DROP_COMPLETE or
            SDL_EventType.SDL_EVENT_DROP_POSITION);
    }
    /// <summary>
    /// The window id that was dropped on, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.drop.windowID;
        set => _backingStruct.drop.windowID = (SDL_WindowID)value;
    }

    /// <summary>
    /// The window id that was dropped on, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    
    /// <summary>
    /// X coordinate, relative to window (not on begin)
    /// </summary>
    public float X {
        get => _backingStruct.drop.x;
        set => _backingStruct.drop.x = value;
    }
    /// <summary>
    /// Y coordinate, relative to window (not on begin)
    /// </summary>
    public float Y {
        get => _backingStruct.drop.y;
        set => _backingStruct.drop.y = value;
    }
    /// <summary>
    /// The source app that sent this drop event, or NULL if that isn't available
    /// </summary>
    public unsafe string? Source {
        get => Marshal.PtrToStringUTF8((IntPtr)_backingStruct.drop.source);
        set {
            UnmanagedMemory.Free(_backingStruct.drop.source);
            if (value is null) {
                _backingStruct.drop.source = null;
                return;
            }
            _backingStruct.drop.source = value.ToUnmanagedPointer();
        }
    }
    /// <summary>
    /// The text for DropText and the file name for DropFile, NULL for other events
    /// </summary>
    public unsafe string? Data {
        get => Marshal.PtrToStringUTF8((IntPtr)_backingStruct.drop.data);
        set {
            UnmanagedMemory.Free(_backingStruct.drop.data);
            if (value is null) {
                _backingStruct.drop.data = null;
                return;
            }
            _backingStruct.drop.data = value.ToUnmanagedPointer();
        }
    }
}

/// <summary>
/// Touch finger event structure 
/// </summary>
/// <remarks>
/// Coordinates in this event are normalized. x and y are normalized to a range between 0.0f and 1.0f, relative to the
/// window, so (0,0) is the top left and (1,1) is the bottom right. Delta coordinates dx and dy are normalized in the
/// ranges of -1.0f (traversed all the way from the bottom or right to all the way up or left) to 1.0f (traversed all
/// the way from the top or left to all the way down or right).
/// <br/><br/>
/// Note that while the coordinates are normalized, they are not clamped, which means in some circumstances you can get
/// a value outside of this range. For example, a renderer using logical presentation might give a negative value when
/// the touch is in the letterboxing. Some platforms might report a touch outside of the window, which will also be
/// outside of the range.
/// </remarks>
public class TouchFingerEvent : CommonEvent {
    public TouchFingerEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.FingerDown or
            EventType.FingerUp or
            EventType.FingerMotion or
            EventType.FingerCanceled);
    }

    public TouchFingerEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is 
            SDL_EventType.SDL_EVENT_FINGER_DOWN or
            SDL_EventType.SDL_EVENT_FINGER_UP or
            SDL_EventType.SDL_EVENT_FINGER_MOTION or
            SDL_EventType.SDL_EVENT_FINGER_CANCELED);
    }
    
    /// <summary>
    /// The touch device id
    /// </summary>
    public uint TouchId {
        get => (uint)_backingStruct.tfinger.touchID;
        set => _backingStruct.tfinger.touchID = (SDL_TouchID)value;
    }
    
    public uint FingerId {
        get => (uint)_backingStruct.tfinger.fingerID;
        set => _backingStruct.tfinger.fingerID = (SDL_FingerID)value;
    }

    /// <summary>
    /// Normalized in the range 0...1
    /// </summary>
    public float X {
        get => _backingStruct.tfinger.x;
        set => _backingStruct.tfinger.x = value;
    }
    
    /// <summary>
    /// Normalized in the range 0...1
    /// </summary>
    public float Y {
        get => _backingStruct.tfinger.y;
        set => _backingStruct.tfinger.y = value;
    }
    
    /// <summary>
    /// Normalized in the range -1...1
    /// </summary>
    public float Dx {
        get => _backingStruct.tfinger.dx;
        set => _backingStruct.tfinger.dx = value;
    }
    
    /// <summary>
    /// Normalized in the range -1...1
    /// </sum
    /// mary>
    public float Dy {
        get => _backingStruct.tfinger.dy;
        set => _backingStruct.tfinger.dy = value;
    }
    
    /// <summary>
    /// Normalized in the range 0...1
    /// </summary>
    public float Pressure {
        get => _backingStruct.tfinger.pressure;
        set => _backingStruct.tfinger.pressure = value;
    }
    
    /// <summary>
    /// The window underneath the finger, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.tfinger.windowID;
        set => _backingStruct.tfinger.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The window underneath the finger, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
}

/// <summary>
/// Keyboard device event
/// </summary>
public class KeyboardDeviceEvent : CommonEvent {
    public KeyboardDeviceEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.KeyboardAdded or
            EventType.KeyboardRemoved);
    }

    public KeyboardDeviceEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is 
            EventType.KeyboardAdded or
            EventType.KeyboardRemoved);
    }
    /// <summary>
    /// The keyboard instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.kdevice.which;
        set => _backingStruct.kdevice.which = (SDL_KeyboardID)value;
    }
}

/// <summary>
/// Keyboard button event structure
/// </summary>
/// <remarks>
/// The key is the base <see cref="Keycode"/> generated by pressing the scancode using the current keyboard layout,
/// applying any options specified in <see cref="Hints.KeycodeOptions"/>. You can get the <see cref="Keycode"/>
/// corresponding to the event scancode and  modifiers directly from the keyboard layout, bypassing
/// <see cref="Hints.KeycodeOptions"/>, by calling SDL_GetKeyFromScancode(). TODO:
/// </remarks>
public class KeyboardEvent : CommonEvent {
    public KeyboardEvent(EventType eventType) : base() {
        Debug.Assert(eventType is 
            EventType.KeyDown or
            EventType.KeyUp);
    }

    public KeyboardEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is 
            EventType.KeyDown or
            EventType.KeyUp);
    }
    /// <summary>
    /// The window id with keyboard focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.key.windowID;
        set => _backingStruct.key.windowID = (SDL_WindowID)value;
    }

    /// <summary>
    /// The window with keyboard focus, if any
    /// </summary>
    public Window Window {
        get => Window.GetById(WindowId);
        set => WindowId = value.Id;
    }

    /// <summary>
    /// The keyboard instance id, or 0 if unknown or virtual
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.key.which;
        set => _backingStruct.key.which = (SDL_KeyboardID)value;
    }
    
    /// <summary>
    /// SDL physical key code
    /// </summary>
    public Scancode Scancode {
        get => (Scancode)_backingStruct.key.scancode;
        set => _backingStruct.key.scancode = (SDL_Scancode)value;
    }
    
    /// <summary>
    /// SDL virtual key code
    /// </summary>
    public Keycode Key {
        get => (Keycode)_backingStruct.key.key;
        set => _backingStruct.key.key = (SDL_Keycode)value;
    }
    
    /// <summary>
    /// current key modifiers
    /// </summary>
    public Keymod Mod  {
        get => (Keymod)_backingStruct.key.mod;
        set => _backingStruct.key.mod = (SDL_Keymod)value;
    }
    
    /// <summary>
    /// The platform dependent scancode for this event
    /// </summary>
    public ushort Raw  {
        get => _backingStruct.key.raw;
        set => _backingStruct.key.raw = value;
    }
    
    /// <summary>
    /// true if the key is pressed
    /// </summary>
    public bool Down {
        get => _backingStruct.key.down;
        set => _backingStruct.key.down = value;
    }
    
    /// <summary>
    /// true if this is a key repeat
    /// </summary>
    public bool Repeat {
        get => _backingStruct.key.repeat;
        set => _backingStruct.key.repeat = value;
    }
}
/// <summary>
/// Joystick device event structure
/// </summary>
/// <remarks>
/// SDL will send <see cref="EventType.JoystickAdded"/> events for devices that are already plugged in during
/// <see cref="NekoSDL.Init"/>.
/// </remarks>
public class JoyDeviceEvent : CommonEvent {
    public JoyDeviceEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.JoystickAdded or 
            EventType.JoystickRemoved or
            EventType.JoystickUpdateComplete);
    }

    public JoyDeviceEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is 
            EventType.JoystickAdded or 
            EventType.JoystickRemoved or
            EventType.JoystickUpdateComplete);
    }

    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.jdevice.which;
        set => _backingStruct.jdevice.which = (SDL_JoystickID)value;
    }
}

/// <summary>
/// Joystick axis motion event
/// </summary>
public class JoyAxisEvent : CommonEvent {
    public JoyAxisEvent() : base(EventType.JoystickAxisMotion) { }

    public JoyAxisEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is SDL_EventType.SDL_EVENT_JOYSTICK_AXIS_MOTION);
    }
    
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.jaxis.which;
        set => _backingStruct.jaxis.which = (SDL_JoystickID)value;
    }

    /// <summary>
    /// The gamepad axis
    /// </summary>
    GamepadAxis Axis {
        get => (GamepadAxis)_backingStruct.jaxis.axis;
        set => _backingStruct.jaxis.axis = (byte)value;
    }
    
    /// <summary>
    /// The axis value (range: -32768 to 32767)
    /// </summary>
    short Value {
        get => _backingStruct.jaxis.value;
        set => _backingStruct.jaxis.value = value;
    }
}

/// <summary>
/// Joystick trackball motion event 
/// </summary>
public class JoyBallEvent : CommonEvent {
    public JoyBallEvent() : base(EventType.JoystickBallMotion) { }

    public JoyBallEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is SDL_EventType.SDL_EVENT_JOYSTICK_BALL_MOTION);
    }
    
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.jball.which;
        set => _backingStruct.jball.which = (SDL_JoystickID)value;
    }
    
    /// <summary>
    /// The joystick trackball index
    /// </summary>
    public byte Ball {
        get => _backingStruct.jball.ball;
        set => _backingStruct.jball.ball = value;
    }
    
    /// <summary>
    /// The relative motion in the X direction
    /// </summary>
    public short XRel {
        get => _backingStruct.jball.xrel;
        set => _backingStruct.jball.xrel = value;
    }
    
    /// <summary>
    /// The relative motion in the Y direction
    /// </summary>
    public short YRel {
        get => _backingStruct.jball.yrel;
        set => _backingStruct.jball.yrel = value;
    }
}

/// <summary>
/// Joystick hat position change event structure
/// </summary>
public class JoyHatEvent : CommonEvent {
    public JoyHatEvent() : base(EventType.JoystickHatMotion) { }

    public JoyHatEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is SDL_EventType.SDL_EVENT_JOYSTICK_HAT_MOTION);
    }
    
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.jhat.which;
        set => _backingStruct.jhat.which = (SDL_JoystickID)value;
    }

    /// <summary>
    /// The joystick hat index
    /// </summary>
    public byte Hat {
        get => _backingStruct.jhat.hat;
        set => _backingStruct.jhat.hat = value;
    }
    
    /// <summary>
    /// The hat position value.
    /// </summary>
    public Hat Value {
        get => (Hat)_backingStruct.jhat.value;
        set => _backingStruct.jhat.value = (byte)value;
    }
}

/// <summary>
/// Joystick battery level change event 
/// </summary>
public class JoyBatteryEvent : CommonEvent {
    public JoyBatteryEvent() : base(EventType.JoystickBatteryUpdated) { }

    public JoyBatteryEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is SDL_EventType.SDL_EVENT_JOYSTICK_BATTERY_UPDATED);
    }
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.jbattery.which;
        set => _backingStruct.jbattery.which = (SDL_JoystickID)value;
    }
    /// <summary>
    /// The joystick battery state
    /// </summary>
    public PowerState State {
        get => (PowerState)_backingStruct.jbattery.state;
        set => _backingStruct.jbattery.state = (SDL_PowerState)value;
    }
    /// <summary>
    /// The joystick battery percent charge remaining
    /// </summary>
    public int Percent {
        get => _backingStruct.jbattery.percent;
        set => _backingStruct.jbattery.percent = value;
    }
}

/// <summary>
/// Joystick button event 
/// </summary>
public class JoyButtonEvent : CommonEvent {
    public JoyButtonEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.JoystickButtonDown or
            EventType.JoystickButtonUp);
    }

    public JoyButtonEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is 
            EventType.JoystickButtonDown or
            EventType.JoystickButtonUp);
    }
    
    /// <summary>
    /// The joystick instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.jbutton.which;
        set => _backingStruct.jbutton.which = (SDL_JoystickID)value;
    }
    
    /// <summary>
    /// The joystick button index
    /// </summary>
    public byte Button {
        get => _backingStruct.jbutton.button;
        set => _backingStruct.jbutton.button = value;
    }
    
    /// <summary>
    /// true if the button is pressed
    /// </summary>
    public bool Down {
        get => _backingStruct.jbutton.down;
        set => _backingStruct.jbutton.down = value;
    }
}
/// <summary>
/// Mouse device event structure
/// </summary>
public class MouseDeviceEvent : CommonEvent {
    public MouseDeviceEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.MouseAdded or
            EventType.MouseRemoved);
    }

    public MouseDeviceEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is 
            EventType.MouseAdded or
            EventType.MouseRemoved);
    }
    
    /// <summary>
    /// The mouse instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.jbutton.which;
        set => _backingStruct.jbutton.which = (SDL_JoystickID)value;
    }
}

/// <summary>
/// Mouse motion event
/// </summary>
public class MouseMotionEvent : CommonEvent {
    public MouseMotionEvent() : base(EventType.MouseMotion) { }

    public MouseMotionEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type == SDL_EventType.SDL_EVENT_MOUSE_MOTION);
    }
    /// <summary>
    /// The window id with mouse focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.motion.windowID;
        set => _backingStruct.motion.windowID = (SDL_WindowID)value;
    }

    /// <summary>
    /// The window with mouse focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    
    /// <summary>
    /// The mouse instance id in relative mode, SDL_TOUCH_MOUSEID for touch events, or 0
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.motion.which;
        set => _backingStruct.motion.which = (SDL_MouseID)value;
    }

    public bool IsTouch => (SDL_MouseID)Which == SDL_TOUCH_MOUSEID;
    
    /// <summary>
    /// The current button state
    /// </summary>
    public MouseButton State {
        get => (MouseButton)_backingStruct.motion.state;
        set => _backingStruct.motion.state = (SDL_MouseButtonFlags)value;
    }
    
    /// <summary>
    /// X coordinate, relative to window
    /// </summary>
    public float X {
        get => _backingStruct.motion.x;
        set => _backingStruct.motion.x = value;
    }
    
    /// <summary>
    /// Y coordinate, relative to window
    /// </summary>
    public float Y {
        get => _backingStruct.motion.y;
        set => _backingStruct.motion.y = value;
    }
    
    /// <summary>
    /// The relative motion in the X direction
    /// </summary>
    public float XRel {
        get => _backingStruct.motion.xrel;
        set => _backingStruct.motion.xrel = value;
    }
    
    /// <summary>
    /// The relative motion in the Y direction
    /// </summary>
    public float YRel {
        get => _backingStruct.motion.yrel;
        set => _backingStruct.motion.yrel = value;
    }
}

/// <summary>
/// Mouse button event
/// </summary>
public class MouseButtonEvent : CommonEvent {
    public MouseButtonEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is 
            EventType.MouseButtonUp or 
            EventType.MouseButtonDown);
    }

    public MouseButtonEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is 
            EventType.MouseButtonUp or 
            EventType.MouseButtonDown);
    }
    /// <summary>
    /// The window id with mouse focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.button.windowID;
        set => _backingStruct.button.windowID = (SDL_WindowID)value;
    }

    /// <summary>
    /// The window with mouse focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    
    /// <summary>
    /// The mouse instance id in relative mode, SDL_TOUCH_MOUSEID for touch events, or 0
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.button.which;
        set => _backingStruct.button.which = (SDL_MouseID)value;
    }

    /// <summary>
    /// The mouse button index
    /// </summary>
    public MouseButton Button {
        get => (MouseButton)_backingStruct.button.Button;
        set => _backingStruct.button.button = (byte)value;
    }

    /// <summary>
    /// true if the button is pressed
    /// </summary>
    private bool Down {
        get => _backingStruct.button.down;
        set => _backingStruct.button.down = value;
    }
    
    /// <summary>
    /// 1 for single-click, 2 for double-click, etc.
    /// </summary>
    public byte Clicks {
        get => _backingStruct.button.clicks;
        set => _backingStruct.button.clicks = value;
    }

    /// <summary>
    /// X coordinate, relative to window
    /// </summary>
    public float X {
        get => _backingStruct.button.x;
        set => _backingStruct.button.x = value;
    }
    
    /// <summary>
    /// Y coordinate, relative to window
    /// </summary>
    public float Y {
        get => _backingStruct.button.y;
        set => _backingStruct.button.y = value;
    }
}

/// <summary>
/// Mouse wheel event structure
/// </summary>
public class MouseWheelEvent : CommonEvent {
    public MouseWheelEvent() : base(EventType.MouseWheel) { }

    public MouseWheelEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type == SDL_EventType.SDL_EVENT_MOUSE_WHEEL);
    }
    
    /// <summary>
    /// The window id with mouse focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.wheel.windowID;
        set => _backingStruct.wheel.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The window with mouse focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    
    /// <summary>
    /// The mouse instance id in relative mode or 0
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.wheel.which;
        set => _backingStruct.wheel.which = (SDL_MouseID)value;
    }
    
    /// <summary>
    /// The amount scrolled horizontally, positive to the right and negative to the left
    /// </summary>
    public float X {
        get => _backingStruct.wheel.x;
        set => _backingStruct.wheel.x = value;
    }
    /// <summary>
    /// The amount scrolled vertically, positive away from the user and negative toward the user
    /// </summary>
    public float Y {
        get => _backingStruct.wheel.y;
        set => _backingStruct.wheel.y = value;
    }
    
    /// <summary>
    ///  Set to one of the SDL_MOUSEWHEEL_* defines. When FLIPPED the values in X and Y will be opposite. Multiply by -1 to change them back
    /// </summary>
    public MouseWheelDirection Direction {
        get => (MouseWheelDirection)_backingStruct.wheel.direction;
        set => _backingStruct.wheel.direction = (SDL_MouseWheelDirection)value;
    }
    
    /// <summary>
    /// X coordinate, relative to window
    /// </summary>
    public float MouseX {
        get => _backingStruct.wheel.mouse_x;
        set => _backingStruct.wheel.mouse_x = value;
    }
    
    /// <summary>
    /// Y coordinate, relative to window
    /// </summary>
    public float MouseY {
        get => _backingStruct.wheel.mouse_y;
        set => _backingStruct.wheel.mouse_y = value;
    }
    
    /// <summary>
    /// The amount scrolled horizontally, accumulated to whole scroll "ticks" (added in 3.2.12)
    /// </summary>
    public int IntegerX {
        get => _backingStruct.wheel.integer_x;
        set => _backingStruct.wheel.integer_x = value;
    }
    
    /// <summary>
    /// The amount scrolled vertically, accumulated to whole scroll "ticks" (added in 3.2.12)
    /// </summary>
    public int IntegerY {
        get => _backingStruct.wheel.integer_y;
        set => _backingStruct.wheel.integer_y = value;
    }
}
/// <summary>
/// Pressure-sensitive pen proximity event
/// </summary>
/// <remarks>
/// When a pen becomes visible to the system (it is close enough to a tablet, etc), SDL will send an
/// <see cref="EventType.PenProximityIn"/> event with the new pen's ID. This ID is valid until the pen leaves proximity
/// again (has been removed from the tablet's area, the tablet has been unplugged, etc). If the same pen reenters
/// proximity again, it will be given a new ID.
/// <br/><br/>
/// Note that "proximity" means "close enough for the tablet to know the tool is there." The pen touching and lifting
/// off from the tablet while not leaving the area are handled by <see cref="EventType.PenDown"/> and
/// <see cref="EventType.PenUp"/>.
/// </remarks>
public class PenProximityEvent : CommonEvent {
    public PenProximityEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is EventType.PenProximityIn or EventType.PenProximityOut);
    }

    public PenProximityEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is EventType.PenProximityIn or EventType.PenProximityOut);
    }
    
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.pproximity.windowID;
        set => _backingStruct.pproximity.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    /// <summary>
    /// The pen instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.pproximity.which;
        set => _backingStruct.pproximity.which = (SDL_PenID)value;
    }
}
/// <summary>
/// Pressure-sensitive pen touched event structure
/// </summary>
/// <remarks>
/// These events come when a pen touches a surface (a tablet, etc), or lifts off from one
/// </remarks>
public class PenTouchEvent : CommonEvent {
    public PenTouchEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is EventType.PenDown or EventType.PenUp);
    }

    public PenTouchEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is EventType.PenDown or EventType.PenUp);
    }
    
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.ptouch.windowID;
        set => _backingStruct.ptouch.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    /// <summary>
    /// The pen instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.ptouch.which;
        set => _backingStruct.ptouch.which = (SDL_PenID)value;
    }

    /// <summary>
    /// Complete pen input state at time of event
    /// </summary>
    public PenInputFlags PenState {
        get => (PenInputFlags)_backingStruct.ptouch.pen_state;
        set => _backingStruct.ptouch.pen_state = (SDL_PenInputFlags)value;
    }
    
    /// <summary>
    /// X coordinate, relative to window
    /// </summary>
    public float X {
        get => _backingStruct.ptouch.x;
        set => _backingStruct.ptouch.x = value;
    }
    
    /// <summary>
    /// Y coordinate, relative to window 
    /// </summary>
    public float Y {
        get => _backingStruct.ptouch.y;
        set => _backingStruct.ptouch.y = value;
    }
    
    /// <summary>
    /// true if eraser end is used (not all pens support this).
    /// </summary>
    public bool Eraser {
        get => _backingStruct.ptouch.eraser;
        set => _backingStruct.ptouch.eraser = value;
    }
    
    /// <summary>
    /// true if the pen is touching or false if the pen is lifted off
    /// </summary>
    public bool Down {
        get => _backingStruct.ptouch.down;
        set => _backingStruct.ptouch.down = value;
    }
}
/// <summary>
/// Pressure-sensitive pen motion event
/// </summary>
/// <remarks>
/// Depending on the hardware, you may get motion events when the pen is not touching a tablet, for tracking a pen even
/// when it isn't drawing. You should listen for <see cref="EventType.PenDown"/> and <see cref="EventType.PenUp"/> 
/// events, or check PenState & <see cref="PenInputFlags.Down"/> to decide if a pen is "drawing" when dealing with pen
/// motion.
/// </remarks>
public class PenMotionEvent : CommonEvent {
    public PenMotionEvent() : base(EventType.PenMotion) { }

    public PenMotionEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is SDL_EventType.SDL_EVENT_PEN_MOTION);
    }
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.pmotion.windowID;
        set => _backingStruct.pmotion.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    /// <summary>
    /// The pen instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.pmotion.which;
        set => _backingStruct.pmotion.which = (SDL_PenID)value;
    }

    /// <summary>
    /// Complete pen input state at time of event
    /// </summary>
    public PenInputFlags PenState {
        get => (PenInputFlags)_backingStruct.pmotion.pen_state;
        set => _backingStruct.pmotion.pen_state = (SDL_PenInputFlags)value;
    }
    
    /// <summary>
    /// X coordinate, relative to window
    /// </summary>
    public float X {
        get => _backingStruct.pmotion.x;
        set => _backingStruct.pmotion.x = value;
    }
    
    /// <summary>
    /// Y coordinate, relative to window 
    /// </summary>
    public float Y {
        get => _backingStruct.pmotion.y;
        set => _backingStruct.pmotion.y = value;
    }
}

/// <summary>
/// Pressure-sensitive pen button event 
/// </summary>
/// <remarks>
/// This is for buttons on the pen itself that the user might click. The pen itself pressing down to draw triggers a
/// <see cref="PenInputFlags.Down"/> event instead.
/// </remarks>
public class PenButtonEvent : CommonEvent {
    public PenButtonEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is EventType.PenButtonDown or EventType.PenButtonUp);
    }
    public PenButtonEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is EventType.PenButtonDown or EventType.PenButtonUp);
    }
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.pbutton.windowID;
        set => _backingStruct.pbutton.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    /// <summary>
    /// The pen instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.pbutton.which;
        set => _backingStruct.pbutton.which = (SDL_PenID)value;
    }

    /// <summary>
    /// Complete pen input state at time of event
    /// </summary>
    public PenInputFlags PenState {
        get => (PenInputFlags)_backingStruct.pbutton.pen_state;
        set => _backingStruct.pbutton.pen_state = (SDL_PenInputFlags)value;
    }
    
    /// <summary>
    /// X coordinate, relative to window
    /// </summary>
    public float X {
        get => _backingStruct.pbutton.x;
        set => _backingStruct.pbutton.x = value;
    }
    
    /// <summary>
    /// Y coordinate, relative to window 
    /// </summary>
    public float Y {
        get => _backingStruct.pbutton.y;
        set => _backingStruct.pbutton.y = value;
    }
    /// <summary>
    /// The pen button index (first button is 1).
    /// </summary>
    public byte Button {
        get => _backingStruct.pbutton.button;
        set => _backingStruct.pbutton.button = value;
    }
    
    /// <summary>
    /// true if the button is pressed
    /// </summary>
    public bool Down {
         get => _backingStruct.pbutton.down;
         set => _backingStruct.pbutton.down = value;
    }
}
public class PenAxisEvent : CommonEvent {
    public PenAxisEvent() : base(EventType.PenAxis) { }
    public PenAxisEvent(ref SDL_Event @event) : base(ref @event) {}
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.paxis.windowID;
        set => _backingStruct.paxis.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The window with pen focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    /// <summary>
    /// The pen instance id
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.paxis.which;
        set => _backingStruct.paxis.which = (SDL_PenID)value;
    }

    /// <summary>
    /// Complete pen input state at time of event
    /// </summary>
    public PenInputFlags PenState {
        get => (PenInputFlags)_backingStruct.paxis.pen_state;
        set => _backingStruct.paxis.pen_state = (SDL_PenInputFlags)value;
    }
    
    /// <summary>
    /// X coordinate, relative to window
    /// </summary>
    public float X {
        get => _backingStruct.paxis.x;
        set => _backingStruct.paxis.x = value;
    }
    
    /// <summary>
    /// Y coordinate, relative to window 
    /// </summary>
    public float Y {
        get => _backingStruct.paxis.y;
        set => _backingStruct.paxis.y = value;
    }
    /// <summary>
    /// Axis that has changed
    /// </summary>
    public PenAxis Button {
        get => (PenAxis)_backingStruct.paxis.axis;
        set => _backingStruct.paxis.axis = (SDL_PenAxis)value;
    }
    
    /// <summary>
    /// New value of axis
    /// </summary>
    public float Value {
        get => _backingStruct.paxis.value;
        set => _backingStruct.paxis.value = value;
    }
}
/// <summary>
/// The "quit requested" event
/// </summary>
public class QuitEvent : CommonEvent {
    public QuitEvent() : base(EventType.Quit) { }

    public QuitEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type == SDL_EventType.SDL_EVENT_QUIT);
    }
}

/// <summary>
/// Sensor event
/// </summary>
public class SensorEvent : CommonEvent {
    public SensorEvent() : base(EventType.SensorUpdate) { }
    public SensorEvent(ref SDL_Event @event) : base(ref @event) {}
    /// <summary>
    /// The instance ID of the sensor
    /// </summary>
    public uint Which {
        get => (uint)_backingStruct.sensor.which;
        set => _backingStruct.sensor.which = (SDL_SensorID)value;
    }

    /// <summary>
    /// Up to 6 values from the sensor - additional values can be queried using SDL_GetSensorData() 
    /// </summary>
    public Span<float> Data => MemoryMarshal.CreateSpan(ref Unsafe.As<SDL_SensorEvent._data_e__FixedBuffer, float>(ref _backingStruct.sensor.data), 6);
    
    /// <summary>
    /// The timestamp of the sensor reading in nanoseconds, not necessarily synchronized with the system clock
    /// </summary>
    public ulong SensorTimestamp {
        get => _backingStruct.sensor.sensor_timestamp;
        set => _backingStruct.sensor.sensor_timestamp = value;
    }
}

/// <summary>
/// Keyboard text editing event
/// </summary>
/// <remarks>
/// The start cursor is the position, in UTF-8 characters, where new typing will be inserted into the editing text.
/// The length is the number of UTF-8 characters that will be replaced by new typing.
/// </remarks>
public class TextEditingEvent : CommonEvent {
    public TextEditingEvent() : base(EventType.TextEditing) { }

    public TextEditingEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type == SDL_EventType.SDL_EVENT_TEXT_EDITING);
    }
    
    /// <summary>
    /// The window id with keyboard focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.edit.windowID;
        set => _backingStruct.edit.windowID = (SDL_WindowID)value;
    }

    /// <summary>
    /// The window with keyboard focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    
    /// <summary>
    /// The editing text
    /// </summary>
    public unsafe string? Text {
        get => Marshal.PtrToStringUTF8((IntPtr)_backingStruct.edit.text);
        set {
            UnmanagedMemory.Free(_backingStruct.edit.text);
            if (value is null) {
                _backingStruct.edit.text = null;
                return;
            }
            _backingStruct.edit.text = value.ToUnmanagedPointer();
        }
    }
    
    /// <summary>
    /// The start cursor of selected editing text, or -1 if not set
    /// </summary>
    public int Start {
        get => _backingStruct.edit.start;
        set => _backingStruct.edit.start = value;
    }
    
    /// <summary>
    /// The length of selected editing text, or -1 if not set
    /// </summary>
    public int Length {
        get => _backingStruct.edit.length;
        set => _backingStruct.edit.length = value;
    }
}

/// <summary>
/// Keyboard IME candidates event 
/// </summary>
public class TextEditingCandidatesEvent : CommonEvent, IEnumerable<string?> {
    public TextEditingCandidatesEvent() : base(EventType.TextEditingCandidates) { }

    public TextEditingCandidatesEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type == SDL_EventType.SDL_EVENT_TEXT_EDITING_CANDIDATES);
    }
    
    /// <summary>
    /// The window id with keyboard focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.wheel.windowID;
        set => _backingStruct.wheel.windowID = (SDL_WindowID)value;
    }

    /// <summary>
    /// The window with keyboard focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    /// <summary>
    /// The number of strings in `candidates`
    /// </summary>
    public int NumCandidates {
        get => _backingStruct.edit_candidates.num_candidates;
        set => _backingStruct.edit_candidates.num_candidates = value;
    }

    /// <summary>
    /// The index of the selected candidate, or -1 if no candidate is selected
    /// </summary>
    public int SelectedCandidateIndex {
        get => _backingStruct.edit_candidates.selected_candidate;
        set => _backingStruct.edit_candidates.selected_candidate = value;
    }
    
    /// <summary>
    /// The index of the selected candidate, or -1 if no candidate is selected
    /// </summary>
    public unsafe string? SelectedCandidate => SelectedCandidateIndex == -1 ? null : this[SelectedCandidateIndex];
    /// <summary>
    /// true if the list is horizontal, false if it's vertical
    /// </summary>
    public bool Horizontal {
        get => _backingStruct.edit_candidates.horizontal;
        set => _backingStruct.edit_candidates.horizontal = value;
    }
    
    public unsafe string? this[int index] {
        get {
            if (index < 0 || index >= NumCandidates)
                throw new ArgumentOutOfRangeException(nameof(index));
            var ptr = _backingStruct.edit_candidates.candidates[index];
            return Marshal.PtrToStringUTF8((IntPtr)ptr);
        }
        set {
            if (index < 0 || index >= NumCandidates)
                throw new ArgumentOutOfRangeException(nameof(index));
            var ptr = _backingStruct.edit_candidates.candidates[index];
            UnmanagedMemory.Free(ptr);
            if (value is null) {
                _backingStruct.edit_candidates.candidates[index] = null;
                return;
            }
            _backingStruct.edit_candidates.candidates[index] = value.ToUnmanagedPointer();
        }
    }

    private class TextEditingCandidatesEventEnumerator(TextEditingCandidatesEvent @event) : IEnumerator<string?> {
        public bool MoveNext() {
            Cursor++;
            return Cursor < @event.NumCandidates;
        }

        public void Reset() {
            Cursor = 0;
        }

        public int Cursor = 0;

        public string? Current => @event[Cursor];

        object? IEnumerator.Current => Current;

        public void Dispose() {
            
        }
    }

    public IEnumerator<string?> GetEnumerator() {
        return new TextEditingCandidatesEventEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
/// <remarks>
/// Keyboard text input event
/// </remarks>
/// <remarks>
/// This event will never be delivered unless text input is enabled by calling <see cref="TextInput.Start"/>.
/// Text input is disabled by default!
/// </remarks>
public class TextInputEvent : CommonEvent {
    public TextInputEvent() : base(EventType.TextInput) { }

    public TextInputEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type == SDL_EventType.SDL_EVENT_TEXT_INPUT);
    }
    
    /// <summary>
    /// The window id with keyboard focus, if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.text.windowID;
        set => _backingStruct.text.windowID = (SDL_WindowID)value;
    }

    /// <summary>
    /// The window with keyboard focus, if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    
    /// <summary>
    /// The input text
    /// </summary>
    public unsafe string? Text {
        get => Marshal.PtrToStringUTF8((IntPtr)_backingStruct.text.text);
        set {
            UnmanagedMemory.Free(_backingStruct.text.text);
            if (value is null) {
                _backingStruct.text.text = null;
                return;
            }
            _backingStruct.text.text = value.ToUnmanagedPointer();
        }
    }
}
/// <summary>
/// A user-defined event type
/// </summary>
/// <remarks>
/// This event is unique; it is never created by SDL, but only by the application. The event can be pushed onto the
/// event queue using <see cref="EventQueue.Push"/>. The contents of the structure members are completely up to the
/// programmer; the only requirement is that 'Type' is a value obtained from <see cref="EventQueue.Register"/>.
/// </remarks>
public class UserEvent : CommonEvent {
    public UserEvent(EventType eventType) : base() {
        Debug.Assert(eventType is >= EventType.User and <= EventType.Last);
    }

    public UserEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is >= EventType.User and <= EventType.Last);
    }
    /// <summary>
    /// The associated window id if any
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.user.windowID;
        set => _backingStruct.user.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The associated window if any
    /// </summary>
    public Window? Window {
        get => WindowId != 0 ? Window.GetById(WindowId) : null;
        set => WindowId = value?.Id??0;
    }
    /// <summary>
    /// User defined event code
    /// </summary>
    public int Code {
        get => _backingStruct.user.code;
        set => _backingStruct.user.code = value;
    }

    /// <summary>
    /// User defined data
    /// </summary>
    public object? Data1 {
        get {
            if (_backingStruct.user.data1 == 0)
                return null;
            var handle = GCHandle.FromIntPtr(_backingStruct.user.data1);
            return handle.Target;
        }
        set {
            if (_backingStruct.user.data1 != 0)
                GCHandle.FromIntPtr(_backingStruct.user.data1).Free();
            var handle = GCHandle.Alloc(value);
            _backingStruct.user.data1 = GCHandle.ToIntPtr(handle);
        }
    }
    
    /// <summary>
    /// User defined data
    /// </summary>
    public object? Data2 {
        get {
            if (_backingStruct.user.data2 == 0)
                return null;
            var handle = GCHandle.FromIntPtr(_backingStruct.user.data2);
            return handle.Target;
        }
        set {
            if (_backingStruct.user.data2 != 0)
                GCHandle.FromIntPtr(_backingStruct.user.data2).Free();
            var handle = GCHandle.Alloc(value);
            _backingStruct.user.data2 = GCHandle.ToIntPtr(handle);
        }
    }
}

/// <summary>
/// Window state change event data
/// </summary>
public class WindowEvent : CommonEvent {
    public WindowEvent(EventType eventType) : base(eventType) {
        Debug.Assert(eventType is >= EventType.WindowFirst and <= EventType.WindowLast);
    }

    public WindowEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.EventType() is >= EventType.WindowFirst and <= EventType.WindowLast);
    }
    /// <summary>
    /// The associated window id
    /// </summary>
    public uint WindowId {
        get => (uint)_backingStruct.window.windowID;
        set => _backingStruct.window.windowID = (SDL_WindowID)value;
    }
    
    /// <summary>
    /// The associated window
    /// </summary>
    public Window Window {
        get => Window.GetById(WindowId);
        set => WindowId = value.Id;
    }
    /// <summary>
    /// event dependent data
    /// </summary>
    public int Data1 {
        get => _backingStruct.window.data1;
        set => _backingStruct.window.data1 = value;
    }
    /// <summary>
    /// event dependent data
    /// </summary>
    public int Data2 {
        get => _backingStruct.window.data2;
        set => _backingStruct.window.data2 = value;
    }
}

/// <summary>
/// Display state change event data
/// </summary>
public class DisplayEvent : CommonEvent {
    public DisplayEvent(EventType eventType) : base() {
        Debug.Assert(eventType is >= EventType.DisplayFirst and <= EventType.DisplayLast);
    }

    public DisplayEvent(ref SDL_Event @event) : base(ref @event) {
        Debug.Assert(@event.Type is >= SDL_EventType.SDL_EVENT_DISPLAY_FIRST and <= SDL_EventType.SDL_EVENT_DISPLAY_LAST);
    }
    /// <summary>
    /// The associated display
    /// </summary>
    public uint DisplayId {
        get => (uint)_backingStruct.display.displayID;
        set => _backingStruct.display.displayID = (SDL_DisplayID)value;
    }

    /// <summary>
    /// event dependent data
    /// </summary>
    public int Data1 {
        get => _backingStruct.display.data1;
        set => _backingStruct.display.data1 = value;
    }
    /// <summary>
    /// event dependent data
    /// </summary>
    public int Data2 {
        get => _backingStruct.display.data1;
        set => _backingStruct.display.data1 = value;
    }
}