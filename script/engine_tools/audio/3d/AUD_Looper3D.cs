using Godot;

[GlobalClass]
public partial class AUD_Looper3D : AUD_BaseLooper
{
    [Export] private AudioStreamPlayer3D _player;

    protected override float _volumeDb
    {
        get => _player.VolumeDb;
        set => _player.VolumeDb = value;
    }
}