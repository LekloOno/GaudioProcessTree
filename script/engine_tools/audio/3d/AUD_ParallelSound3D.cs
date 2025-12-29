using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_ParallelSound3D : AUD_Sound
{
    [Export] private Array<AudioStreamPlayer3D> _subPlayers = new();
    [Export] private float _minPitch = 1f;
    [Export] private float _maxPitch = 1f;
    
    protected float _pitchBaseDelta;

    public override void Play()
    {
        AudioStreamPlayer3D player = _subPlayers.PickRandom();
        player.PitchScale = (float)GD.RandRange(_minPitch, _maxPitch) + _pitchBaseDelta;
        player.Play();
    }

    public override void Stop()
    {
        foreach (var player in _subPlayers)
            player.Stop();
    }
}