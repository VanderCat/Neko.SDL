namespace Neko.Sdl.Extra.Tray;

public unsafe partial class TrayMenu : SdlWrapper<SDL_TrayMenu> {
    public Tray ParentTray => SDL_GetTrayMenuParentTray(this);

    public TrayEntry? ParentEntry {
        get {
            var ptr = SDL_GetTrayMenuParentEntry(this);
            if (ptr is null) return null;
            return ptr;
        }
    }

    public TrayEntry[] Entries {
        get {
            int size;
            var ptr = SDL_GetTrayEntries(this, &size);
            var entries = new TrayEntry[size];
            for (var i = 0; i < size; i++) {
                entries[i] = ptr[i];
            }
            return entries;
        }
    }

    public TrayEntry InsertTrayEntryAt(int pos, string label, TrayEntryFlags flags) =>
        SDL_InsertTrayEntryAt(this, pos, label, (SDL_TrayEntryFlags)flags);

    public void Remove(TrayEntry entry) => SDL_RemoveTrayEntry(entry);
}