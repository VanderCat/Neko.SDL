using System.Runtime.CompilerServices;

namespace Neko.Sdl.Extra.StandardLibrary;

public static class Math {
    public static int Abs(int x) => SDL_abs(x);
    public static double Acos(double x) => SDL_acos(x);
    public static float Acos(float x) => SDL_acosf(x);
    public static double Asin(double x) => SDL_asin(x);
    public static float Asin(float x) => SDL_asinf(x);
    public static double Atan(double x) => SDL_atan(x);
    public static float Atan(float x) => SDL_atanf(x);
    public static double Atan2(double y, double x) => SDL_atan2(y, x);
    public static float Atan2(float y, float x) => SDL_atan2f(y, x);
    public static double Ceil(double x) => SDL_ceil(x);
    public static float Ceil(float x) => SDL_ceilf(x);
    public static double CopySign(double x, double y) => SDL_copysign(x, y);
    public static float CopySign(float x, float y) => SDL_copysignf(x, y);
    public static double Cos(double x) => SDL_cos(x);
    public static float Cos(float x) => SDL_cosf(x);
    public static double Sin(double x) => SDL_sin(x);
    public static float Sin(float x) => SDL_sinf(x);
    public static double Tan(double x) => SDL_tan(x);
    public static float Tan(float x) => SDL_tanf(x);
    public static double Exp(double x) => SDL_exp(x);
    public static float Exp(float x) => SDL_expf(x);
    public static double Abs(double x) => SDL_fabs(x);
    public static float Abs(float x) => SDL_fabsf(x);
    public static double Floor(double x) => SDL_floor(x);
    public static float Floor(float x) => SDL_floorf(x);
    public static double Mod(double x, double y) => SDL_fmod(x, y);
    public static float Mod(float x, float y) => SDL_fmodf(x, y);
    public static double Log(double x) => SDL_log(x);
    public static float Log(float x) => SDL_logf(x);
    public static double Log10(double x) => SDL_log10(x);
    public static float Log10(float x) => SDL_log10f(x);
    public static long Lround(double x) => SDL_lround(x);
    public static long Lround(float x) => SDL_lroundf(x);

    public static unsafe (double, double) Modf(double x) {
        double integer = 0.0;
        return (SDL_modf(x, &integer), integer);
    }
    public static unsafe (float, float) Modf(float x) {
        float integer = 0.0f;
        return (SDL_modff(x, &integer), integer);
    }

    public static double Pow(double x, double y) => SDL_pow(x, y);
    public static float Pow(float x, float y) => SDL_powf(x, y);
    public static uint Random() => SDL_rand_bits();
    public static unsafe uint Random(ref ulong state) => SDL_rand_bits_r((ulong*)Unsafe.AsPointer(ref state));
    public static int Random(int n) => SDL_rand(n);
    public static unsafe int Random(ref ulong state, int n) => SDL_rand_r((ulong*)Unsafe.AsPointer(ref state), n);
    public static float RandomF() => SDL_randf();
    public static unsafe float RandomF(ref ulong state) => SDL_randf_r((ulong*)Unsafe.AsPointer(ref state));
    public static double Round(double x) => SDL_round(x);
    public static float Round(float x) => SDL_roundf(x);
    public static double Scalbn(double x, int n) => SDL_scalbn(x, n);
    public static float Scalbn(float x, int n) => SDL_scalbnf(x, n);
    public static double Sqrt(double x) => SDL_sqrt(x);
    public static float Sqrt(float x) => SDL_sqrtf(x);
    public static void RandomSeed(ulong seed) => SDL_srand(seed);
    
}