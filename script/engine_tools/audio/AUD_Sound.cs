using System;
using Godot;

[GlobalClass, Tool]
public abstract partial class AUD_Sound : Node, AUD_ISound
{
    /// <summary>
    /// Not editable in run time for now.
    /// </summary>
    private float _baseVolumeDb = 0f;
    
    /// <summary>
    /// Not editable in run time for now.
    /// </summary>
    private float _basePitchScale = 1f;
    protected float _relativeVolumeDb = 0f;
    protected float _relativePitchScale = 1f;

    public abstract void Play();
    public abstract void Stop();
    public abstract float VolumeDb {get; protected set;}
    public abstract float PitchScale {get; protected set;}

    public override void _EnterTree()
    {
        SetBaseVolumeDb(_baseVolumeDb);
        SetBasePitchScale(_basePitchScale);
    }

    [Export] public float BaseVolumeDb
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

    [Export] public float BasePitchScale
    {
        get => _basePitchScale;
        protected set
        {
            SetBasePitchScale(value);
            _basePitchScale = value;
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