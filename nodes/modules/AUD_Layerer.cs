using System.Collections.Generic;
using Godot;

namespace GaudioProcessTree.Nodes.Modules;
/// <summary>
/// Plays and control multiple children sound as one.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Finished"/> fires once every sound layer have themselves Finished.
/// </para>
/// </remarks>
[GlobalClass, Tool, Icon("res://addons/GaudioProcessTree/icons/gaudio_layerer.svg")]
public partial class AUD_Layerer : AUD_Module
{
    protected LinkedList<AUD_Sound> _layers;
    protected int _playingSounds = 0;

    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    private void AddLayer(AUD_Sound layer)
    {
        layer.Finished += TrackLayerLifetime;
        _layers.AddLast(layer);
    }

    private void RemoveLayer(AUD_Sound layer)
    {
        layer.Finished -= TrackLayerLifetime;
        _layers.Remove(layer);
    }

    private void Pop()
    {
        AUD_Sound layer = _layers.Last.Value;
        layer.Finished -= TrackLayerLifetime;
        _layers.RemoveLast();
    }

    private void ClearLayers()
    {
        if (_layers == null)
        {
            _layers = [];
            return;
        }
            
        for (int i = _layers.Count - 1; i >= 0; i--)
            Pop();
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
    protected override void SetBaseVolumeDb(float baseVolumeDb) =>
        SetLayersVolumeDb(AbsVolumeDbFromBase(baseVolumeDb));

    protected override void SetRelativeVolumeDb(float relativeVolumeDb) =>
        SetLayersVolumeDb(AbsVolumeDbFromRelative(relativeVolumeDb));

    private void SetLayersPitchScale(float pitchScale)
    {
        if (_layers == null)
            return;
            
        foreach (AUD_Sound layer in _layers)
            layer.RelativePitchScale = pitchScale;
    }
    protected override void SetBasePitchScale(float basePitchScale) =>
        SetLayersPitchScale(AbsPitchFromBase(basePitchScale));

    protected override void SetRelativePitchScale(float relativePitchScale) =>
        SetLayersPitchScale(AbsPitchFromRelative(relativePitchScale));

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
            ForwardFinished();
    }

    protected override void PropagateTimeScale()
    {
        foreach (AUD_Sound layer in _layers)
            layer.UpdateEffectiveTimeScale();
    }
}