using Godot;

/// <summary>
/// An example binding for AUD_Time with no time scale. <br/>
/// To be correctly used, you should set it as an auto-load in your Project Settings.
/// </summary>
public partial class AUD_NoTimeScale : Node, AUD_ILocalTime
{
    public ulong LocalScaledTicksMsec => Time.GetTicksMsec();
    public ulong LocalScaledTicksUsec => Time.GetTicksUsec();

    public override void _EnterTree()
    {
        AUD_Time.Instance = this;
    }
}