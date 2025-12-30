using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_LayeredSound : AUD_Sound
{
    [Export] private Array<AUD_Sound> _layers;
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