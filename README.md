This plugin provides a way to describe audio processing in a tree-like structure, with various common tools and spatial abstraction to Godot AudioStreamPlayer.

It has been built as part of my engine extensions in an unrelated Godot project. Thus, the commit history might be quite inconsistent as it is an abrupt git filter-repo of the original project, that included undirectly related commits to this work that might have been scrapped in the process.

- [Getting started](#getting-started)
- [Important note on dependency](#important-note-on-dependency)
- [General Idea](#general-idea)
  - [AUD\_Sound common concept](#aud_sound-common-concept)
  - [AUD\_Module](#aud_module)
  - [AUD\_StreamPlayer](#aud_streamplayer)
- [Editor integration](#editor-integration)
- [AUD\_Time dependency](#aud_time-dependency)
  - [Simplest - Delete/Modifying AUD\_Fader](#simplest---deletemodifying-aud_fader)
    - [Delete](#delete)
    - [Modify](#modify)
  - [Enable AUD\_Fader via AUD\_Time](#enable-aud_fader-via-aud_time)
    - [Very simple - No time scale implementation](#very-simple---no-time-scale-implementation)
    - [Simple - Use provided scale implementation](#simple---use-provided-scale-implementation)
    - [Advanced - Implement your own `AUD_ILocalTime`](#advanced---implement-your-own-aud_ilocaltime)
      - [Example case for a completely modular approach](#example-case-for-a-completely-modular-approach)


# Getting started

To get started, you can either download the project, or execute the following command in your project's addons directory.
```sh
git clone git@github.com:LekloOno/GaudioProcessTree.git
```

You can then start building your processing tree using the different `AUD_Sound` nodes.  
All `AUD_Sound` tree branches should terminate with an `AUD_StreamPlayer` leaf node (which itself is an `AUD_Sound`). More on than in [AUD_StreamPlayer](#aud_streamplayer)'s section.

Note that the provided `AUD_Fader` module requires one extra configuration to be used, see [note on dependency](#important-note-on-dependency) and [dependency](#aud_time-dependency) sections.

The plugin is fully commented, and you can read a condensed introduction of the main concepts in the [General idea](#general-idea) section.

# Important note on dependency

The plugin is almost entirely standalone, but the provided module [AUD_Fader](nodes/modules/AUD_Fader.cs) depends on another tool I made internally, that is not specific to audio, thus not included in this plugin.

"PHX_Time" allows to get precise time ticks msec/usec with engine time scaling, but it has been abstracted away so you're not constrainted to use it.

You have four options -
- [**Simplest**](#simplest---deletemodifying-aud_fader) - Deleting or modifying AUD_Fader - duh
- [**Simple**](#very-simple---no-time-scale-implementation) - Implement AUD_Time with no time scale
- [**Medium**](#simple---use-provided-scale-implementation) - Use the provided time scale template.
- [**Advanced**](#advanced---implement-your-own-aud_ilocaltime) - Implement your own `AUD_ILocalTime`.

You can find a step by step guide of different options at the [end of this document](#aud_time-dependency).

# General Idea

The idea is to mimic the concepts of Node3D/2D parenting on Audio modules. You can parent any AUD_Sound derivative to any other one, and create very extensive processing chain in tree-like structures.  

## AUD_Sound common concept

Such parenting allows notably to modify pitch and volume relatively. That is, each [Sound](nodes/AUD_Sound.cs) has a Base, Relative, and Asbolute property for both its VolumeDB and PitchScale.

The relative property is publicly accessible, and be modified to influence the volume or pitch of a node, that will propagate to its branch, just like modifying the position of a node moves its children with it.

VolumeDb is additive, and PitchScale is multiplicative.  
That is, a `RelativeVolumeDb` of 0 and `RelativePitchScale` of 1 respectively gives the "original" volume and pitch scale of the target.

## AUD_Module

[Modules](nodes/modules/) are non-leaf nodes, that can parent other modules or stream players. They are various modules for various processing operations. Currently implemented are
- [AUD_LayeredSound](nodes/modules/AUD_LayeredSound.cs) - layering multiple sounds.
- [AUD_RandomSound](nodes/modules/AUD_RandomSound.cs) - randomizing sound picking and pitch.
- [AUD_ParallelSound](nodes/modules/AUD_ParallelSound.cs) running streams in parallel
- [AUD_Fader](nodes/modules/AUD_Fader.cs) - fading sounds in and out.

## AUD_StreamPlayer

[Stream Players](nodes/stream_players/) are the leaf nodes of the processing trees, and the concrete bind to godot. 

They wrap Godot's AudioStreamPlayer in a generic way.  
That is, AUD_Sound trees are compatible with any kind of AudioStreamPlayer, no matter if it is 2D, 3D, or non dimensional. This can enable some added flexibility, especially at prototyping phase when you are unsure of what exact player to use.

# Editor integration

The nodes are integrated in editor, and notably provides dedicated configuration warnings to guide the user through setting up his processing tree.

# AUD_Time dependency

## Simplest - Delete/Modifying AUD_Fader

### Delete
Of course, you can simply remove the [AUD_Fader](nodes/modules/AUD_Fader.cs) node. The rest of the plugin is fully standalone.

You can eventually implement your own later on by extending [AUD_Module](nodes/AUD_Module.cs).

### Modify
Another simple solution is to modify the [AUD_Fader](nodes/modules/AUD_Fader.cs) node. Typically, you can simply replace all mentions of `AUD_Time.ScaledTicksMsec` by `Time.GetTicksMsec()`. Recompile, and everything should work as standalone.

## Enable AUD_Fader via AUD_Time
Indeed, to not make the plugin too rigid, I didn't assume you would use my PHX_Time script, and instead provided an [AUD_Time](static/time/AUD_Time.cs) class and [AUD_ILocalTime](static/time/AUD_ILocalTime.cs) interface.  
[AUD_Time]() holds the static reference used by [AUD_Fader]() which is a [AUD_ILocalTime]().

The interface requires you to implement a method to retrieve the ScaledTicksMsec, and ScaledTicksUsec.

### Very simple - No time scale implementation

If you don't care about time scale at all, you can simply implement a static AUD_ILocalTime as such -
```cs
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
```
<small>Template provided in [static/time/templates/AUD_NoTimeScale.cs](static/time/templates/AUD_NoTimeScale.cs)</small>

Then, you can set this script as an autoload in `Project Settings > Globals > Path (folder icon)`, and select the `AUD_NoTimeScale` script.

### Simple - Use provided scale implementation

I provided a simple implementation very close to what I actually use in my personnal project. The only difference is that my AUD_ILocalTime implementation actually holds a static instance itself, so I can use it without relying on the audio plugin.

```cs
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
```
<small>Template provided in [static/time/templates/AUD_TimeScale.cs](static/time/templates/AUD_TimeScale.cs)</small>

Then, you can set this script as an autoload in `Project Settings > Globals > Path (folder icon)`, and select the `AUD_TimeScale` script.

### Advanced - Implement your own `AUD_ILocalTime`

You can also implement your own specification. For example, mine is a wrapper of my PHX_Time class.  

The general idea is simple - implement AUD_ILocalTime, and make sure an instance of this implementation is set as the static `AUD_Time` instance reference before running, so typically using an auto-load with `_EnterTree` like in the examples provided before.


#### Example case for a completely modular approach

In my case, I wanted my `PHX_Time` to be completely independant of `AUD_Time`, and still have this later rely on it.    
The setup is fairly simple. Below is the `PHX_Time` script, which is an auto-load singleton node.

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

Then, I have another autoload glue-node named `Bootstrap` which notably binds `PHX_Time` to `AUD_Time` in the following manner.
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
It is not necessary, godot should load auto-load nodes in the order you specified, so I could simply place the bootstrap later than PHX_Time in the list.  
Not only that - since my glue class only requires `PHX_Time` to be correctly initialized on properties access, and not construction, having `Bootstrap` execute first would most likely not cause any issue, as the properties are only accessed at runtime - that is after all auto-loads should be correctly initialized.

Yet, I just prefered to have this strict guarantee.

For further details, here is the my so called `StaticServiceLifeCycle`.

```cs
public static class StaticServiceLifeCycle<T>
{
    private static readonly TaskCompletionSource _tcs = new();

    public static Task Initialized => _tcs.Task;

    public static void MarkInitialized()
        => _tcs.TrySetResult();
}
```