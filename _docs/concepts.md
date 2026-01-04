@ingroup guides
@page 2_concepts General Concepts

@section general_idea General Idea

The idea is to mimic Node2D / Node3D parenting for audio modules.

You can parent any `AUD_Sound` derivative to another one, creating
tree-like processing chains.

The plugin enables multiple features:
- Relative volume and pitch propagation, in a similar fashion to local and global transform parenting on Node2D/Node3D.
- Bindings for time-scale compliance.
- Various built-in processing modules, like fader, randomizer, sequencer, etc.
- Wrapping of Godot's StreamPlayer/2D/3D behind a single abstract interface to make GaudioProcessTrees cross compatible with any "spatialness". 

@section aud_sound AUD_Sound common concept

Each @ref AUD_Sound has:

- Base
- Relative
- Absolute

properties for both **VolumeDb** and **PitchScale**.

Relative properties propagate to children, similar to transform inheritance on Node2D/3D.

`VolumeDb` is additive since it is already on a logarithmic scale (decibels), and `PitchScale` is multiplicative.  
That is, a `RelativeVolumeDb` of 0 and `RelativePitchScale` of 1 respectively gives the "original" volume and pitch scale of the target.

Similarly, setting a parent node `Base` or `RelativeVolumeDb` to -1, and its children to +1, will effectively cancel out.  
For pitch scale, setting the parent's `Base` or `RelativePitchScale` to 2, and its children to 0.5 will cancel out.

@section aud_module AUD_Module

@ref AUD_Module are non-leaf nodes that can parent other modules or stream players. They provide common processing patterns.

Currently implemented modules:

Parenting AUD_Sound:
- @ref AUD_LayeredSound – layering multiple sounds
- @ref AUD_Fader – fading sounds in and out
- @ref AUD_Sequencer – sequential playback
- @ref AUD_Delayer – delayed playback
- 
Parenting AUD_StreamPlayer:
- @ref AUD_RandomSound – randomizing sound selection and pitch
- @ref AUD_ParallelSound – running randomized streams in parallel

@section aud_streamplayer AUD_StreamPlayer

@ref AUD_StreamPlayer are leaf nodes and provide the concrete binding to Godot.

They wrap Godot’s `AudioStreamPlayer` types in a generic way, allowing the same
processing tree to work with 2D, 3D, or non-spatial audio.