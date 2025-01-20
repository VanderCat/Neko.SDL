using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Extra.Tray;

public sealed unsafe partial class TrayEntry : SdlWrapper<SDL_TrayEntry> {
    public bool IsChecked {
        get => SDL_GetTrayEntryChecked(this);
        set => SDL_SetTrayEntryChecked(this, value);
    }

    public bool IsEnabled {
        get => SDL_GetTrayEntryEnabled(this);
        set => SDL_SetTrayEntryEnabled(this, value);
    }

    public bool IsSeparator => SDL_GetTrayEntryLabel(this) is null;
    
    public string? Label {
        get => SDL_GetTrayEntryLabel(this);
        set => SDL_SetTrayEntryLabel(this, value);
    }

    public TrayMenu Parent => SDL_GetTrayEntryParent(this);
    public TrayMenu SubMenu => SDL_GetTraySubmenu(this);
    public TrayMenu CreateSubMenu() => SDL_CreateTraySubmenu(this);

    public delegate void TrayCallback(TrayEntry selected);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void NativeCallback(IntPtr userdata, SDL_TrayEntry* entry) {
        var pin = userdata.AsPin<TrayCallback>(false);
        var managedCallback = pin.Target;
        managedCallback(entry);
    }

    private Pin<TrayCallback>? _callback;

    public TrayCallback? Callback {
        get => _callback?.Target;
        set {
            _callback?.Dispose();
            if (value is null) {
                SDL_SetTrayEntryCallback(this, null, 0);
                return;
            }
            _callback = value.Pin(GCHandleType.Normal);
            SDL_SetTrayEntryCallback(this, &NativeCallback, _callback.Pointer);
        }
    }

}