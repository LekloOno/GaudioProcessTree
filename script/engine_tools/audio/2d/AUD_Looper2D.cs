using Godot;

[GlobalClass]
public partial class AUD_Looper2D : AUD_Looper
{
    [Export] private AudioStreamPlayer2D _player;

    protected override float _volumeDb
    {
        get => _player.VolumeDb;
        set => _player.VolumeDb = value;
    }
}