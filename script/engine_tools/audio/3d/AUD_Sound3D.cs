using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_Sound3D : AUD_Sound
{
    [Export] private AudioStreamPlayer3D _player;
    [Export] private Array<AudioStream> _sounds = new();
    [Export] private float _minPitch = 1f;
    [Export] private float _maxPitch = 1f;
    
    public override void Play()
    {
        _player.Stream = _sounds.PickRandom();
        _player.PitchScale = (float)GD.RandRange(_minPitch, _maxPitch);
        _player.Play();
    }

    public override void Stop() => _player.Stop();
}