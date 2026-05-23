using System;
using System.Diagnostics.CodeAnalysis;
using GaudioProcessTree.Nodes;
using Godot;
using Godot.Collections;

namespace GaudioProcessTree;
/// <summary>
/// The AUD_Sound class defines the base abstraction and implementation of AUD_ISound sound processing tree nodes. <br/>
/// <br/>
/// Such tree are composed of module nodes (non-leaf) that encapsulate further nodes, until eventually reaching stream player leafs. <br/>
/// The module nodes provide specific interfaces for various redundant processing operation like fading, layering, randomization, etc. <br/>
/// The stream nodes bind this tree to concrete Godot AudioStreamPlayers, in a generic manner so that any "spatialness" can be relevant to the same processing tree.
/// </summary>
[GlobalClass, Tool, Icon("res://addons/GaudioProcessTree/icons/gaudio_tree_node.svg")]
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
    public event Action Finished;

    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    public sealed override void _EnterTree()
    {
        SetBaseVolumeDb(_baseVolumeDb);
        SetBasePitchScale(_basePitchScale);
        
        if (UseTimeScale)
            _timeScale = () => Engine.TimeScale;

        EnterTreeSpec();
    }
    /// <summary>
    /// Defines specialized behavior once the AUD_Sound _EnterTree routine has been executed. <br/>
    /// <br/>
    /// This runs AFTER the main _EnterTree routine of AUD_Sound. SetBaseVolumeDb and SetBasePitchScale have already been called once.<br/>
    /// Thus, it is very likely that base, relative and absolute volumeDb/PitchScale have already been initialized depending on their implementation.
    /// </summary>
    protected virtual void EnterTreeSpec(){}

    // +-----------------+
    // |  BASE BEHAVIOR  |
    // +-----------------+
    // ____________________
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
    /// <param name="baseVolumeDb">The value BaseVolumeDb will be set at after the operation.</param>
    protected abstract void SetBaseVolumeDb(float baseVolumeDb);

    
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
    /// <param name="basePitchScale">The value BasePitchScale will be set at after the operation.</param>
    protected abstract void SetBasePitchScale(float basePitchScale);

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
    /// <param name="relativeVolumeDb">The value RelativeVolumeDb will be set at after the operation.</param>
    protected abstract void SetRelativeVolumeDb(float relativeVolumeDb);

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
    /// <param name="relativePitchScale">The value RelativePitchScale will be set at after the operation.</param>
    protected abstract void SetRelativePitchScale(float relativePitchScale);

    // +-----------------+
    // |     HELPERS     |
    // +-----------------+
    // ____________________
    /// <summary>
    /// Used by implementing class to forward Finished event. <br/>
    /// </summary>
    protected void ForwardFinished() =>
        Finished?.Invoke();
    /// <summary>
    /// Retrieves this node's current absolute pitch scale. <br/>
    /// Note: Absolute pitch scale is not equivalent to PitchScale, it is engine time-scale aware.
    /// </summary>
    public float AbsolutePitchScale => PitchScale * (float)_timeScale();
    /// <summary>
    /// Retrieves this node's absolute pitch from an arbitrary relative pitch scale. <br/>
    /// Note: Absolute pitch scale is not equivalent to PitchScale, it is engine time-scale aware.
    /// </summary>
    /// <param name="relativePitchScale">The supposed relative pitch scale.</param>
    /// <returns>The absolute pitch scale this node would have with this relative pitch scale.</returns>
    protected float AbsPitchFromRelative(float relativePitchScale) =>
        BasePitchScale * relativePitchScale * (float)_timeScale();
    /// <summary>
    /// Retrieves this node's absolute pitch from an arbitrary base pitch scale. <br/>
    /// Note: Absolute pitch scale is not equivalent to PitchScale, it is engine time-scale aware.
    /// </summary>
    /// <param name="basePitchScale">The supposed base pitch scale.</param>
    /// <returns>The absolute pitch scale this node would have with this base pitch scale.</returns>
    protected float AbsPitchFromBase(float basePitchScale) =>
        basePitchScale * RelativePitchScale * (float)_timeScale();
    /// <summary>
    /// Retrieves this node's absolute volume Db from an arbitrary relative volume Db. <br/>
    /// Note: Absolute volume Db is equivalent to VolumeDb as it does not fluctuate with engine time scale.
    /// </summary>
    /// <param name="relativeVolumeDb">The supposed relative volume Db.</param>
    /// <returns>The absolute volume Db this node would have with this relative volume Db.</returns>
    protected float AbsVolumeDbFromRelative(float relativeVolumeDb) =>
        BaseVolumeDb + relativeVolumeDb;
    /// <summary>
    /// Retrieves this node's absolute volume Db from an arbitrary base volume Db. <br/>
    /// Note: Absolute volume Db is equivalent to VolumeDb as it does not fluctuate with engine time scale.
    /// </summary>
    /// <param name="baseVolumeDb">The supposed base volume Db.</param>
    /// <returns>The absolute volume Db this node would have with this base volume Db.</returns>
    protected float AbsVolumeDbFromBase(float baseVolumeDb) =>
        baseVolumeDb + RelativeVolumeDb;

    // +-------------------+
    // |   TIME SCALING    |
    // +-------------------+
    // _____________________
    protected Func<double> _timeScale = () => 1.0;

    private bool _useTimeScale = true;
    public bool UseTimeScale
    {
        get => _useTimeScale;
        protected set
        {
            if (_useTimeScale == value)
                return;
                
            _useTimeScale = value;
            UpdateEffectiveTimeScale();
            PropagateTimeScale();
            UpdateConfigurationWarnings();
        }
    }

    public bool EffectiveTimeScale => _useTimeScale || (TryGetParent(out AUD_Module? parent ) && parent.EffectiveTimeScale);
    public void UpdateEffectiveTimeScale()
    {
        if (EffectiveTimeScale)
            _timeScale = () => Engine.TimeScale;
        else
            _timeScale = () => 1.0;
    }

    /// <summary>
    /// Propagate a time scale setting change to children, typically call UpdateEffectiveTimeScale on every children.
    /// </summary>
    protected abstract void PropagateTimeScale();
    
    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];

        if (!TryGetParent(out AUD_Module? parent) || parent.UseTimeScale == false)
        {
            properties.Add(new Dictionary
            {
                { "name", "AUD_Sound Time" },
                { "type", (int)Variant.Type.Nil },
                { "usage", (int)PropertyUsageFlags.Category }
            });

            properties.Add(new Dictionary
            {
                { "name", "UseTimeScale" },
                { "type", (int)Variant.Type.Bool },
                { "usage", (int)PropertyUsageFlags.Default }
            });
        }

        return properties;
    }

    public override bool _PropertyCanRevert(StringName property)
    {
        if (property == "UseTimeScale")
            return _useTimeScale != true;

        return false;
    }

    public override Variant _PropertyGetRevert(StringName property)
    {
        if (property == "UseTimeScale")
            return true;

        return default;
    }

    private bool TryGetParent([NotNullWhen(true)] out AUD_Module? parentModule)
    {
        Node parent = GetParent();
        if (parent is AUD_Module module)
        {
            parentModule = module;
            return true;
        }
        
        parentModule = null;
        return false;
    }

    private bool IsRootModule() =>
        GetParent() is not AUD_Module;

    public sealed override void _Ready()
    {
        // Ensure time scale is propagated from parent
        if (TryGetParent(out AUD_Module? parent) && parent.UseTimeScale == true)
            UseTimeScale = true;

        SetPhysicsProcess(!Engine.IsEditorHint());
        ReadySpec();
    }
    /// <summary>
    /// Allows the implementing class to define further custom _Ready routines. <br/>
    /// This prevents the user from unintendedly hiding important AUD_Module's _Ready base routines.
    /// </summary>
    protected virtual void ReadySpec(){}
}