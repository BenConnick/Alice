using System.Runtime.CompilerServices;
using UnityEngine;

public static class Trig
{
    private const int numDegrees = 360;
    private static float[] cosineQuarterDegreeCache = new float[numDegrees * 4];
    private static float[] sineQuarterDegreeCache = new float[numDegrees * 4];
    private static bool trigPrecomputed = false;

    private static void PrecomputeTrig()
    {
        for (int i = 0; i < numDegrees * 4; i++)
        {
            float angleDegrees = i * 0.25f;
            cosineQuarterDegreeCache[i] = Mathf.Cos(Mathf.Deg2Rad * angleDegrees);
            sineQuarterDegreeCache[i] = Mathf.Sin(Mathf.Deg2Rad * angleDegrees);
        }

        trigPrecomputed = true;
    }

    // Gets a Cosine faster than calling Mathf.Cos. Hopefully the compiler inlines this, or else it might not be faster. If you find the compiler is not inlining the cache functions, you can inline them manually
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(float degrees)
    {
        if (!trigPrecomputed) PrecomputeTrig();
        if (degrees < 0)
        {
            degrees = -degrees % 360;
            degrees = 360 - degrees;
        }
        return cosineQuarterDegreeCache[Mathf.FloorToInt(degrees * 4) % sineQuarterDegreeCache.Length];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Same as above but for Sine
    public static float Sin(float degrees)
    {
        if (!trigPrecomputed) PrecomputeTrig();
        if (degrees < 0)
        {
            degrees = -degrees % 360;
            degrees = 360 - degrees;
        }
        return sineQuarterDegreeCache[Mathf.FloorToInt(degrees * 4) % sineQuarterDegreeCache.Length];
    }
}
