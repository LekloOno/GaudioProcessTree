/// <summary>
/// A static tool to abstract the way of retrieving time passed in Msec/Usec. <br/>
/// It can notably enable the use of custom logic to handle time-scale and pauses.
/// </summary>
public static class AUD_Time
{
    public static AUD_ILocalTime Instance {get; set;}
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Miliseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    public static ulong ScaledTicksMsec {get => Instance.LocalScaledTicksMsec;}
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Microseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    public static ulong ScaledTicksUsec {get => Instance.LocalScaledTicksUsec;}
}