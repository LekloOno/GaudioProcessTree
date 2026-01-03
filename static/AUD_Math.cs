using Godot;

/// <summary>
/// Some static methods for simple audio-related maths.
/// </summary>
public static class AUD_Math
{
    /// <summary>
    /// Linearly interpolating volume using decibels isn't humanly perceived as linear, since decibels are logarithmic. <br/>
    /// This is a simple helper function to avoid this problem.
    /// It interpolates linearly a volume in linear-scale between decibel-scale values.
    /// </summary>
    /// <param name="fromDb">The start value for interpolation in decibels.</param>
    /// <param name="toDb">The destination value for interpolation in decibels.</param>
    /// <param name="t">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting volume of the interpolation in decibels.</returns>
    public static float LerpDB(float fromDb, float toDb, float t)
    {
        float linear1 = Mathf.DbToLinear(fromDb);
        float linear2 = Mathf.DbToLinear(toDb);
        float linearResult = Mathf.Lerp(linear1, linear2, t);
        return Mathf.LinearToDb(linearResult);
    }
}