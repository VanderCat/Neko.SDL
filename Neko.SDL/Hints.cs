using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Neko.Sdl;

public static partial class Hints {
    public enum Priority : uint {
        Default = SDL_HintPriority.SDL_HINT_DEFAULT,
        Normal = SDL_HintPriority.SDL_HINT_NORMAL,
        Override = SDL_HintPriority.SDL_HINT_OVERRIDE,
    }
    public class Hint {
        internal Hint(string name) {
            _name = name;
        }

        internal Hint(ReadOnlySpan<byte> name) {
            _name = Encoding.UTF8.GetString(name);
        }
        
        private readonly string _name;
        
        public bool GetBool(bool defaultValue = false) => SDL_GetHintBoolean(_name, defaultValue);
        public string? GetValue() => SDL_GetHint(_name);
        public void SetValue(string value) => SDL_SetHint(_name, value);

        public void SetValue(string value, Priority priority) =>
            SDL_SetHintWithPriority(_name, value, (SDL_HintPriority)(uint)priority);

        public void Reset() => SDL_ResetHint(_name).ThrowIfError();

        private bool _internalCallbackAssigned = false;
        private Pin<Hint>? _pin;

        internal event HintCallback? _callbacks;
        
        public unsafe event HintCallback Callbacks {
            add {
                if (!_internalCallbackAssigned) {
                    _pin = this.Pin();
                    SDL_AddHintCallback(_name, &UmnmagedHintCallback, _pin.Pointer);
                }
                _callbacks += value;
            }
            remove {
                _callbacks -= value;
                
                if (!_internalCallbackAssigned) return;
                if ((_callbacks?.GetInvocationList().Length ?? 0) > 0) return;
                
                SDL_RemoveHintCallback(_name, &UmnmagedHintCallback, _pin.Pointer);
                _pin.Dispose();
            }
        }

        internal void InvokeCallbacks(string? oldValue, string? newValue) {
            _callbacks?.Invoke(oldValue, newValue);
        }
    }

    public delegate void HintCallback(string? oldValue, string? newValue);
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static unsafe void UmnmagedHintCallback(IntPtr userdata, byte* name, byte* oldValue, byte* newValue) {
        var pin = new Pin<Hint>(userdata);
        if (pin.TryGetTarget(out var hint)) {
            var old = Marshal.PtrToStringAnsi((IntPtr)oldValue);
            var _new = Marshal.PtrToStringAnsi((IntPtr)newValue);
            hint.InvokeCallbacks(old, _new);
        }
    }

    public static void ResetAll() => SDL_ResetHints();

}