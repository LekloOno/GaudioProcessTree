using System.Collections.Generic;
using Godot;

[GlobalClass, Tool]
public partial class AUD_ParallelSound : AUD_RandomSound
{
    record Voice(long Id, float RandomPitch);
    [Export] private AudioStreamPolyphonic _polyphonicStream; 
    [Export] private float _maxPolyphony = 5f;
    private AudioStreamPlaybackPolyphonic _playback;
    private readonly Queue<Voice> _voices = new();

    private float AbsolutePitch(float randomPitch, float parallelPitch) =>
        randomPitch * parallelPitch * _player.PitchScale;

    private void SetParallelPitchScale(float pitchScale)
    {
        foreach (Voice voice in _voices)
        {
            float voicePitch = AbsolutePitch(voice.RandomPitch, pitchScale);
            _playback.SetStreamPitchScale(voice.Id, voicePitch);
        }
    }

    protected override void SetBasePitchScale(float pitchScale) =>
        SetParallelPitchScale(pitchScale * RelativePitchScale);
    protected override void SetRelativePitchScale(float pitchScale) =>
        SetParallelPitchScale(BasePitchScale * pitchScale);
    
    protected float _pitchBaseDelta;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            return;
            
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

        float pitchScale = AbsolutePitch(randomPitch, PitchScale);
        long newVoice = _playback.PlayStream(stream, 0, 0, pitchScale);
        _voices.Enqueue(new(newVoice, randomPitch));
    }

    public override void Stop() =>
        _player.Stop();
}