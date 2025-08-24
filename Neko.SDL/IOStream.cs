using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;

namespace Neko.Sdl;

public unsafe class IOStream : Stream {
    public SDL_IOStream* Handle;
    public IntPtr Pointer => (IntPtr)Handle;

    private static class Native {
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        public static long Size(IntPtr userdata) {
            try {
                var native = userdata.AsPin<Stream>().Target;
                return native.Length;
            }
            catch (Exception e) {
                SDL_SetErrorV(e.ToString().Replace("%", "%%"), null);
                return -1;
            }
        }
        
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        public static long Seek(IntPtr userdata, long offset, SDL_IOWhence whence) {
            try {
                var stream = userdata.AsPin<Stream>().Target;
                return stream.Seek(offset, (SeekOrigin)whence);
            }
            catch (Exception e) {
                SDL_SetErrorV(e.ToString().Replace("%", "%%"), null);
                return -1;
            }
        }
        
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        public static nuint Read(IntPtr userdata, IntPtr ptr, nuint size, SDL_IOStatus* status) {
            try {
                var native = userdata.AsPin<Stream>().Target;
                var result = (nuint)native.Read(new Span<byte>((void*)ptr, (int)size));
                if (result == 0)
                    *status = SDL_IOStatus.SDL_IO_STATUS_EOF;
                return result;
            }
            catch (Exception e) {
                *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
                SDL_SetErrorV(e.ToString().Replace("%", "%%"), null);
                return 0;
            }
        }
        
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        public static nuint Write(IntPtr userdata, IntPtr ptr, nuint size, SDL_IOStatus* status) {
            try {
                var native = userdata.AsPin<Stream>().Target;
                native.Write(new Span<byte>((void*)ptr, (int)size));
                return size;
            }
            catch (Exception e) {
                *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
                SDL_SetErrorV(e.ToString().Replace("%", "%%"), null);
                return 0;
            }
        }
        
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        public static SDLBool Flush(IntPtr userdata, SDL_IOStatus* status) {
            try {
                var native = userdata.AsPin<Stream>().Target; 
                native.Flush();
                return true;
            }
            catch (Exception e) {
                *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
                SDL_SetErrorV(e.ToString().Replace("%", "%%"), null);
                return false;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        public static SDLBool Close(IntPtr userdata) {
            try {
                var native = userdata.AsPin<Stream>().Target;
                native.Dispose();
                return true;
            }
            catch (Exception e) {
                SDL_SetErrorV(e.ToString().Replace("%", "%%"), null);
                return false;
            }
        }

        public static readonly Pin<SDL_IOStreamInterface> Interface = new(new SDL_IOStreamInterface {
            version = (uint)Marshal.SizeOf<SDL_IOStreamInterface>(),
            size = &Size,
            close = &Close,
            flush = &Flush,
            read = &Read,
            seek = &Seek,
            write = &Write,
        });
    }
    
    internal IOStream(SDL_IOStream* handle) {
        Handle = handle;
        if (handle == null)
            throw new NullReferenceException("The provided handle is invalid");
    }

    private Pin<SDL_IOStream>? _pin;
    
    internal IOStream(ref SDL_IOStream obj) {
        _pin = obj.Pin();
        Handle = (SDL_IOStream*)_pin.Pointer;
    }
    
    public static implicit operator SDL_IOStream*(IOStream o) => o.Handle;
    public static implicit operator IOStream(SDL_IOStream* o) => new(o);

    public static IOStream Open(Stream stream) {
        return SDL_OpenIO((SDL_IOStreamInterface*)Native.Interface.Addr, stream.Pin(GCHandleType.Normal).Pointer);
    }
    
    protected void Dispose(bool disposing) {
        _pin?.Dispose();
    }

    public override bool Equals(object? obj) {
        if (obj is IOStream wrapper)
            return Equals(wrapper);
        return false;
    }

    public bool Equals(IOStream wrapper) {
        return this.Handle == wrapper.Handle;
    }
    
    /// <summary>
    /// Flush any buffered data in the stream
    /// </summary>
    /// <remarks>
    /// This function makes sure that any buffered data is written to the stream.
    /// Normally this isn't necessary but if the stream is a pipe or socket it guarantees that any pending data is sent.
    /// </remarks>
    public override void Flush() =>
        SDL_FlushIO(this).ThrowIfError();

    /// <summary>
    /// Query the stream status of an <see cref="IOStream"/>
    /// </summary>
    /// <remarks>
    /// This information can be useful to decide if a short read or write was due to an error, an EOF, or a
    /// non-blocking operation that isn't yet ready to complete.
    /// <br/><br/>
    /// An <see cref="IOStream"/>'s status is only expected to change after a <see cref="Read"/> or <see cref="Write"/> call;
    /// don't expect it to change if you just call this query function in a tight loop.
    /// <br/><br/>
    /// </remarks>
    public IOStatus Status => (IOStatus)SDL_GetIOStatus(this);

    /// <summary>
    /// Get the properties associated with an <see cref="IOStream"/>
    /// </summary>
    public Properties Properties {
        get {
            var result = SDL_GetIOProperties(this);
            if (result == 0) 
                throw new SdlException();
            return (Properties) result;
        }
    }

    public nuint Read(IntPtr ptr, nuint size) {
        var result = SDL_ReadIO(this, ptr, size);
        if (result == 0 && Status == IOStatus.Error)
            throw new SdlException();
        return result;
    }

    public nuint Read(void* ptr, nuint size) {
        return Read((nint)ptr, size);
    }

    public nuint Read<T>(Span<T> span) where T : unmanaged {
        fixed (T* ptr = span)
            return Read(ptr, (nuint)span.Length);
    }

    public override int Read(byte[] buffer, int offset, int count) {
        fixed (byte* ptr = buffer)
            return (int)Read(ptr+offset, (nuint)count);
    }
    
    

    public override long Seek(long offset, SeekOrigin origin) {
        var result = SDL_SeekIO(this, offset, (SDL_IOWhence)origin);
        if (result == -1)
            throw new NotSupportedException(SDL_GetError());
        return result;
    }
    
    public override void SetLength(long value) {
        throw new NotSupportedException();
    }
    
    public nuint Write(IntPtr ptr, nuint size) {
        var result = SDL_WriteIO(this, ptr, size);
        if (result < size && Status == IOStatus.Error)
            throw new SdlException();
        return result;
    }
    
    public nuint Write(void* ptr, nuint size) {
        return Write((nint)ptr, size);
    }
    
    public nuint Write<T>(Span<T> span) where T : unmanaged {
        fixed (T* ptr = span)
            return Read(ptr, (nuint)span.Length);
    }
    
    public override void Write(byte[] buffer, int offset, int count) {
        fixed (byte* ptr = buffer)
            Write(ptr+offset, (nuint)count);
    }
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => true;
    public override long Length => SDL_GetIOSize(this);
    public override long Position {
        get => SDL_TellIO(this);
        set => Seek(value, SeekOrigin.Begin);
    }

    /// <summary>
    /// Close and free an allocated SDL_IOStream structure.
    /// </summary>
    /// <remarks>
    /// <see cref="Close"/> closes and cleans up the <see cref="SDL_IOStream"/> stream.
    /// It releases any resources used by the stream and frees the <see cref="SDL_IOStream"/> itself.
    /// This throws if the stream failed to flush to its output (e.g. to disk).
    /// <br/><br/>
    /// Note that if this fails to flush the stream for any reason, this function reports an error,
    /// but the <see cref="IOStream"/> is still invalid once this function returns.
    /// <br/><br/>
    /// This call flushes any buffered writes to the operating system, but there are no guarantees that those writes
    /// have gone to physical media; they might be in the OS's file cache, waiting to go to disk later.
    /// If it's absolutely crucial that writes go to disk immediately, so they are definitely stored even if the power
    /// fails before the file cache would have caught up, one should call <see cref="Flush"/> before closing. Note that
    /// flushing takes time and makes the system and your app operate less efficiently, so do so sparingly.
    /// </remarks>
    public override void Close() {
        SDL_CloseIO(this).ThrowIfError();
        base.Close();
    }

    //prob won't work
    public static unsafe IOStream FromConstMem(ref ReadOnlySpan<byte> mem) =>
        SDL_IOFromConstMem((IntPtr)Unsafe.AsPointer(ref mem), (uint)mem.Length);
    public static IOStream FromDynamicMem() =>
        SDL_IOFromDynamicMem();
    public static IOStream FromFile(string file, string mode) =>
        SDL_IOFromFile(file, mode);

    public static IOStream FromMem(ref Span<byte> mem) =>
        SDL_IOFromMem((IntPtr)Unsafe.AsPointer(ref mem), (uint)mem.Length);
    public byte[] LoadFile() {
        nuint nya = 0;
        var meow = SDL_LoadFile_IO(this, &nya, false);
        var arr = new byte[nya];
        fixed(byte* ptr = arr)
            Unsafe.CopyBlock(ptr, (void*)meow, (uint)nya);
        UnmanagedMemory.Free(meow);
        return arr;
    }
    public short ReadS16BE() {
        short result = 0;
        SDL_ReadS16BE(this, &result).ThrowIfError();
        return result;
    }
    public short ReadS16LE() {
        short result = 0;
        SDL_ReadS16LE(this, &result).ThrowIfError();
        return result;
    }
    public int ReadS32BE() {
        int result = 0;
        SDL_ReadS32BE(this, &result).ThrowIfError();
        return result;
    }
    public int ReadS32LE() {
        int result = 0;
        SDL_ReadS32LE(this, &result).ThrowIfError();
        return result;
    }
    public long ReadS64BE() {
        long result = 0;
        SDL_ReadS64BE(this, &result).ThrowIfError();
        return result;
    }
    public long ReadS64LE() {
        long result = 0;
        SDL_ReadS64LE(this, &result).ThrowIfError();
        return result;
    }
    public sbyte ReadS8() {
        sbyte result = 0;
        SDL_ReadS8(this, &result).ThrowIfError();
        return result;
    }
    public ushort ReadU16BE() {
        ushort result = 0;
        SDL_ReadU16BE(this, &result).ThrowIfError();
        return result;
    }
    public ushort ReadU16LE() {
        ushort result = 0;
        SDL_ReadU16LE(this, &result).ThrowIfError();
        return result;
    }
    public uint ReadU32BE() {
        uint result = 0;
        SDL_ReadU32BE(this, &result).ThrowIfError();
        return result;
    }
    public uint ReadU32LE() {
        uint result = 0;
        SDL_ReadU32LE(this, &result).ThrowIfError();
        return result;
    }
    public ulong ReadU64BE() {
        ulong result = 0;
        SDL_ReadU64BE(this, &result).ThrowIfError();
        return result;
    }
    public ulong ReadU64LE() {
        ulong result = 0;
        SDL_ReadU64LE(this, &result).ThrowIfError();
        return result;
    }
    public byte ReadU8() {
        byte result = 0;
        SDL_ReadU8(this, &result).ThrowIfError();
        return result;
    }
    public void SaveFile(byte[] data) {
        fixed (byte* ptr = data)
            SDL_SaveFile_IO(this, (IntPtr)ptr, (nuint)data.Length, false).ThrowIfError();
    }
    public void WriteS16BE(short value) =>
        SDL_WriteS16BE(this, value).ThrowIfError();
    
    public void WriteS16LE(short value) =>
        SDL_WriteS16LE(this, value).ThrowIfError();
    public void WriteS32BE(int value) =>
        SDL_WriteS32BE(this, value).ThrowIfError();
    
    public void WriteS32LE(int value) =>
        SDL_WriteS32LE(this, value).ThrowIfError();
    
    public void WriteS64BE(long value) =>
        SDL_WriteS64BE(this, value).ThrowIfError();
    public void WriteS64LE(long value) =>
        SDL_WriteS64LE(this, value).ThrowIfError();
    
    public void WriteS8(sbyte value) =>
        SDL_WriteS8(this, value).ThrowIfError();
    public void WriteU16BE(ushort value) =>
        SDL_WriteU16BE(this, value).ThrowIfError();
    public void WriteU16LE(ushort value) =>
        SDL_WriteU16LE(this, value).ThrowIfError();
    public void WriteU32BE(uint value) =>
        SDL_WriteU32BE(this, value).ThrowIfError();
    public void WriteU32LE(uint value) =>
        SDL_WriteU32LE(this, value).ThrowIfError();
    public void WriteU64BE(ulong value) =>
        SDL_WriteU64BE(this, value).ThrowIfError();
    public void WriteU64LE(ulong value) =>
        SDL_WriteU64LE(this, value).ThrowIfError();
    public void WriteU8(byte value) =>
        SDL_WriteU8(this, value).ThrowIfError();
}