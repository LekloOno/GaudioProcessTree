@ingroup 2_guides
@page 2_time_dependency AUD_Time Dependency

@section simplest Simplest – Delete or modify AUD_Fader

@subsection do-not-use-fader Don't use or delete AUD_FADER

Of course, you can simply not use @ref AUD_Fader or delete it. The rest of the plugin is fully standalone.

You could eventually also implement your own fader by extending @ref AUD_Module.

@subsection modify-fader Modify AUD_Fader source

Another simple solution is to modify @ref AUD_Fader. You can replace every occurence of `AUD_Time.ScaledTicksMsec` by `Time.GetTicksMsec()`, and it will fully work, but will not take time-scale and pauses into account.

@section enable Enable AUD_Fader via AUD_Time

To keep the plugin flexible, time handling is abstracted through
@ref AUD_Time and @ref AUD_ILocalTime.

@subsection no_scale Very simple – No time scale

If you don't mind about time scale at all, you can simply implement a static AUD_ILocalTime as provided in the @ref AUD_ILocalTime templates.

```cs
public partial class AUD_NoTimeScale : Node, AUD_ILocalTime
{
    public ulong LocalScaledTicksMsec => Time.GetTicksMsec();
    public ulong LocalScaledTicksUsec => Time.GetTicksUsec();

    public override void _EnterTree()
    {
        AUD_Time.Instance = this;
    }
}
```

Then, you can set this script as an autoload in `Project > Project Settings > Globals > Path (folder icon)`, and select the @ref AUD_NoTimeScale script.

@subsection scale Simple – Use provided scale implementation

The second provided template is a simple implementation that accumulates scaled time in `_PhysicsProcess`.

```cs
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
```

Again, you can set this script as an autoload in `Project > Project Settings > Globals > Path (folder icon)`, and select the @ref AUD_ExampleTime script.

@subsection advanced Advanced – Custom implementation

You can implement your own @ref AUD_ILocalTime, for example by wrapping
an existing time system of your own. This allows complete decoupling between audio and global timing logic.

The general idea is simple - implement AUD_ILocalTime, and make sure an instance of this implementation is set as the static `AUD_Time` instance reference before running, so typically using an auto-load with `_EnterTree` like in the examples provided before.

@subsubsection advanced-example Advanced Example for completely modular approach

In the project this plugin was initially built for, `PHX_Time` static class already existed to handle scaled time accumulation. It would be odd to have the GaudioProcessTree depend on it, and it would be even more odd to have `PHX_Time` depend on GaudioProcessTree.

So instead, the setup is fairly simple. We glue `PHX_Time` and @ref AUD_Time through an additional auto-load node. It waits for `PHX_Time` to be setup, and binds a wrapper of it as the singleton's instance of @ref AUD_Time.

Below is the `PHX_Time` source to get a better grasp of the idea.
```cs
public partial class PHX_Time : Node
{
    public static PHX_Time Instance {get; private set;}
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Miliseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    public static ulong ScaledTicksMsec {get => Instance._scaledTicksMsec;}
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Microseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    public static ulong ScaledTicksUsec {get => Instance._scaledTicksUsec;}

    public ulong LocalScaledTicksMsec => _scaledTicksMsec;

    public ulong LocalScaledTicksUsec => _scaledTicksUsec;

    private ulong _scaledTicksMsec = 0;
    private ulong _scaledTicksUsec = 0;
    public override void _EnterTree()
    {
        Instance = this;
        StaticServiceLifeCycle<PHX_Time>.MarkInitialized();
    }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Pausable;
    }

    public override void _PhysicsProcess(double delta)
    {
        double deltaMsec = delta * 1000;
        _scaledTicksMsec += (ulong) deltaMsec;
        _scaledTicksUsec += (ulong) deltaMsec * 1000;;
    }
}
```

Note the `StaticServiceLifeCycle<PHX_Time>.MarkInitialized();`. We will come back to it.

Then, below is the `Bootstrap` glue which notably binds `PHX_Time` to `AUD_Time` in the following manner.
```cs
public partial class Bootstrap : Node
{
    class PHX_LocalTimeWrapper : AUD_ILocalTime
    {
        public ulong LocalScaledTicksMsec => PHX_Time.ScaledTicksMsec;
        public ulong LocalScaledTicksUsec => PHX_Time.ScaledTicksUsec;
    }

    public async override void _EnterTree()
    {
        await StaticServiceLifeCycle<PHX_Time>.Initialized;
        AUD_Time.Instance = new PHX_LocalTimeWrapper();
    }
}
```
Here comes the point of `StaticServiceLifeCycle<PHX_Time>.MarkInitialized();`.  
It is not necessary, godot should load auto-load nodes in the order you specified, so you could simply place the bootstrap later than `PHX_Time` in the list.

Not only that - since the glue class `PHX_LocalTimeWrapper` only requires `PHX_Time` to be correctly initialized on properties access, and not construction, having `Bootstrap` execute first would most likely not cause any issue, as the properties are only accessed at runtime - that is after all auto-loads should be correctly initialized.

Yet, it just offers stricter guarantees.

For further details, here is the so-called `StaticServiceLifeCycle`.

```cs
public static class StaticServiceLifeCycle<T>
{
    private static readonly TaskCompletionSource _tcs = new();

    public static Task Initialized => _tcs.Task;

    public static void MarkInitialized()
        => _tcs.TrySetResult();
}
```