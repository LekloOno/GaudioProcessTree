using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_LayeredSound : AUD_Wrapper
{
    record OriginalSettings(float VolumeDb, float PitchScale);

    [Export] private Array<AUD_Sound> _layers;

    private void SetLayersVolumeDb(float volumeDb)
    {
        foreach (AUD_Sound layer in _layers)
            layer.RelativeVolumeDb = volumeDb;
    }
    protected override void SetBaseVolumeDb(float volumeDb) =>
        SetLayersVolumeDb(volumeDb + RelativeVolumeDb);

    protected override void SetRelativeVolumeDb(float volumeDb) =>
        SetLayersVolumeDb(BaseVolumeDb + volumeDb);

    private void SetLayersPitchScale(float pitchScale)
    {
        foreach (AUD_Sound layer in _layers)
            layer.RelativePitchScale = pitchScale;
    }
    protected override void SetBasePitchScale(float pitchScale) =>
        SetLayersPitchScale(pitchScale * RelativePitchScale);

    protected override void SetRelativePitchScale(float pitchScale) =>
        SetLayersPitchScale(BasePitchScale * pitchScale);

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