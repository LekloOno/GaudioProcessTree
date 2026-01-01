using Godot;

[GlobalClass, Tool]
public abstract partial class AUD_StreamPlayer : AUD_Sound
{
    public abstract AudioStream Stream {get; set;}
    public abstract StringName Bus {get; set;}
    public abstract AudioStreamPlayback GetStreamPlayBack();
    protected override void SetBaseVolumeDb(float volumeDb) =>
        VolumeDb = volumeDb + RelativeVolumeDb;

    protected override void SetBasePitchScale(float pitchScale) =>
        PitchScale = pitchScale * RelativePitchScale;

    protected override void SetRelativeVolumeDb(float volumeDb) =>
        VolumeDb = BaseVolumeDb + volumeDb;

    protected override void SetRelativePitchScale(float pitchScale) =>
        PitchScale = BasePitchScale * pitchScale;
}