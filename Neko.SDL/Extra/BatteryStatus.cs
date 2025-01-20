namespace Neko.Sdl.Extra;

public static unsafe class BatteryStatus {
    private static int _timeLeft;
    private static int _chargePercent;
    private static PowerState _powerState;

    public static PowerState PowerState => _powerState;
    public static int TimeLeft => _timeLeft;
    public static int ChargePercent => _chargePercent;

    public static void Update() {
        fixed(int* seconds = &_timeLeft)
        fixed (int* percent = &_chargePercent)
            _powerState = (PowerState)SDL_GetPowerInfo(seconds, percent);
        if (_powerState == PowerState.Error) throw new SdlException("");
    }
}