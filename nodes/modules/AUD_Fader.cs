using System;
using System.Collections.Generic;
using GaudioProcessTree.Static;
using GaudioProcessTree.Static.Time;
using Godot;

namespace GaudioProcessTree.Nodes.Modules;
/// <summary>
/// Fades in and out the VolumeDb of a child AUD_Sound by respectively calling Play() and Stop().
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Finished"/> fires when the fader completes fading out.
/// </para>
/// </remarks>
[GlobalClass, Tool, Icon("res://addons/GaudioProcessTree/icons/gaudio_fader.svg")]
public partial class AUD_Fader : AUD_Module
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
            return;

        if (_sound == null)
            return;
        
        _mutedVolumeDb = -80f - _sound.VolumeDb;

        if (_startMuted)
            _sound.RelativeVolumeDb = _mutedVolumeDb;
        else
            _sound.RelativeVolumeDb = VolumeDb;
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

    protected override void SetBaseVolumeDb(float baseVolumeDb) =>
        SetFaderVolumeDb(AbsVolumeDbFromBase(baseVolumeDb));

    protected override void SetRelativeVolumeDb(float relativeVolumeDb) =>
        SetFaderVolumeDb(AbsVolumeDbFromRelative(relativeVolumeDb));

    protected override void SetBasePitchScale(float basePitchScale)
    {
        if (_sound == null) return;
        _sound.RelativePitchScale = AbsPitchFromBase(basePitchScale);
    }
    protected override void SetRelativePitchScale(float relativePitchScale)
    {
        if (_sound == null) return;
        _sound.RelativePitchScale = AbsPitchFromRelative(relativePitchScale);
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
        _fadeStart = AUD_Time.ScaledTicksMsec;
        _startVolume = VolumeDb;
    }


    protected override void PhysicsProcessSpec(double delta) =>
        _onUpdate?.Invoke();

    private void Fade()
    {
        float elapsed = (AUD_Time.ScaledTicksMsec - _fadeStart)/1000f;

        if (elapsed >= _currentFadeTime)
        {
            _sound.RelativeVolumeDb = _currentTargetVolume;
            _onUpdate -= Fade;

            if (_muting)
                ForwardFinished();

            return;
        }

        float elapsedScaled = elapsed/_currentFadeTime;
        float lerped = AUD_Math.LerpDB(_startVolume, _currentTargetVolume, elapsedScaled);
        
        _sound.RelativeVolumeDb = lerped;
    }

    protected override void PitchTimeScale() =>
        _sound.RelativePitchScale = AbsolutePitchScale;
}