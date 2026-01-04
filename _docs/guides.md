@page guides Guides

@section concepts General Concepts

@subsection general_idea Idea

The idea is to mimic Node2D / Node3D parenting for audio modules.

You can parent any `AUD_Sound` derivative to another one, creating
tree-like processing chains.

The plugin enables multiple features:
- Relative volume and pitch propagation, in a similar fashion to local and global transform parenting on Node2D/Node3D.
- Dynamic time-relative pitch scaling, to stretch and squeeze streams when time scale is changed (through any root node configuration, or through the specific AUD_TimeScaler)
- Bindings for time-scale compliance.
- Various built-in processing modules, like fader, randomizer, sequencer, etc.
- Wrapping of Godot's StreamPlayer/2D/3D behind a single abstract interface to make GaudioProcessTrees cross compatible with any "spatialness". 

@subsection aud_sound AUD_Sound common concept

@subsubsection relative_properties Relative levels of VolumeDb and PitchScale
Each @ref AUD_Sound has:

- Base
- Relative
- Local
- Absolute

designation for both its **volume (db)** and **pitch scale** properties.

**Base** volume/pitch is a value you can set in the inspector, think of it as the "local transform" of Node2D that you tweak in your scene, but sound-wise.

**Relative** is a modifier that allows to modulate and propagate changes on tree children without overriding the base property. Its purpose is runtime only, it is typically used within `AUD_Sound` themselves to propagate changes. Thus it isn't exposed in the editor, but you can also manipulate it in external runtime code.

**Local** (commonly just called *VolumeDb* and *PitchScale* on `AUD_Sound` nodes) is the result of the combination of <u>Base</u> and <u>Relative</u> properties.

**Absolute** is the result of the combination of the <u>Local</u> property, and eventual engine time scaling. For the volume, time scale has no effect, so the absolute volume is equivalent to the local volume. But time scale strecthes sounds, so the absolute pitch scale can differ from the local pitch scale.

@subsubsection note_on_scales Note on volume and pitch scales

**Volume** is additive since it is already on a logarithmic scale (decibels).
So, setting `RelativeVolumeDb` to 0 will result in the "original" volume - `BaseVolumeDb`.  
Similarly, setting a parent node volume (base or relative) to -1, and its children to +1, will effectively cancel out.

**Pitch** on the other hand is multiplicative, since it is relative to the frequency of the sound. Thus, it can never be 0, and setting `RelativePitchScale` to 1 will result in the "original" pitch - `BasePitchScale`.  
As well, setting a parent node pitch (base or relative) to 2, and its children to 0.5 will cancel out.

@subsubsection time_scaling Pitch time scaling

By default, all GaudioProcessTrees use pitch time scaling. This option makes so pitch scale grows proportionnaly to engine speed, to reflect a realistic slow-mo effect in sounds.  
With slower engine speeds, the sounds will get pitched down, and with faster engine speed, it will get pitched up.  

This is fully dynamic, even already playing sound will get properly modulated in real time.

The option is visible for any root node of a GaudioProcessTree, under "AUD_Sound Time" category, as "Use Time Scale" flag.

![alt text](gaudio_use_time_scale.png)

@attention
This option is only visible for root node (of a GaudioProcessTree), as it controls the entire tree scaling behavior.

Additionnaly, you can use the AUD_TimeScaler node to time scale only parts of a GaudioProcessTree, if necessary.

@warning
Nothing prevents you from enabling root time scaling, and AUD_TimeScaler together, or even multiple AUD_TimeScaler. The time scalers will still reapply time scaling, which will exponentially scale pitch, but eh - it's possible !

@subsection aud_module AUD_Module

@ref AUD_Module are non-leaf nodes that can parent other modules or stream players. They provide common processing patterns.

Currently implemented modules:

Parenting AUD_Sound:
- @ref AUD_LayeredSound – layering multiple sounds
- @ref AUD_Fader – fading sounds in and out
- @ref AUD_Sequencer – sequential playback
- @ref AUD_Delayer – delayed playback
- @ref AUD_TimeScaler - scales the pitch of the following tree branch with engine's time scale
Parenting AUD_StreamPlayer:
- @ref AUD_RandomSound – randomizing sound selection and pitch
- @ref AUD_ParallelSound – running randomized streams in parallel

@subsection aud_streamplayer AUD_StreamPlayer

@ref AUD_StreamPlayer are leaf nodes and provide the concrete binding to Godot.

They wrap Godot’s `AudioStreamPlayer` types in a generic way, allowing the same
processing tree to work with 2D, 3D, or non-spatial audio.

@important
Inside GaudioProcessTrees, do not try to change the volume/pitch of a sound through the `AudioStreamPlayer`. It will have no effect, as your changes will be overwritten by the containing `AUD_StreamPlayer`. You should set these properties in the Base volume and pitch of the `AUD_StreamPlayer` instead.

@section time_dependency AUD_Time Dependency

@subsection simplest Simplest – Delete or modify AUD_Fader

@subsubsection do-not-use-fader Don't use or delete AUD_FADER

Of course, you can simply not use @ref AUD_Fader or delete it. The rest of the plugin is fully standalone.

You could eventually also implement your own fader by extending @ref AUD_Module.

@subsubsection modify-fader Modify AUD_Fader source

Another simple solution is to modify @ref AUD_Fader. You can replace every occurence of `AUD_Time.ScaledTicksMsec` by `Time.GetTicksMsec()`, and it will fully work, but will not take time-scale and pauses into account.

@subsection enable Enable AUD_Fader via AUD_Time

To keep the plugin flexible, time handling is abstracted through
@ref AUD_Time and @ref AUD_ILocalTime.

@subsubsection no_scale Very simple – No time scale

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

@subsubsection scale Simple – Use provided scale implementation

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

@subsubsection advanced Advanced – Custom implementation

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

@section tutorial_gun Example Tutorial – Gun Sound

This tutorial demonstrates how to design a gun sound using GaudioProcessTree.

@subsection gun_structure Decomposing the sound

A rapid-fire gun sound can be split into:

- Attack sound
- Hold sound
- Tail sound

Usually, you would typically have some glue between each of these events, and an associated sound to play.  
Instead, you can build a processing tree and have a much simpler glue - that is, start and stop playing the "gun sound", whatever that even means.

@subsection gun_tree Example processing tree

![Gun example](gun_example.png)

- A layerer plays the impact sound and a sequencer together
- The sequencer fades in the hold sound, then plays the tail sound once the fader starts fading out.

@subsection adding_variation Adding variation

Let's add some sugar coating: Maybe we want to pick random sounds with random pitch scales for the impact and tail sound, so it does not get too repetitive.

You can do that using an additional @ref AUD_RandomSound or @ref AUD_ParallelSound.

![Parallelizer](parallelizer.png)
![Parallelizer settings](parallelizer_settings.png)

Maybe we want to go one step further, and layer the impact sound as multiple randomized sounds:

![Layerer](layerer.png)

This approach keeps gameplay code simple while allowing rich and flexible in-editor sound design.  