using System.Collections.Generic;
using Godot;

namespace GaudioProcessTree.Nodes.Modules;
/// <summary>
/// Plays a sound with a given delay in seconds.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Finished"/> fires when the delayed sound completes, or when the delayed play got cancel before the sound could actually start playing.
/// </para>
/// </remarks>
[GlobalClass, Tool, Icon("res://addons/GaudioProcessTree/icons/gaudio_delayer.svg")]
public partial class AUD_Delayer : AUD_Module
{
    [Export(PropertyHint.Range, "0,10,exp,or_greater")]
    private double _delay;

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

    private Timer _timer;

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
            warnings.Add("This node has no attached Sound to delay.\nConsider adding an AUD_Sound as a child.");
        if (TooManySounds())
            warnings.Add("This node has multiple Sound children. It will only delay one of them.\nConsider using AUD_LayeredSound as a child to delay multiple sounds.");

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

    // +-------------------+
    // |  MODULE BEHAVIOR  |
    // +-------------------+
    // _____________________
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

    protected override void ReadySpec()
    {
        if (Engine.IsEditorHint())
            return;

        _timer = new(){
            OneShot = true,
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics
        };

        AddChild(_timer);
        _timer.Timeout += DeferredPlay;
    }

    public override void Play() => _timer.Start(_delay);
    private void DeferredPlay() => Sound.Play();

    public override void Stop()
    {
        if (_timer.IsStopped())
            _sound.Stop();
        else
        {
            _timer.Stop();
            ForwardFinished();
        }
    }

    protected override void PropagateTimeScale()
    {
        _sound?.UpdateEffectiveTimeScale();
    }
}