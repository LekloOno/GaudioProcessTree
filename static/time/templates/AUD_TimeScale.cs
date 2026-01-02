using Godot;

public partial class AUD_ExampleTime : Node, AUD_ILocalTime
{
    public ulong LocalScaledTicksMsec {get; private set;}
    public ulong LocalScaledTicksUsec {get; private set;}

    public override void _EnterTree() =>
        AUD_Time.Instance = this;

    public override void _Ready() =>
        ProcessMode = ProcessModeEnum.Pausable;

    public override void _PhysicsProcess(double delta)
    {
        double deltaMsec = delta * 1000;
        LocalScaledTicksMsec += (ulong) deltaMsec;
        LocalScaledTicksUsec += (ulong) deltaMsec * 1000;;
    }
}