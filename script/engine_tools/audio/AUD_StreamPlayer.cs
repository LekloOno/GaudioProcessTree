using Godot;

public abstract partial class AUD_StreamPlayer : Node
{
    public abstract AudioStream Stream {get; set;}
    public abstract float VolumeDb {get; set;}
    public abstract float PitchScale {get; set;}
    public abstract void Play();
    public abstract void Stop();
    public abstract AudioStreamPlayback GetStreamPlayBack();
}