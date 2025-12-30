using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_LayeredSound : AUD_Sound
{
    record OriginalSettings(float VolumeDb, float PitchScale);

    [Export] private Array<AUD_Sound> _layers;
    private System.Collections.Generic.Dictionary<AUD_Sound, OriginalSettings> _originalDbs;
    private float _dbModulation = 0f;
    private float _pitchModulation = 1f;

    /// <summary>
    /// VolumeDb on LayeredSound is expressed in a linear relative volume. <br/>
    /// This modifier is applied additively on all the layers.
    /// </summary>
    public override float VolumeDb
    {
        get => _dbModulation;
        set 
        {
            _dbModulation = value;
            foreach ((AUD_Sound layer, OriginalSettings original) in _originalDbs)
                layer.VolumeDb = original.VolumeDb + _dbModulation;
        }
    }

    /// <summary>
    /// PitchScale on LayeredSound is expressed in relative frequency. <br/>
    /// This parameter is thus multiplicative, its default value is 1. <br/>
    /// The resulting pitch scales of each layers is PitchScale * original.
    /// </summary>
    public override float PitchScale
    {
        get => _pitchModulation;
        set
        {
            _pitchModulation = value;
            foreach ((AUD_Sound layer, OriginalSettings original) in _originalDbs)
                layer.PitchScale = original.PitchScale * _pitchModulation;
        }
    }

    public override void _Ready()
    {
        foreach (AUD_Sound layer in _layers)
            _originalDbs.Add(layer, new(layer.VolumeDb, layer.PitchScale));
    }

    public override void Play()
    {
        foreach (AUD_Sound layer in _layers)
            layer.Play();
    }

    public override void Stop()
    {
        foreach (AUD_Sound layer in _layers)
            layer.Stop();
    }
}