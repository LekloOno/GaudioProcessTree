using System;
using Godot;

[GlobalClass, Tool]
public partial class AUD_Fader : AUD_Wrapper
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
    /// Wether the sound this fader fades should start at muted or unmuted state.
    /// </summary>
    [Export] private bool _startMuted = true;
    /// <summary>
    /// Stores the time at which the fade started.
    /// </summary>
    private float _fadeStart;
    /// <summary>
    /// Stores the db volume at which a new fade operation has started.
    /// </summary>
    private float _startVolume;

    private Action _onUpdate;   // Allows to bind dynamically the fade in/out delegates on update and avoid a branching

    private float _mutedVolumeDb;
    private float _currentFadeTime;
    private float _currentTargetVolume;
    private bool _muting = true;

    private void SetFaderVolumeDb(float volumeDb)
    {
        if (_muting)
            return;

        if (IsPhysicsProcessing())
            _currentTargetVolume = volumeDb;
        else if (_sound != null)
            _sound.RelativeVolumeDb = volumeDb;
    }

    protected override void SetBaseVolumeDb(float volumeDb) =>
        SetFaderVolumeDb(volumeDb + RelativeVolumeDb);

    protected override void SetRelativeVolumeDb(float volumeDb) =>
        SetFaderVolumeDb(BaseVolumeDb + volumeDb);

    protected override void SetBasePitchScale(float pitchScale)
    {
        if (_sound == null) return;
        _sound.RelativePitchScale = pitchScale * RelativePitchScale;
    }
    protected override void SetRelativePitchScale(float pitchScale)
    {
        if (_sound == null) return;
        _sound.RelativePitchScale = BasePitchScale * pitchScale;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        // Kinda dirty, but I'd rather still be able to use AutoPlay, so what the fader manipulates is as abstract as possible.
        // Otherwise, we could simply disallow auto play, and call play on ready, but it already infers meaning to the wrapped AUD_Sound.
        if (Engine.IsEditorHint())
        {
            SetPhysicsProcess(false);
            return;
        }

        if (_sound == null)
            return;
        
        _mutedVolumeDb = -80f - _sound.VolumeDb;

        if (_startMuted)
            _sound.RelativeVolumeDb = _mutedVolumeDb;
        else
            _sound.RelativeVolumeDb = VolumeDb;
        SetPhysicsProcess(!_startMuted);
    }
    /// <summary>
    /// Starts Fading in.
    /// </summary>
    public override void Play()
    {
        InitFade();
        _muting = false;
        _currentFadeTime = _fadeInTime;
        _currentTargetVolume = VolumeDb;
        _onUpdate += Fade;
    }

    /// <summary>
    /// Starts Fading out.
    /// </summary>
    public override void Stop()
    {
        InitFade();
        _muting = true;
        _currentFadeTime = _fadeOutTime;
        _currentTargetVolume = _mutedVolumeDb;
        _onUpdate += Fade;
    }

    private void InitFade()
    {
        _fadeStart = PHX_Time.ScaledTicksMsec;
        _startVolume = _sound.VolumeDb;
        SetPhysicsProcess(true);
    }


    public override void _PhysicsProcess(double delta) =>
        _onUpdate?.Invoke();

    private void Fade()
    {
        float elapsed = (PHX_Time.ScaledTicksMsec - _fadeStart)/1000f;

        if (elapsed >= _currentFadeTime)
        {
            _sound.RelativeVolumeDb = _currentTargetVolume;
            _onUpdate -= Fade;
            SetPhysicsProcess(false);
            return;
        }

        float elapsedScaled = elapsed/_currentFadeTime;
        float lerped = MATH_Sound.LerpDB(_startVolume, _currentTargetVolume, elapsedScaled);
        
        _sound.RelativeVolumeDb = lerped;
    }
}