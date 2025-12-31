using Godot;

public abstract partial class AUD_Wrapper : AUD_Sound
{
    /// <summary>
    /// Not editable in run time
    /// </summary>
    [Export] protected float _baseVolumeDb = 0f;
    
    /// <summary>
    /// Not editable in run time
    /// </summary>
    [Export] protected float _basePitchScale = 1f;
    
    public override float VolumeDb
    {
        get => BaseVolumeDb + RelativeVolumeDb;
        protected set => BaseVolumeDb = value - RelativeVolumeDb; 
    }

    public override float PitchScale
    {
        get => BasePitchScale * RelativePitchScale;
        protected set => BasePitchScale = value / RelativePitchScale; 
    }

    public override void _Ready()
    {
        SetBaseVolumeDb(_baseVolumeDb);
        SetBasePitchScale(_basePitchScale);
    }


    public override float BaseVolumeDb
    {
        get => _baseVolumeDb;
        protected set
        {
            _baseVolumeDb = value;
            SetBaseVolumeDb(value);
        }
    }
    protected abstract void SetBaseVolumeDb(float volume);

    protected float _relativeVolumeDb = 0f;
    public override float RelativeVolumeDb
    {
        get => _relativeVolumeDb;
        set 
        {
            _relativeVolumeDb = value;
            SetRelativeVolumeDb(value);
        }
    }
    protected abstract void SetRelativeVolumeDb(float volume);

    public override float BasePitchScale
    {
        get => _basePitchScale;
        protected set
        {
            _basePitchScale = value;
            SetBasePitchScale(value);
        }
    }
    protected abstract void SetBasePitchScale(float pitchScale);
    
    protected float _relativePitchScale = 1f;
    public override float RelativePitchScale
    {
        get => _relativePitchScale;
        set 
        {
            _relativePitchScale = value;
            SetRelativePitchScale(value);
        }
    }
    protected abstract void SetRelativePitchScale(float pitchScale);
}