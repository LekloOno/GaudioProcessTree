using Godot;

public partial class AUD_NoTimeScale : Node, AUD_ILocalTime
{
    public ulong LocalScaledTicksMsec => Time.GetTicksMsec();
    public ulong LocalScaledTicksUsec => Time.GetTicksUsec();

    public override void _EnterTree()
    {
        AUD_Time.Instance = this;
    }
}