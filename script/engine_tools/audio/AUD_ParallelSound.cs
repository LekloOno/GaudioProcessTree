using Godot;
using Godot.Collections;

public partial class AUD_ParallelSound : AUD_Sound
{
    [Export] private AudioStreamPolyphonic _polyphonicStream; 
    [Export] private AUD_StreamPlayer _player;
    [Export] private Array<AudioStream> _sounds = new();
    [Export] private float _minPitch = 1f;
    [Export] private float _maxPitch = 1f;
    private AudioStreamPlaybackPolyphonic _playback;
    
    protected float _pitchBaseDelta;
    public override void _Ready()
    {
        _player.Stream = _polyphonicStream;
        _playback = _player.GetStreamPlayBack() as AudioStreamPlaybackPolyphonic;
    }

    public override void Play()
    {
        AudioStream stream = _sounds.PickRandom();
        float pitchScale = (float)GD.RandRange(_minPitch, _maxPitch) + _pitchBaseDelta;
        _playback.PlayStream(stream, 0, 0, pitchScale);
    }

    public override void Stop() =>
        _player.Stop();
}