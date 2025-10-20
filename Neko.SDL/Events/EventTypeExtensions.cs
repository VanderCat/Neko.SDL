using System.Runtime.CompilerServices;

namespace Neko.Sdl.Events;

public static class EventTypeExtensions {
    /// <inheritdoc cref="Event.Enabled"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Enabled(this EventType eventType) => EventQueue.Enabled(eventType);
    /// <inheritdoc cref="Event.SetEnabled"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetEnabled(this EventType eventType, bool enabled) => EventQueue.SetEnabled(eventType, enabled);
    /// <inheritdoc cref="Event.Flush(EventType)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Flush(this EventType eventType) => EventQueue.Flush(eventType);
}