using System;
using Godot;

/// <summary>
/// A Stream Player is a leaf node in an AUD_Sound processing tree. <br/>
/// It binds the tree branch to a concrete Godot AudioStreamPlayer by wrapping it under a generic interface so that any kind of AudioStreamPlayer (simple, 2D, 3D) can be used.<br/>
/// <br/>
/// It is not necessary for the Godot AudioStreamPlayer to be a child of the AUD_StreamPlayer, and child aren't auto-referenced but should be manually assigned in the inspector.
/// It is intentionnal, not to break possible Node3D/Node2D required spatial hierarchy, since AUD_Sound only extends Node. <br/>
/// You can thus place an AudioStreamPlayer2D/3D wherever you want to be correctly spatially-parented.
/// </summary>
[GlobalClass, Tool]
public abstract partial class AUD_StreamPlayer : AUD_Sound
{
    /// <summary>
    /// Determines the Finished and stopping behavior. <br/>
    /// If true, calling Stop() has no effect, and Finished is only fired when the playing stream finishes without interruption. <br/>
    /// If false, calling Stop() does interrupt the Stream, and fires the Finished event.
    /// </summary>
    [Export] private bool _interruptable = true;
    public abstract AudioStream Stream {get; set;}
    public abstract StringName Bus {get; set;}
    public abstract AudioStreamPlayback GetStreamPlayBack();
    protected override void SetBaseVolumeDb(float volumeDb) =>
        VolumeDb = volumeDb + RelativeVolumeDb;

    protected override void SetBasePitchScale(float pitchScale) =>
        PitchScale = pitchScale * RelativePitchScale;

    protected override void SetRelativeVolumeDb(float volumeDb) =>
        VolumeDb = BaseVolumeDb + volumeDb;

    protected override void SetRelativePitchScale(float pitchScale) =>
        PitchScale = BasePitchScale * pitchScale;

    public override void Stop()
    {
        if (!_interruptable)
            return;

        StopPlayer();
        ForwardFinished();
    }

    /// <summary>
    /// Defines the way of concretely stopping the wrapped stream player.
    /// </summary>
    protected abstract void StopPlayer();
}