using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// AUD_StreamPlayer implementation for Godot's AudioStreamPlayer.
/// </summary>
[GlobalClass, Tool]
public partial class AUD_StreamPlayer1D : AUD_StreamPlayer
{
    private AudioStreamPlayer _player;
    [Export] public AudioStreamPlayer Player
    {
        get => _player;
        private set
        {
            if (_player != null)
                _player.Finished -= ForwardFinished;

            _player = value;
            _player.Finished += ForwardFinished;
            UpdateConfigurationWarnings();
        }
    }
    
    public override AudioStream Stream
    {
        get => _player?.Stream;
        set 
        {
            if (_player == null) return;
            _player.Stream = value;
        }
    }

    public override float VolumeDb
    {
        get => _player == null ? 0f : _player.VolumeDb;
        protected set
        {
            if (_player == null) return;
            _player.VolumeDb = value;  
        }
    }
    
    public override float PitchScale
    {
        get => _player == null ? 1f : _player.PitchScale;
        protected set
        {
            if (_player == null) return;
            _player.PitchScale = value;
        }
    }

    public override StringName Bus
    {
        get => _player?.Bus;
        set
        {
            if (_player == null) return;
            _player.Bus = value;
        }
    }

    public override AudioStreamPlayback GetStreamPlayBack() =>
        _player.GetStreamPlayback();

    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];

        if (_player == null)
            warnings.Add("This node has no attached AudioStreamPlayer.\nConsider assigning one in the inspector.");

        return [.. warnings];
    }

    public override void Play() => _player.Play();
    protected override void StopPlayer() => _player.Stop();
}