using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Neko.Sdl;

public sealed partial class Properties : IDisposable, IEnumerable<string> {
    public static implicit operator SDL_PropertiesID(Properties p) => (SDL_PropertiesID)p.Id;
    public static explicit operator Properties(SDL_PropertiesID id) => new(id);
    public uint Id { get; } 
    
    public static Properties Global { get; }

    static Properties() {
        var id = SDL_GetGlobalProperties();
        if (id == 0) throw new SdlException("");
        Global = (Properties)id;
    }
    
    public Properties() {
        Id = (uint)SDL_CreateProperties();
    }
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public Properties(uint id) {
        Id = id;
    }
    
    public Properties(SDL_PropertiesID id) {
        Id = (uint)id;
    }

    public void Dispose() {
        SDL_DestroyProperties(this);
    }

    public void CopyTo(Properties properties) =>
        SDL_CopyProperties(this, properties).ThrowIfError();

    public void Clear(Utf8String name) => 
        SDL_ClearProperty(this, name).ThrowIfError();
    
    public bool GetBoolean(Utf8String name, bool defaultValue) => 
        SDL_GetBooleanProperty(this, name, defaultValue);
    
    public float GetFloat(Utf8String name, float defaultValue) => 
        SDL_GetFloatProperty(this, name, defaultValue);
    
    public long GetNumber(Utf8String name, long defaultValue) => 
        SDL_GetNumberProperty(this, name, defaultValue);
    
    public string GetString(Utf8String name, string defaultValue) => 
#pragma warning disable CS8603 // Possible null reference return.
        SDL_GetStringProperty(this, name, defaultValue);
#pragma warning restore CS8603 // Possible null reference return.
    
    public IntPtr GetPointer(Utf8String name, IntPtr defaultValue) => 
        SDL_GetPointerProperty(this, name, defaultValue);
    
    public void SetBoolean(Utf8String name, bool value) => 
        SDL_SetBooleanProperty(this, name, value).ThrowIfError();
    
    public void SetFloat(Utf8String name, float value) => 
        SDL_SetFloatProperty(this, name, value).ThrowIfError();
    
    public void SetNumber(Utf8String name, long value) => 
        SDL_SetNumberProperty(this, name, value).ThrowIfError();
    
    public void SetString(Utf8String name, string value) => 
        SDL_SetStringProperty(this, name, value).ThrowIfError();
    
    public void SetPointer(Utf8String name, IntPtr value) => 
        SDL_SetPointerProperty(this, name, value).ThrowIfError();
    
    public void SetPointerWithCleanup(Utf8String name, IntPtr value) => 
        throw new NotImplementedException();

    public PropertyType GetUnderlyingType(Utf8String name) =>
        (PropertyType)(int)SDL_GetPropertyType(this, name);

    public void Lock() => SDL_LockProperties(this).ThrowIfError();
    public void Unlock() => SDL_UnlockProperties(this);

    public IEnumerator<string> GetEnumerator() =>
        new PropertiesEnumerator(this);

    public void HasProperty(Utf8String name) => SDL_HasProperty(this, name);

    public override bool Equals(object? obj) {
        return obj is Properties properties && Equals(properties);
    }

    private bool Equals(Properties other) {
        return Id == other.Id;
    }

    public override int GetHashCode() {
        return (int)Id;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

internal unsafe class PropertiesEnumerator : IEnumerator<string> {
    private List<string> _list = new();
    public PropertiesEnumerator(Properties properties) {
        var pin = this.Pin();
        SDL_EnumerateProperties(properties, &Callback, pin.Pointer);
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Callback(IntPtr userdata, SDL_PropertiesID props, byte* name) {
        var pin = new Pin<PropertiesEnumerator>(userdata);
        
        if (!pin.TryGetTarget(out var enumerator)) return;
        
        var str = Marshal.PtrToStringAnsi((IntPtr)name);
        if (str is not null)
            enumerator._list.Add(str);
    }

    public bool MoveNext() => Cursor++ < _list.Count;

    public void Reset() => Cursor = 0;

    public int Cursor { get; set; }
    public string Current => _list[Cursor];

    object IEnumerator.Current => Current;

    public void Dispose() { }
}