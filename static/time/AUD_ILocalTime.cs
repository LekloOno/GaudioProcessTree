namespace GaudioProcessTree.Static.Time;
/// <summary>
/// Base interface to define an instance of time manager.
/// </summary>
public interface AUD_ILocalTime
{
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Miliseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    ulong LocalScaledTicksMsec {get;}
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Microseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    ulong LocalScaledTicksUsec {get;}
}