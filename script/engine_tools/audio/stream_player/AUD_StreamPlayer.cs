using Godot;

[GlobalClass]
public abstract partial class AUD_StreamPlayer : AUD_Sound
{
    public abstract AudioStream Stream {get; set;}
    public abstract StringName Bus {get; set;}
    public abstract AudioStreamPlayback GetStreamPlayBack();
}