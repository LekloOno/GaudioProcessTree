public interface AUD_ISound
{
    void Play();
    void Stop();
    float VolumeDb {get;}
    float PitchScale {get;}
    float BaseVolumeDb {get;}
    float BasePitchScale {get;}
    float RelativeVolumeDb {get; set;}
    float RelativePitchScale {get; set;}
}