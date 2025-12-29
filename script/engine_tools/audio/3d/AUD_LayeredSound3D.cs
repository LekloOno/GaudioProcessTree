using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AUD_LayeredSound3D : AUD_Sound
{
    [Export] private Array<AUD_Sound3D> _slideInLayers;
    public override void Play()
    {
        foreach (AUD_Sound3D layer in _slideInLayers)
            layer.Play();
    }

    public override void Stop()
    {
        foreach (AUD_Sound3D layer in _slideInLayers)
            layer.Stop();
    }
}