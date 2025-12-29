using Godot;

[GlobalClass]
public abstract partial class AUD_Sound : Node, AUD_ISound
{
    public abstract void Play();
    public abstract void Stop();
}