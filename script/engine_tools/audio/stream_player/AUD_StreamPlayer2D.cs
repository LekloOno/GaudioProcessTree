using System.Collections.Generic;
using Godot;

[GlobalClass, Tool]
public partial class AUD_StreamPlayer2D : AUD_StreamPlayer
{
    [Export] private AudioStreamPlayer2D _player;
    public override AudioStream Stream
    {
        get => _player == null ? null : _player.Stream;
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
        get => _player == null ? null : _player.Bus;
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
            warnings.Add("This node has no attached AudioStreamPlayer2D.\nConsider assigning one in the inspector.");

        return [.. warnings];
    }

    public override void Play() => _player.Play();
    public override void Stop() => _player.Stop();
}