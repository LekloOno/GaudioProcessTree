using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_RandomSound : AUD_Wrapper
{
    [Export] protected AUD_StreamPlayer _player;
    [Export] protected Array<AudioStream> _sounds = [];
    [Export] protected float _minPitch = 1f;
    [Export] protected float _maxPitch = 1f;

    protected override void SetBaseVolumeDb(float volume) =>
        _player.RelativeVolumeDb = VolumeDb;
    protected override void SetRelativeVolumeDb(float volume) =>
        SetBaseVolumeDb(volume);

    protected override void SetBasePitchScale(float pitchScale) =>
        _player.RelativePitchScale = PitchScale * _randomPitch;
    protected override void SetRelativePitchScale(float pitchScale) =>
        SetBasePitchScale(pitchScale);

    private float _randomPitch = 1f;

    public override void Play()
    {
        _player.Stream = _sounds.PickRandom();
        _randomPitch = (float)GD.RandRange(_minPitch, _maxPitch);
        _player.RelativePitchScale = _randomPitch * PitchScale;
        _player.Play();
    }

    public override void Stop() => _player.Stop();
}