using Godot;

[GlobalClass]
public abstract partial class AUD_StreamPlayer : AUD_Sound
{
    public abstract AudioStream Stream {get; set;}
    public abstract StringName Bus {get; set;}
    public abstract AudioStreamPlayback GetStreamPlayBack();
    private float _relativeVolumeDb = 0f;
    public override float RelativeVolumeDb
    {
        get => _relativeVolumeDb;
        set
        {
            _relativeVolumeDb = value;
            VolumeDb = BaseVolumeDb + value;
        }
    }

    private float _relativePitchScale = 1f;
    public override float RelativePitchScale
    {
        get => _relativePitchScale;
        set
        {
            _relativePitchScale = value;
            PitchScale = BasePitchScale * value;
        }
    }

    private float _baseVolumeDb;
    public override float BaseVolumeDb
    {
        get => _baseVolumeDb;
        protected set
        {
            _baseVolumeDb = value;
            VolumeDb = value + RelativeVolumeDb;
        }
    }

    private float _basePitchScale;
    public override float BasePitchScale
    {
        get => _basePitchScale;
        protected set
        {
            _basePitchScale = value;
            PitchScale = value * RelativePitchScale;
        }
    }

    public override void _EnterTree()
    {
        BaseVolumeDb = VolumeDb;
        BasePitchScale = PitchScale;
    }
}