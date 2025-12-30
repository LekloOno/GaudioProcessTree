using System;
using Godot;

[GlobalClass]
public abstract partial class AUD_Looper : Node, AUD_ILooper
{
    /// <summary>
    /// Starts Fading in.
    /// </summary>
    public abstract void StartLoop();

    /// <summary>
    /// Starts Fading out.
    /// </summary>
    public abstract void StopLoop();
}