using Godot;

[GlobalClass]
public abstract partial class AUD_StreamPlayer : AUD_Sound
{
    public abstract AudioStream Stream {get; set;}
    public abstract float VolumeDb {get; set;}
    public abstract float PitchScale {get; set;}
    public abstract AudioStreamPlayback GetStreamPlayBack();
}