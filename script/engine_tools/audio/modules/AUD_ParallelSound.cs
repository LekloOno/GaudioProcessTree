using System.Collections.Generic;
using Godot;

/// <summary>
/// Specialization of AUD_RandomSound that runs each new stream Play on a parallel channel. <br/>
/// <br/>
/// Modification to its relative volumeDb and pitchScale still applies to all channels, but relatively to their own initial volumeDb and pitchScale. <br/>
/// This is the default behavior for volumeDb, but modifying the pitch scale of a player usually don't affect polyphonic stream. <br/>
/// The node provides a way to achieve it while maintaining independant base pitch. <br/>
/// <br/>
/// Setting the Stream of an AudioStreamPlayer stops all its currently playing sounds. AUD_RandomSound would suffer from this in cases where we need to repeatedly play. <br/>
/// AUD_ParallelSound is thus also relevant if avoiding this issue is necessary.
/// </summary>
[GlobalClass, Tool]
public partial class AUD_ParallelSound : AUD_RandomSound
{
    record Voice(long Id, float RandomPitch);
    private AudioStreamPolyphonic _polyphonicStream; 
    [Export] public AudioStreamPolyphonic PolyphonicStream
    {
        get => _polyphonicStream;
        set
        {
            _polyphonicStream = value;
            UpdateConfigurationWarnings();
        }
    }


    private uint _maxPolyphony = 5;
    [Export(PropertyHint.Range, "1,16,1,or_greater")]
    public uint MaxPolyphony
    {
        get => _maxPolyphony;
        set
        {
            _maxPolyphony = value;
            UpdateConfigurationWarnings();
        }
    }

    private AudioStreamPlaybackPolyphonic _playback;
    private readonly Queue<Voice> _voices = new();

    // +-------------------+
    // |  CONFIG WARNINGS  |
    // +-------------------+
    // _____________________
    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [.. base._GetConfigurationWarnings()];
        
        if (_polyphonicStream == null)
            warnings.Add("AudioStreamPolyphonic must be provided for this node to function.\nPlease provide one as Polyphonic Stream property.");
        else if (_polyphonicStream.Polyphony <= _maxPolyphony)
            warnings.Add("Unsufficient number of polyphone streams.\n"
+ "The number of polyphony streams available (" + _polyphonicStream.Polyphony + ") on the provided AudioStreamPolyphonic isn't sufficient to match expected Max Polyphony (" + _maxPolyphony + ").\n"
+ "Consider increasing the value of PolyphonicStream.Polyphony, or decreasing Max Polyphony.");

        return [.. warnings];
    }

    // +-------------------+
    // |  MODULE BEHAVIOR  |
    // +-------------------+
    // _____________________
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
        float randomPitch = (float)GD.RandRange(MinPitch, MaxPitch);

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