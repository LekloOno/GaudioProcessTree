using System;
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
public partial class AUD_Parallelizer : AUD_Randomizer
{
    readonly struct Voice()
    {
        public long Id { get; }
        public float RandomPitch { get; }
        public double Length { get; }

        public Voice(long id, float randomPitch, double length) : this()
        {
            Id = id;
            RandomPitch = randomPitch;
            Length = length;
        }
    }

    record VoiceTracker(Voice Voice)
    {
        public double Lifetime {get; set;} = Voice.Length;
        public long Id => Voice.Id;
        public float RandomPitch => Voice.RandomPitch;
        public double Length => Voice.Length;

        public VoiceTracker(long id, float randomPitch, double length)
            : this(new Voice(id, randomPitch, length)) {}
    }

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
    private readonly LinkedList<VoiceTracker> _voices = new();

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
        randomPitch * parallelPitch * Player.PitchScale;

    private void SetParallelPitchScale(float pitchScale)
    {
        foreach (VoiceTracker tracker in _voices)
        {
            float voicePitch = AbsolutePitch(tracker.Voice.RandomPitch, pitchScale);
            _playback.SetStreamPitchScale(tracker.Voice.Id, voicePitch);
        }
    }

    protected override void SetBasePitchScale(float pitchScale) =>
        SetParallelPitchScale(pitchScale * RelativePitchScale);
    protected override void SetRelativePitchScale(float pitchScale) =>
        SetParallelPitchScale(BasePitchScale * pitchScale);
    
    protected float _pitchBaseDelta;

    public override void _Ready()
    {
        SetPhysicsProcess(!Engine.IsEditorHint());

        if (Engine.IsEditorHint())
            return;
            
        Player.Stream = _polyphonicStream;
        Player.Play();
        _playback = Player.GetStreamPlayBack() as AudioStreamPlaybackPolyphonic;
    }

    public override void _PhysicsProcess(double delta)
    {
        LinkedListNode<VoiceTracker> tracker = _voices.First;
        while (tracker != null)
        {
            LinkedListNode<VoiceTracker> next = tracker.Next;
            if (Cycle(tracker.Value, delta))
                RemoveTracker(tracker);
            tracker = next;
        }
    }

    private bool Cycle(VoiceTracker tracker, double delta)
    {
        tracker.Lifetime -= delta * AbsolutePitch(tracker.RandomPitch, PitchScale);
        return tracker.Lifetime <= 0;
    }

    public void EnqueueVoice(AudioStream stream, float randomPitch)
    {
        float pitchScale = AbsolutePitch(randomPitch, PitchScale);

        long newVoice = _playback.PlayStream(stream, 0, 0, pitchScale);
        double length = stream.GetLength();

        _voices.AddLast(new VoiceTracker(newVoice, randomPitch, length));
    }

    private void RemoveVoice(long id)
    {
        _playback.SetStreamVolume(id, -80f);
        _playback.StopStream(id);
        if (_voices.Count == 0)
            ForwardFinished();
    }

    private void RemoveTracker(LinkedListNode<VoiceTracker> tracker)
    {
        _voices.Remove(tracker);
        RemoveVoice(tracker.Value.Id);
    }

    private void DequeueVoice()
    {
        long oldestVoice = _voices.First.Value.Id;
        _voices.RemoveFirst();
        RemoveVoice(oldestVoice);
    }

    public override void Play()
    {
        AudioStream stream = _sounds.PickRandom();
        float randomPitch = (float)GD.RandRange(MinPitch, MaxPitch);

        if (_voices.Count >= _maxPolyphony)
            DequeueVoice();
        
        EnqueueVoice(stream, randomPitch);
    }
}