using Godot;

[GlobalClass]
public abstract partial class AUD_Wrapper : AUD_Sound
{
    
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
        SetBaseVolumeDb(BaseVolumeDb);
        SetBasePitchScale(BasePitchScale);
    }
}