using System.Collections.Generic;
using Godot;

/// <summary>
/// Plays and control multiple children sound as one.
/// </summary>
[GlobalClass, Tool]
public partial class AUD_LayeredSound : AUD_Module
{
    private List<AUD_Sound> _layers;

    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    protected override void ModuleEnterTree()
    {
        _layers = [];
        foreach (Node node in GetChildren())
            if (node is AUD_Sound sound)
                _layers.Add(sound);
    }

    protected override void OnSoundChildChanged(List<AUD_Sound> sounds) =>
        _layers = sounds;
    
    // +-------------------+
    // |  CONFIG WARNINGS  |
    // +-------------------+
    // _____________________
    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];

        if (_layers == null || _layers.Count == 0)
            warnings.Add("This node has no attached Sound to layer.\nConsider adding at least one AUD_Sound as a child.");

        return [.. warnings];
    }

    // +-------------------+
    // |  MODULE BEHAVIOR  |
    // +-------------------+
    // _____________________
    private void SetLayersVolumeDb(float volumeDb)
    {
        if (_layers == null)
            return;

        foreach (AUD_Sound layer in _layers)
            layer.RelativeVolumeDb = volumeDb;
    }
    protected override void SetBaseVolumeDb(float volumeDb) =>
        SetLayersVolumeDb(volumeDb + RelativeVolumeDb);

    protected override void SetRelativeVolumeDb(float volumeDb) =>
        SetLayersVolumeDb(BaseVolumeDb + volumeDb);

    private void SetLayersPitchScale(float pitchScale)
    {
        if (_layers == null)
            return;
            
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