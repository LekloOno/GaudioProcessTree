using System.Collections.Generic;
using Godot;
using Godot.Collections;

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

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = base._GetPropertyList();
        for (int i = 0; i < properties.Count; i++)
        {
            Dictionary dict = properties[i];
            if (dict.ContainsKey("name") && (string)dict["name"] == "UseTimeScale")
            {
                properties.RemoveAt(i);
                break;
            }

        }

        return properties;
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
        if (HasTimeScalerParent(out string chainedScaleWarning))
            warnings.Add(chainedScaleWarning);

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

    private bool HasTimeScalerParent(out string warning)
    {
        Node parent = GetParent();
        while (parent is AUD_Module module)
        {
            if (module.UseTimeScale)
            {
                warning = "This node's root already uses pitch time scaling.\nChaining time scaling can lead to unexpected behavior as it will apply scaling multiple times.";
                return true;
            }
            if (module is AUD_TimeScaler)
            {
                warning = "This node's branch already contains an AUD_TimeScaler node.\nChaining AUD_TimeScaler can lead to unexpected behavior as it will apply scaling multiple times.";
                return true;
            }
            parent = parent.GetParent();
        }

        warning = "";
        return false;
    }

    // +-------------------+
    // |  MODULE BEHAVIOR  |
    // +-------------------+
    // _____________________
    /// <summary>
    /// Overrides UseTimeScale to always be true, even if it is not a root node.
    /// </summary>
    protected override void ReadySpec() =>
        UseTimeScale = true;

    protected override void SetBasePitchScale(float basePitchScale)
    {
        if (Sound == null) return;
        Sound.RelativePitchScale = AbsPitchFromBase(basePitchScale);
    }
    protected override void SetRelativePitchScale(float relativePitchScale)
    {
        if (Sound == null) return;
        Sound.RelativePitchScale = AbsPitchFromRelative(relativePitchScale);
    }

    protected override void SetBaseVolumeDb(float baseVolumeDb)
    {
        if (Sound == null) return;
        Sound.RelativeVolumeDb = AbsVolumeDbFromBase(baseVolumeDb);
    }

    protected override void SetRelativeVolumeDb(float relativeVolumeDb)
    {
        if (Sound == null) return;
        Sound.RelativeVolumeDb = AbsVolumeDbFromRelative(relativeVolumeDb);
    }

    public override void Play() => _sound.Play();

    public override void Stop() => _sound.Stop();

    protected override void PropagateTimeScale()
    {
        _sound.UpdateEffectiveTimeScale();
    }
}