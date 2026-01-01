public interface AUD_ISound
{
    void Play();
    void Stop();
    /// <summary>
    /// Retrieves the current effective VolumeDb of this sound. <br/>
    /// Cannot be modified directly, you should either modify the RelativeVolumeDb, or if it is exposed in the inspector by the implementing node, the BaseVolumeDb.
    /// </summary>
    float VolumeDb {get;}
    /// <summary>
    /// Retrieves the current effective PitchScale of this sound.
    /// Cannot be modified directly, you should either modify the RelativePitchScale, or if it is exposed in the inspector by the implementing node, the BasePitchScale.
    /// </summary>
    float PitchScale {get;}
    /// <summary>
    /// The original/initial VolumeDb of this sound. <br/>
    /// Could typically be exposed in the inspector by the implementing node.
    /// </summary>
    float BaseVolumeDb {get;}
    /// <summary>
    /// The original/initial PitchScale of this sound. <br/>
    /// Could typically be exposed in the inspector by the implementing node.
    /// </summary>
    float BasePitchScale {get;}
    /// <summary>
    /// Allows to modify the original VolumeDb of the sound, without overriding it. <br/>
    /// Assigning it a value of 0 will set the sound to its original volume.
    /// </summary>
    float RelativeVolumeDb {get; set;}
    /// <summary>
    /// Allows to modify the original PichScale of the sound, without overriding it. <br/>
    /// Assigning it a value of 1 will set the sound to its original pitch.
    /// </summary>
    float RelativePitchScale {get; set;}
}