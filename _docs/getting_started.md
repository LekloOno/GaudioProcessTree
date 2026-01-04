@defgroup guides User Guides
@ingroup guides
@page 1_getting_started Getting Started

@section installation Installation

To get started, clone the repository into your project's `addons` directory:

```sh
git clone git@github.com:LekloOno/GaudioProcessTree.git
```

If you want to, you can also make it a submodule of your own repository, using the following command:
```sh
git submodule add git@github.com:LekloOno/GaudioProcessTree.git GaudioProcessTree
```
(Assuming the plugin is currently in relative path `GaudioProcessTree`)

Then, you must compile, and enable it under `Project > Project Settings > Plugins`.

@section brief_usage Brief usage

You can start building your processing tree using the different @ref AUD_Sound nodes.  

All `AUD_Sound` tree branches should terminate with an @ref AUD_StreamPlayer leaf node (which itself is an `AUD_Sound`). More on than in @ref AUD_StreamPlayer_usage's section.

@section dependency_note Important note on dependency

The plugin is almost entirely standalone, but the provided module @ref AUD_Fader depends on a time provider abstraction.

@note
The reason for this is I internally already used a `PHX_Time` static class that kept track of time with time-scale and pause awareness. Since it's not specific to audio, I didn't thought it would make sense to make it part of the plugin, so instead, I abstracted it away through some interfaces that allows you to implement it however you want.

You have four option:
- Simplest - Don't use or modify @ref AUD_Fader to not rely on AUD_Time
- Simple - Implement `AUD_Time` without time scaling
- Intermediate - Use the provided time scale template.
- Advanced - Implement your own `AUD_ILocalTime`.

A step-by-step guide for each of these options is available in @ref 3_time_dependency.
