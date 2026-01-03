using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Plays and control multiple children sound as one.
/// </summary>
[GlobalClass, Tool]
public partial class AUD_Layerer : AUD_Module
{
    private List<AUD_Sound> _layers;
    /// <summary>
    /// On AUD_Layerer, Finished fires when all the layered sounds have themselves Finished.
    /// </summary>
    public override event Action Finished;
    private int _playingSounds = 0;

    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    private void AddLayer(AUD_Sound layer)
    {
        layer.Finished += TrackLayerLifetime;
        _layers.Add(layer);
    }

    private void RemoveLayer(AUD_Sound layer)
    {
        layer.Finished -= TrackLayerLifetime;
        _layers.Remove(layer);
    }

    private void ClearLayers()
    {
        if (_layers == null)
        {
            _layers = [];
            return;
        }
            
        for (int i = _layers.Count - 1; i >= 0; i--)
            RemoveLayer(_layers[i]);
    }

    private void SetLayers(List<AUD_Sound> layers)
    {
        ClearLayers();
        foreach(AUD_Sound layer in layers)
            AddLayer(layer);
    }

    protected override void ModuleEnterTree()
    {
        ClearLayers();
        foreach (Node node in GetChildren())
            if (node is AUD_Sound sound)
                AddLayer(sound);
    }

    protected override void OnSoundChildChanged(List<AUD_Sound> sounds) =>
        SetLayers(sounds);
    
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
        _playingSounds = _layers.Count;
        foreach (AUD_Sound layer in _layers)
            layer.Play();
    }

    public override void Stop()
    {
        foreach (AUD_Sound layer in _layers)
            layer.Stop();
    }

    private void TrackLayerLifetime()
    {
        if (-- _playingSounds == 0)
            Finished?.Invoke();
    }
}