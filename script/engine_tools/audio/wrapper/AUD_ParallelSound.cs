using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_ParallelSound : AUD_RandomSound
{
    record Voice(long Id, float RandomPitch);
    [Export] private AudioStreamPolyphonic _polyphonicStream; 
    [Export] private float _maxPolyphony = 5f;
    private AudioStreamPlaybackPolyphonic _playback;
    private readonly Queue<Voice> _voices = new();

    private float AbsolutePitch(float randomPitch) =>
        randomPitch * PitchScale * _player.PitchScale;

    protected override void SetBasePitchScale(float pitchScale)
    {
        foreach (Voice voice in _voices)
            _playback.SetStreamPitchScale(voice.Id, AbsolutePitch(voice.RandomPitch));
    }
    protected override void SetRelativePitchScale(float pitchScale) =>
        SetBasePitchScale(pitchScale);
    
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
        float randomPitch = (float)GD.RandRange(_minPitch, _maxPitch);

        if (_voices.Count >= _maxPolyphony)
        {
            long oldestVoice = _voices.Dequeue().Id;
            _playback.SetStreamVolume(oldestVoice, -80f);
            _playback.StopStream(oldestVoice);
        }

        long newVoice = _playback.PlayStream(stream, 0, 0, AbsolutePitch(randomPitch));
        _voices.Enqueue(new(newVoice, randomPitch));
    }

    public override void Stop() =>
        _player.Stop();
}