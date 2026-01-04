@mainpage GaudioProcessTree

GaudioProcessTree is a Godot plugin that allows you to describe audio processing
using tree-like structures, inspired by Node2D / Node3D parenting.

It enables :
- Relative volume and pitch propagation, in a similar fashion to local and global transform parenting on Node2D/Node3D.
- Bindings for time-scale compliance.
- Various built-in processing modules, like fader, randomizer, sequencer, etc.
- Wrapping of Godot's StreamPlayer/2D/3D behind a single abstract interface to make GaudioProcessTrees cross compatible with any "spatialness". 

This documentation is split into the following sections:

- @ref 1_s1_getting_started
- @ref 2_editor_integration
- @ref 1_concepts
- @ref 2_time_dependency
- @ref 3_tutorial_gun