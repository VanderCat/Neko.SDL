namespace Neko.Sdl.Extra.Tray;

public unsafe partial class Tray : SdlWrapper<SDL_Tray> {
    public Tray(Video.Surface icon, string tooltip) {
        Handle = SDL_CreateTray(icon, tooltip);
        _icon = icon;
        _tooltip = tooltip;
    }

    public TrayMenu CreateMenu() =>
        SDL_CreateTrayMenu(this);

    public TrayMenu Menu {
        get => SDL_GetTrayMenu(this);
    }

    private Video.Surface _icon;
    private string _tooltip;

    public Video.Surface Icon {
        get => _icon;
        set {
            SDL_SetTrayIcon(this, value);
            _icon = value;
        }
    }

    public string Tooltip {
        get => _tooltip;
        set {
            SDL_SetTrayTooltip(this, value);
            _tooltip = value;
        }
    }

    public override void Dispose() {
        base.Dispose();
        SDL_DestroyTray(this);
    }
}