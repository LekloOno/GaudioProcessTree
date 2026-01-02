using Godot;

public static class AUD_Math
{
    public static float LerpDB(float dB1, float dB2, float t)
    {
        float linear1 = Mathf.DbToLinear(dB1);
        float linear2 = Mathf.DbToLinear(dB2);
        float linearResult = Mathf.Lerp(linear1, linear2, t);
        return Mathf.LinearToDb(linearResult);
    }
}