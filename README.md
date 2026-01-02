This plugin provides a way to describe audio processing in a tree-like structure, with various common tools and spatial abstraction to Godot AudioStreamPlayer.

It has been built as part of my engine extensions in an unrelated Godot project. Thus, the commit history might be quite inconsistent as it is an abrupt git filter-repo of the original project, that included undirectly related commits to this work that might have been scrapped in the process.

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