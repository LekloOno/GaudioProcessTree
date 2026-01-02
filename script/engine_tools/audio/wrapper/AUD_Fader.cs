using System;
using System.Collections.Generic;
using Godot;

[GlobalClass, Tool]
public partial class AUD_Fader : AUD_Wrapper
{
    private AUD_Sound _sound;
    /// <summary>
    /// Time for the volume to reach _volume on start - in seconds.
    /// </summary>
    [Export(PropertyHint.Range, "0,5,exp,or_greater")]
    private float _fadeInTime;
    /// <summary>
    /// Time for the volume to reach 0 on stop - in seconds.
    /// </summary>
    [Export(PropertyHint.Range, "0,5,exp,or_greater")]
    private float _fadeOutTime;
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

    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    protected override void ModuleEnterTree()
    {
        InitSoundChild();

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
    
    private void InitSoundChild()
    {
        _sound = null;
        foreach (Node node in GetChildren())
            if (node is AUD_Sound sound)
            {
                _sound = sound;
                return;
            }
    }

    protected override void OnSoundChildChanged(List<AUD_Sound> sounds)
    {
        if (sounds.Count == 0)
            _sound = null;
        else
            _sound = sounds[0];
    }
    // +-------------------+
    // |  CONFIG WARNINGS  |
    // +-------------------+
    // _____________________
    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];

        if (_sound == null)
            warnings.Add("This node has no attached Sound to fade.\nConsider adding an AUD_Sound as a child.");
        if (TooManySounds())
            warnings.Add("This node has multiple Sound children. It will only fade one of them.\nConsider using AUD_LayeredSound as a child to fade multiple sounds.");

        return [.. warnings];
    }

    private bool TooManySounds()
    {
        bool found = false;
        foreach (Node node in GetChildren())
            if (node is AUD_Sound)
                if (found)
                    return true;
                else
                    found = true;

        return false;
    }

    // +-------------------+
    // |  MODULE BEHAVIOR  |
    // +-------------------+
    // _____________________
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
        _startVolume = VolumeDb;
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