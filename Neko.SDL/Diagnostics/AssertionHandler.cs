namespace Neko.Sdl.Diagnostics;

/// <summary>
/// A callback that fires when an SDL assertion fails.
/// </summary>
/// <remarks>
/// This callback may be called from any thread that triggers an assert at any time.
/// </remarks>
/// <returns>
/// An <see cref="AssertState"/> value indicating how to handle the failure.
/// </returns>
public delegate AssertState AssertionHandler(AssertData data);