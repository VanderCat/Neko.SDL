using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Neko.Sdl;

/// <summary>
/// A property is a variable that can be created and retrieved by name at runtime.
/// <br/><br/>
/// All properties are part of a property group (SDL_PropertiesID). A property group can be created
/// with the constructor and destroyed with the Dispose function.
/// <br/><br/>
/// Properties can be added to and retrieved from a property group through the following functions:
/// <br/><br/>
/// <see cref="SetPointer"/> and <see cref="GetPointer"/> operate on <see cref="IntPtr"/> pointer types. <br/>
/// <see cref="SetString"/> and <see cref="GetString(SDL.Utf8String,string?)"/> operate on string types.<br/>
/// <see cref="SetNumber"/> and <see cref="GetNumber"/> operate on signed 64-bit integer types.<br/>
/// <see cref="SetFloat"/> and <see cref="GetFloat"/> operate on floating point types.<br/>
/// <see cref="SetBoolean"/> and <see cref="GetBoolean"/> operate on boolean types.<br/>
/// <br/><br/>
/// Properties can be removed from a group by using <see cref="Clear"/>.
/// </summary>
public partial class Properties : IDisposable, IEnumerable<string> {
    public static implicit operator SDL_PropertiesID(Properties p) => (SDL_PropertiesID)p.Id;
    public static explicit operator Properties(SDL_PropertiesID id) => new(id);
    public uint Id { get; } 
    
    public static Properties Global { get; }

    static Properties() {
        var id = SDL_GetGlobalProperties();
        if (id == 0) throw new SdlException("");
        Global = (Properties)id;
    }
    
    /// <summary>
    /// Create a group of properties
    /// </summary>
    /// <remarks>
    /// All properties are automatically destroyed when SDL_Quit() is called.
    /// </remarks>
    public Properties() {
        Id = (uint)SDL_CreateProperties();
        if (Id == 0) throw new SdlException();
    }
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public Properties(uint id) {
        Id = id;
    }
    
    public Properties(SDL_PropertiesID id) {
        Id = (uint)id;
    }

    /// <summary>
    /// Destroy a group of properties
    /// </summary>
    /// <remarks>
    /// All properties are deleted and their cleanup functions will be called, if any
    /// </remarks>
    public void Dispose() {
        SDL_DestroyProperties(this);
    }

    /// <summary>
    /// Copy a group of properties
    /// </summary>
    /// <param name="properties">the destination properties</param>
    /// <remarks>
    /// Copy all the properties from one group of properties to another,
    /// with the exception of properties requiring cleanup (set using <see cref="SetPointerWithCleanup"/>),
    /// which will not be copied. Any property that already exists on dst will be overwritten.
    /// </remarks>
    public void CopyTo(Properties properties) =>
        SDL_CopyProperties(this, properties).ThrowIfError();

    /// <summary>
    /// Clear a property from a group of properties
    /// </summary>
    /// <param name="name">the name of the property to clear</param>
    public void Clear(Utf8String name) => 
        SDL_ClearProperty(this, name).ThrowIfError();
    
    /// <summary>
    /// Get a boolean property from a group of properties
    /// </summary>
    /// <param name="name">the name of the property to query</param>
    /// <param name="defaultValue">the default value of the property</param>
    /// <returns>the value of the property, or default_value if it is not set or not a boolean property</returns>
    public bool GetBoolean(Utf8String name, bool defaultValue) => 
        SDL_GetBooleanProperty(this, name, defaultValue);

    public bool? GetBoolean(Utf8String name) {
        byte a = 0xFF;
        var result = SDL_GetBooleanProperty(this, name, Unsafe.As<byte, SDLBool>(ref a));
        var result1 = Unsafe.As<SDLBool, byte>(ref result);
        if (result1 == 0xFF) return null;
        return result;
    }
    
    public float GetFloat(Utf8String name, float defaultValue) => 
        SDL_GetFloatProperty(this, name, defaultValue);
    
    public long GetNumber(Utf8String name, long defaultValue) => 
        SDL_GetNumberProperty(this, name, defaultValue);
    
    public string GetString(Utf8String name, string defaultValue) => 
#pragma warning disable CS8603 // Possible null reference return.
        SDL_GetStringProperty(this, name, defaultValue);
#pragma warning restore CS8603 // Possible null reference return.
    public string? GetString(Utf8String name) => SDL_GetStringProperty(this, name, null);

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

    /// <summary>
    /// Enumerate the properties contained in a group of properties
    /// </summary>
    /// <returns></returns>
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