using Godot;

[GlobalClass, Tool]
public partial class AUD_StreamPlayer3D : AUD_StreamPlayer
{
    [Export] private AudioStreamPlayer3D _player;
    public override AudioStream Stream
    {
        get => _player == null ? null : Stream;
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

    public override void Play() => _player.Play();
    public override void Stop() => _player.Stop();
}