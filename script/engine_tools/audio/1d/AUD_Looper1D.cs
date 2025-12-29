using Godot;

[GlobalClass]
public partial class AUD_Looper1D : AUD_Looper
{
    [Export] private AudioStreamPlayer _player;

    protected override float _volumeDb
    {
        get => _player.VolumeDb;
        set => _player.VolumeDb = value;
    }
}