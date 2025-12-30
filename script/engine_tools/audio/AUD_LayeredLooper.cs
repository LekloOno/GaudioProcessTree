using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_LayeredLooper : AUD_Looper
{
    [Export] private Array<AUD_BaseLooper> _layers;
    public override void StartLoop()
    {
        foreach (AUD_BaseLooper layer in _layers)
            layer.StartLoop();
    }

    public override void StopLoop()
    {
        foreach (AUD_BaseLooper layer in _layers)
            layer.StopLoop();
    }
}