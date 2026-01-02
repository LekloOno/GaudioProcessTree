using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class AUD_RandomSound : AUD_Wrapper
{
    protected AUD_StreamPlayer _player;
    [Export] protected Array<AudioStream> _sounds;
    [Export] protected float _minPitch = 1f;
    [Export] protected float _maxPitch = 1f;
    private float _randomPitch = 1f;

    protected override void ModuleEnterTree()
    {
        foreach (Node node in GetChildren())
            if (node is AUD_StreamPlayer player)
            {
                _player = player;
                return;
            }
    }


    protected override void OnSoundChildChanged(List<AUD_Sound> sounds)
    {
        foreach (AUD_Sound sound in sounds)
            if (sound is AUD_StreamPlayer player)
            {
                _player = player;
                return;
            }
    }

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