using System.Collections.Generic;
using Godot;

namespace GaudioProcessTree.Nodes.Modules;
/// <summary>
/// Scales the pitch of its child with Engine TimeScale.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Finished"/> forwards its child Finished event.
/// </para>
/// </remarks>
[GlobalClass, Tool, Icon("res://addons/GaudioProcessTree/icons/gaudio_time_scaler.svg")]
public partial class AUD_TimeScaler : AUD_Module
{
    private AUD_Sound _sound = null;
    public AUD_Sound Sound
    {
        get => _sound;
        private set
        {
            if (_sound != null)
                _sound.Finished -= ForwardFinished;
            
            _sound = value;
            if (_sound != null)
                _sound.Finished += ForwardFinished;
        }
    }

    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    protected override void ModuleEnterTree()
    {
        Sound = null;
        foreach (Node node in GetChildren())
            if (node is AUD_Sound sound)
            {
                Sound = sound;
                return;
            }
    }
    
    protected override void OnSoundChildChanged(List<AUD_Sound> sounds)
    {
        if (sounds.Count == 0)
            Sound = null;
        else
            Sound = sounds[0];
    }

    // +-------------------+
    // |  CONFIG WARNINGS  |
    // +-------------------+
    // _____________________
    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];

        if (Sound == null)
            warnings.Add("This node has no attached Sound to scale.\nConsider adding an AUD_Sound as a child.");
        if (TooManySounds())
            warnings.Add("This node has multiple Sound children. It will only scale one of them.\nConsider using AUD_LayeredSound as a child to delay multiple sounds.");
        if (HasTimeScalerParent())
            warnings.Add("This node's branch already contains an AUD_TimeScaler node.\nChaining AUD_TimeScaler can lead to unexpected behavior as it will apply scaling multiple times.");

        return [.. warnings];
    }

    private bool TooManySounds()
    {
        bool found = false;
        foreach (Node node in GetChildren())
            if (node is AUD_Sound)
                if (found)
                    return true;
                else
                    found = true;

        return false;
    }

    private bool HasTimeScalerParent()
    {
        Node parent = GetParent();
        while (parent is AUD_Module module)
            if (module is AUD_TimeScaler)
                return true;

        return false;
    }

    // +-------------------+
    // |  MODULE BEHAVIOR  |
    // +-------------------+
    // _____________________
    protected override void SetBasePitchScale(float pitchScale)
    {
        if (Sound == null) return;
        Sound.RelativePitchScale = pitchScale * RelativePitchScale * (float)Engine.TimeScale;
    }
    protected override void SetRelativePitchScale(float pitchScale)
    {
        if (Sound == null) return;
        Sound.RelativePitchScale = BasePitchScale * pitchScale * (float)Engine.TimeScale;
    }

    protected override void SetBaseVolumeDb(float volumeDb)
    {
        if (Sound == null) return;
        Sound.RelativeVolumeDb = volumeDb + RelativeVolumeDb;
    }

    protected override void SetRelativeVolumeDb(float volumeDb)
    {
        if (Sound == null) return;
        Sound.RelativeVolumeDb = BaseVolumeDb + volumeDb;
    }

    public override void Play() => _sound.Play();

    public override void Stop() => _sound.Stop();

    public override void _Ready() =>
        SetPhysicsProcess(!Engine.IsEditorHint());

    public override void _PhysicsProcess(double delta) =>
        _sound.RelativePitchScale = PitchScale * (float)Engine.TimeScale;
}