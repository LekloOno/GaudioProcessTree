using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_LayeredSound : AUD_Wrapper
{
    record OriginalSettings(float VolumeDb, float PitchScale);

    [Export] private Array<AUD_Sound> _layers;

    protected override void SetBaseVolumeDb(float volume)
    {
        foreach (AUD_Sound layer in _layers)
            layer.RelativeVolumeDb = VolumeDb;
    }
    protected override void SetRelativeVolumeDb(float volume) =>
        SetBaseVolumeDb(volume);

    protected override void SetBasePitchScale(float pitchScale)
    {
        foreach (AUD_Sound layer in _layers)
            layer.RelativePitchScale = PitchScale;
    }
    protected override void SetRelativePitchScale(float pitchScale) =>
        SetBasePitchScale(pitchScale);

    public override void Play()
    {
        foreach (AUD_Sound layer in _layers)
            layer.Play();
    }

    public override void Stop()
    {
        foreach (AUD_Sound layer in _layers)
            layer.Stop();
    }
}