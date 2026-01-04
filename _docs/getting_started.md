@page getting_started Getting Started

@section installation Installation

To get started, clone the repository into your project's `addons` directory:

```sh
git clone git@github.com:LekloOno/GaudioProcessTree.git
```

If you want to, you can also make it a submodule of your own repository, using the following command:
```sh
git submodule add git@github.com:LekloOno/GaudioProcessTree.git GaudioProcessTree
```
(Assuming the plugin is currently in this `GaudioProcessTree` relative path)

Then, you must compile, and enable it under `Project > Project Settings > Plugins`.

@section brief_usage Brief usage

You can start building your processing tree using the different @ref AUD_Sound nodes.  

All `AUD_Sound` tree branches should terminate with an @ref AUD_StreamPlayer leaf node (which itself is an `AUD_Sound`). More on than in @ref aud_streamplayer section.  

I recommend reading through the @ref aud_sound section to get a grasp of the basic principles and features of this plugin.

@section dependency_note Important note on dependency

The plugin is almost entirely standalone, but the provided module @ref AUD_Fader depends on a time provider abstraction.

@note
The reason for this is I internally already used a `PHX_Time` static class that kept track of time with time-scale and pause awareness. Since it's not specific to audio, I didn't thought it would make sense to make it part of the plugin, so instead, I abstracted it away through some interfaces that allows you to implement it however you want.

You have four option:
- Simplest - Don't use or modify @ref AUD_Fader to not rely on AUD_Time
- Simple - Implement `AUD_Time` without time scaling
- Intermediate - Use the provided time scale template.
- Advanced - Implement your own `AUD_ILocalTime`.

A step-by-step guide for each of these options is available in @ref time_dependency.

@section editor_integration Editor Integration

The plugin integrates directly into the Godot editor.

It provides:
- Dedicated nodes icons
- Configuration warnings
- In-depth Editor-time properties validation and hints

These features guide users through the set up of an audio processing trees
and help avoid invalid configurations or unexpected behaviors.

@subsection icons Dedicated icons

![Dedicated Icons](icons.png)

@subsection warnings Configuration warnings example

![Configuration Warnings](warnings_example.png)
