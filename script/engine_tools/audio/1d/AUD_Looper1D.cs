using Godot;

[GlobalClass]
public partial class AUD_Looper1D : AUD_BaseLooper
{
    [Export] private AudioStreamPlayer _player;

    protected override float _volumeDb
    {
        get => _player.VolumeDb;
        set => _player.VolumeDb = value;
    }
}