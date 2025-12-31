using Godot;

[GlobalClass]
public abstract partial class AUD_Sound : Node, AUD_ISound
{
    public abstract void Play();
    public abstract void Stop();
    /// <summary>
    /// Retrieves the current effective VolumeDb of this sound. <br/>
    /// Cannot be modified directly, you should either modify the RelativeVolumeDb, or BaseVolumeDb.
    /// </summary>
    public abstract float VolumeDb {get; protected set;}
    /// <summary>
    /// Retrieves the current effective PitchScale of this sound.
    /// Cannot be modified directly, you should either modify the RelativePitchScale, or BasePitchScale.
    /// </summary>
    public abstract float PitchScale {get; protected set;}
    /// <summary>
    /// Allows to modify the original VolumeDb of the sound, without overriding it. <br/>
    /// Assigning it a value of 0 will have no effect.
    /// </summary>
    public abstract float RelativeVolumeDb {get; set;}
    /// <summary>
    /// Allows to modify the original PichScale of the sound, without overriding it. <br/>
    /// Assigning it a value of 1 will have no effect.
    /// </summary>
    public abstract float RelativePitchScale {get; set;}
    /// <summary>
    /// The original/initial VolumeDb of this sound. <br/>
    /// </summary>
    public abstract float BaseVolumeDb {get; protected set;}
    /// <summary>
    /// The original/initial PitchScale of this sound. <br/>
    /// </summary>
    public abstract float BasePitchScale {get; protected set;}
}