using System.Collections.Generic;
using Godot;
using Godot.Collections;

/// <summary>
/// Plays a random sound on a given player, with pitchScale randomization. <br/>
/// It does not play a random AUD_Sound ! Its child must be an AUD_StreamPlayer. <br/>
/// Otherwize, that would imply the use of many distinct players for such a common pattern.
/// </summary>
[GlobalClass, Tool]
public partial class AUD_RandomSound : AUD_Module
{
    protected AUD_StreamPlayer _player;
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
        _player = null;
        foreach (Node node in GetChildren())
            if (node is AUD_StreamPlayer player)
            {
                _player = player;
                return;
            }
    }

    protected override void OnSoundChildChanged(List<AUD_Sound> sounds)
    {
        _player = null;
        foreach (AUD_Sound sound in sounds)
            if (sound is AUD_StreamPlayer player)
            {
                _player = player;
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

        if (_player == null)
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
    protected override void SetBaseVolumeDb(float volumeDb)
    {
        if (_player == null) return;
        _player.RelativeVolumeDb = volumeDb + RelativeVolumeDb;
    }

    protected override void SetRelativeVolumeDb(float volumeDb)
    {
        if (_player == null) return;
        _player.RelativeVolumeDb = BaseVolumeDb + volumeDb;
    }

    protected override void SetBasePitchScale(float pitchScale)
    {
        if (_player == null) return;
        _player.RelativePitchScale = pitchScale * RelativePitchScale * _randomPitch;
    }
    protected override void SetRelativePitchScale(float pitchScale)
    {
        if (_player == null) return;
        _player.RelativePitchScale = BasePitchScale * pitchScale * _randomPitch;
    }

    public override void Play()
    {
        _player.Stream = _sounds.PickRandom();
        _randomPitch = (float)GD.RandRange(_minPitch, _maxPitch);
        _player.RelativePitchScale = _randomPitch * PitchScale;
        _player.Play();
    }

    public override void Stop() => _player.Stop();
}