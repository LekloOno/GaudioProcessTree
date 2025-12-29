using Godot;

[GlobalClass]
public partial class AUD_StreamPlayer3D : AUD_StreamPlayer
{
    [Export] private AudioStreamPlayer3D _player;
    public override AudioStream Stream
    {
        get => _player.Stream;
        set => _player.Stream = value;
    }

    public override float VolumeDb
    {
        get => _player.VolumeDb;
        set => _player.VolumeDb = value;
    }

    public override float PitchScale
    {
        get => _player.PitchScale;
        set => _player.PitchScale = value;
    }

    public override AudioStreamPlayback GetStreamPlayBack() =>
        _player.GetStreamPlayback();

    public override void Play() => _player.Play();
    public override void Stop() => _player.Stop();
}