# GaudioProcessTree

[![Documentation](https://img.shields.io/badge/docs-GaudioProcessTree-blue)](https://lekloono.github.io/GaudioProcessTree/)  

GaudioProcessTree is a Godot c# plugin that allows you to describe audio processing
using tree-like structures, inspired by Node2D / Node3D parenting.

## Features overview

The plugin enables:
- Relative volume and pitch propagation, in a similar fashion to local and global transform parenting on Node2D/Node3D.
- Dynamic time-relative pitch scaling, to stretch and squeeze streams when time scale is changed (through any root node configuration, or through the specific AUD_TimeScaler)
- Binding interfaces for time-scale compliance and ready-to-use implementing templates.
- Various built-in processing modules, like fader, randomizer, sequencer, etc.
- Wrapping of Godot's StreamPlayer/2D/3D behind a single abstract interface to make GaudioProcessTrees cross compatible with any "spatialness". 

You can find a brief introduction to the basic principles of the plugin in the [AUD_Sound common concepts](https://lekloono.github.io/GaudioProcessTree/guides.html#aud_sound) section.

## Table of Content

- [Getting Started](https://lekloono.github.io/GaudioProcessTree/getting_started.html) - Installation guide, brief usage description, and further configuration notes.
  - [Installation](https://lekloono.github.io/GaudioProcessTree/getting_started.html#installation)
  - [Brief usage](https://lekloono.github.io/GaudioProcessTree/getting_started.html#brief_usage)
  - [Dependency note](https://lekloono.github.io/GaudioProcessTree/getting_started.html#dependency_note)
  - [Editor integration overview](https://lekloono.github.io/GaudioProcessTree/getting_started.html#editor_integration)
- [Guides](https://lekloono.github.io/GaudioProcessTree/guides.html) - Various user guides to start building your process trees with the plugin.
  - [Base concepts overview](https://lekloono.github.io/GaudioProcessTree/guides.html#concepts)
  - [Time dependency setup](https://lekloono.github.io/GaudioProcessTree/guides.html#time_dependency)
  - [Gun sounds introductory tutorial](https://lekloono.github.io/GaudioProcessTree/guides.html#time_dependency#tutorial_gun)
- [Classes](https://lekloono.github.io/GaudioProcessTree/annotated.html) - Class list, also available as:
  - [index](https://lekloono.github.io/GaudioProcessTree/classes.html)
  - [hierarchy](https://lekloono.github.io/GaudioProcessTree/hierarchy.html)
  - [graph](https://lekloono.github.io/GaudioProcessTree/inherits.html).
- More to explore on the [full doc page](https://lekloono.github.io/GaudioProcessTree/index.html).