using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace GaudioProcessTree.Nodes.Modules;
/// <summary>
/// Plays a random sound on a given player, with pitchScale randomization. <br/>
/// It does not play a random AUD_Sound ! Its child must be an AUD_StreamPlayer. <br/>
/// Otherwize, that would imply the use of many distinct players for such a common pattern.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Finished"/> is a direct forward of its player Finished event.
/// </para>
/// </remarks>
[GlobalClass, Tool, Icon("res://addons/GaudioProcessTree/icons/gaudio_randomizer.svg")]
public partial class AUD_Randomizer : AUD_Module
{
    private AUD_StreamPlayer _player = null;
    public AUD_StreamPlayer Player
    {
        get => _player;
        private set
        {
            if (_player != null)
                _player.Finished -= ForwardFinished;

            _player = value;
            if (_player != null)
                _player.Finished += ForwardFinished;
        }
    }

    protected Array<AudioStream> _sounds;

    [Export] public Array<AudioStream> Sounds
    {
        get => _sounds;
        protected set
        {
            _sounds = value;
            UpdateConfigurationWarnings();
        }
    }

    private float _minPitch = 1f;
    [Export(PropertyHint.Range, "0.1,5,exp,or_greater,or_less")]
    protected float MinPitch
    {
        get => _minPitch;
        set => _minPitch = Mathf.Clamp(value, MIN_PITCH, _maxPitch);
    }
    
    private float _maxPitch = 1f;
    [Export(PropertyHint.Range, "0.1,5,exp,or_greater,or_less")]
    protected float MaxPitch
    {
        get => _maxPitch;
        set => _maxPitch = Mathf.Max(value, _minPitch);
    }

    private float _randomPitch = 1f;

    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    protected override void ModuleEnterTree()
    {
        Player = null;
        foreach (Node node in GetChildren())
            if (node is AUD_StreamPlayer player)
            {
                Player = player;
                return;
            }
    }

    protected override void OnSoundChildChanged(List<AUD_Sound> sounds)
    {
        Player = null;
        foreach (AUD_Sound sound in sounds)
            if (sound is AUD_StreamPlayer player)
            {
                Player = player;
                return;
            }
    }

    // +-------------------+
    // |  CONFIG WARNINGS  |
    // +-------------------+
    // _____________________
    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];

        if (Player == null)
            warnings.Add("This node has no Stream Player.\nConsider adding an AUD_StreamPlayer as a child.");
        if (TooManyStreamPlayer())
            warnings.Add("This node has multiple Stream Players.\nIt will only support one of them.");
        if (NoAudioStreams())
            warnings.Add("AudioStreams must be provided for this node to function.\nPlease provide at least one stream in its list of sounds.");

        return [.. warnings];
    }

    private bool TooManyStreamPlayer()
    {
        bool found = false;
        foreach (Node node in GetChildren())
            if (node is AUD_StreamPlayer)
                if (found)
                    return true;
                else
                    found = true;

        return false;
    }

    private bool NoAudioStreams()
    {
        if (_sounds == null)
            return true;

        foreach (AudioStream stream in _sounds)
            if (stream != null)
                return false;
        
        return true;
    }

    // +-------------------+
    // |  MODULE BEHAVIOR  |
    // +-------------------+
    // _____________________
    protected override void SetBaseVolumeDb(float baseVolumeDb)
    {
        if (Player == null) return;
        Player.RelativeVolumeDb = AbsVolumeDbFromBase(baseVolumeDb);
    }

    protected override void SetRelativeVolumeDb(float relativeVolumeDb)
    {
        if (Player == null) return;
        Player.RelativeVolumeDb = AbsVolumeDbFromRelative(relativeVolumeDb);
    }

    protected override void SetBasePitchScale(float basePitchScale)
    {
        if (Player == null) return;
        Player.RelativePitchScale = AbsPitchFromBase(basePitchScale) * _randomPitch;
    }
    protected override void SetRelativePitchScale(float relativePitchScale)
    {
        if (Player == null) return;
        Player.RelativePitchScale = AbsPitchFromRelative(relativePitchScale) * _randomPitch;
    }

    public override void Play()
    {
        Player.Stream = _sounds.PickRandom();
        _randomPitch = (float)GD.RandRange(_minPitch, _maxPitch);
        Player.RelativePitchScale = _randomPitch * PitchScale;
        Player.Play();
    }

    public override void Stop() => Player.Stop();

    protected override void PitchTimeScale() =>
        _player.RelativePitchScale = AbsolutePitchScale * _randomPitch;
}