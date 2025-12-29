using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_ParallelSound : AUD_Sound
{
    [Export] private AudioStreamPolyphonic _polyphonicStream; 
    [Export] private AUD_StreamPlayer _player;
    [Export] private Array<AudioStream> _sounds = [];
    [Export] private float _minPitch = 1f;
    [Export] private float _maxPitch = 1f;
    [Export] private float _maxPolyphony = 5f;
    private AudioStreamPlaybackPolyphonic _playback;
    private readonly Queue<long> _voices = new();
    
    protected float _pitchBaseDelta;
    public override void _Ready()
    {
        _player.Stream = _polyphonicStream;
        _player.Play();
        _playback = _player.GetStreamPlayBack() as AudioStreamPlaybackPolyphonic;
    }

    public override void Play()
    {
        AudioStream stream = _sounds.PickRandom();
        float pitchScale = (float)GD.RandRange(_minPitch, _maxPitch) + _pitchBaseDelta;

        if (_voices.Count >= _maxPolyphony)
        {
            long oldestVoice = _voices.Dequeue();
            _playback.StopStream(oldestVoice);
            _playback.SetStreamVolume(oldestVoice, -80f);
        }

        long newVoice = _playback.PlayStream(stream, 0, 0, pitchScale);
        _voices.Enqueue(newVoice);
    }

    public override void Stop() =>
        _player.Stop();
}