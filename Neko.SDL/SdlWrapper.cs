namespace Neko.Sdl;

public abstract unsafe class SdlWrapper<T> : IDisposable where T : unmanaged {
    public T* Handle;
    public IntPtr Pointer => (IntPtr)Handle;
    internal SdlWrapper(){}

    internal SdlWrapper(T* handle) {
        Handle = handle;
        //TODO: throw if null
    }

    private Pin<T>? _pin;
    
    internal SdlWrapper(ref T obj) {
        _pin = obj.Pin();
        Handle = (T*)_pin.Pointer;
    }
    
    public static implicit operator T*(SdlWrapper<T> o) => o.Handle;
    
    public virtual void Dispose() {
        GC.SuppressFinalize(this);
        _pin?.Dispose();
    }

    public override bool Equals(object? obj) {
        if (obj is SdlWrapper<T> wrapper)
            return Equals(wrapper);
        return false;
    }

    public bool Equals(SdlWrapper<T> wrapper) {
        return this.Handle == wrapper.Handle;
    }
}