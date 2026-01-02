using Godot;

[GlobalClass, Tool]
public abstract partial class AUD_Sound : Node, AUD_ISound
{
    protected const float MIN_PITCH = 0.001f;
    private float _baseVolumeDb = 0f;
    private float _basePitchScale = 1f;
    protected float _relativeVolumeDb = 0f;
    protected float _relativePitchScale = 1f;

    public abstract void Play();
    public abstract void Stop();
    public abstract float VolumeDb {get; protected set;}
    public abstract float PitchScale {get; protected set;}

    public sealed override void _EnterTree()
    {
        SetBaseVolumeDb(_baseVolumeDb);
        SetBasePitchScale(_basePitchScale);
        EnterTreeSpec();
    }

    /// <summary>
    /// Defines specialized behavior once the AUD_Sound _EnterTree routine has been executed. <br/>
    /// <br/>
    /// This runs AFTER the main _EnterTree routine of AUD_Sound. SetBaseVolumeDb and SetBasePitchScale have already been called once.<br/>
    /// Thus, it is very likely that base, relative and absolute volumeDb/PitchScale have already been initialized depending on their implementation.
    /// </summary>
    protected virtual void EnterTreeSpec(){}

    [Export(PropertyHint.Range, "-80,20,0.1,or_greater,or_less")]
    public float BaseVolumeDb
    {
        get => _baseVolumeDb;
        protected set
        {
            SetBaseVolumeDb(value);
            _baseVolumeDb = value;
        }
    }

    // Maybe add a float "nextVolumeDb" in the setters specifiers to make it explicit, instead of having to manually compute it
    // (like pitchScale * RelativePitchScale when setting the BasePitchScale for example)

    /// <summary>
    /// Specify some additionnal custom behavior before the internal BaseVolumeDb is set to volumeDb.
    /// </summary>
    /// <param name="volumeDb">The value BaseVolumeDb will be set at after the operation.</param>
    protected abstract void SetBaseVolumeDb(float volumeDb);

    
    [Export(PropertyHint.Range, "0.1,5,exp,or_greater,or_less")]
    public float BasePitchScale
    {
        get => _basePitchScale;
        protected set
        {
            SetBasePitchScale(value);
            _basePitchScale = Mathf.Max(value, MIN_PITCH);
        }
    }
    /// <summary>
    /// Specify some additionnal custom behavior before the internal BasePitchScale is set to pitchScale.
    /// </summary>
    /// <param name="pitchScale">The value BasePitchScale will be set at after the operation.</param>
    protected abstract void SetBasePitchScale(float pitchScale);

    public float RelativeVolumeDb
    {
        get => _relativeVolumeDb;
        set
        {
            SetRelativeVolumeDb(value);
            _relativeVolumeDb = value;
        }
    }
    /// <summary>
    /// Specify some additionnal custom behavior before the internal RelativeVolumeDb is set to pitchScale.
    /// </summary>
    /// <param name="volumeDb">The value RelativeVolumeDb will be set at after the operation.</param>
    protected abstract void SetRelativeVolumeDb(float volumeDb);

    public float RelativePitchScale
    {
        get => _relativePitchScale;
        set
        {
            SetRelativePitchScale(value);
            _relativePitchScale = value;
        }
    }
    /// <summary>
    /// Specify some additionnal custom behavior before the internal RelativePitchScale is set to pitchScale.
    /// </summary>
    /// <param name="pitchScale">The value RelativePitchScale will be set at after the operation.</param>
    protected abstract void SetRelativePitchScale(float pitchScale);
}