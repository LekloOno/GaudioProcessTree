using System;
using Godot;

[GlobalClass]
public partial class AUD_Fader : AUD_Sound
{
    [Export] private AUD_Sound _sound;
    /// <summary>
    /// Time for the volume to reach _volume on start - in seconds.
    /// </summary>
    [Export] private float _fadeInTime;
    /// <summary>
    /// Time for the volume to reach 0 on stop - in seconds.
    /// </summary>
    [Export] private float _fadeOutTime;
    /// <summary>
    /// Max target volume - in DB.
    /// </summary>
    [Export] public override float VolumeDb {get; set;} = 0f;
    /// <summary>
    /// Stores the time at which the fade started.
    /// </summary>
    private float _fadeStart;
    /// <summary>
    /// Stores the db volume at which a new fade operation has started.
    /// </summary>
    private float _startVolume;

    private Action _onUpdate;   // Allows to bind dynamically the fade in/out delegates on update and avoid a branching
    
    public override float PitchScale
    {
        get => _sound.PitchScale;
        set => _sound.PitchScale = value;    
    }

    public override void _Ready()
    {
        SetPhysicsProcess(false);
        _sound.VolumeDb = -80;
    }

    /// <summary>
    /// Starts Fading in.
    /// </summary>
    public override void Play()
    {
        InitFade();
        _onUpdate += FadeIn;
    }

    /// <summary>
    /// Starts Fading out.
    /// </summary>
    public override void Stop()
    {
        InitFade();
        _onUpdate += FadeOut;
    }

    private void InitFade()
    {
        _fadeStart = PHX_Time.ScaledTicksMsec;
        _startVolume = _sound.VolumeDb;
        SetPhysicsProcess(true);
    }


    public override void _PhysicsProcess(double delta) =>
        _onUpdate?.Invoke();

    private void Fade(Action action, float currentFadeTime, float currentTargetVolume)
    {
        float elapsed = (PHX_Time.ScaledTicksMsec - _fadeStart)/1000f;

        if (elapsed >= currentFadeTime)
        {
            _sound.VolumeDb = currentTargetVolume;
            _onUpdate -= action;
            SetPhysicsProcess(false);
            return;
        }

        float elapsedScaled = elapsed/currentFadeTime;
        float lerped = MATH_Sound.LerpDB(_startVolume, currentTargetVolume, elapsedScaled);
        
        _sound.VolumeDb = lerped;
    }

    private void FadeOut() => Fade(FadeOut, _fadeOutTime, -80);
    private void FadeIn() => Fade(FadeIn, _fadeInTime, VolumeDb);
}