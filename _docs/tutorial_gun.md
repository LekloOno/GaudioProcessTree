@page tutorial_gun Example Tutorial – Gun Sound

This tutorial demonstrates how to design a gun sound using GaudioProcessTree.

@section gun_structure Decomposing the sound

A rapid-fire gun sound can be split into:

- Attack sound
- Hold sound
- Tail sound

Usually, you would typically have some glue between each of these events, and an associated sound to play.  
Instead, you can build a processing tree and have a much simpler glue - that is, start and stop playing the "gun sound", whatever that even means.

@section gun_tree Example processing tree

![Gun example](gun_example.png)

- A layerer plays the impact sound and a sequencer together
- The sequencer fades in the hold sound, then plays the tail sound once the fader starts fading out.

@section adding_variation Adding variation

Let's add some sugar coating: Maybe we want to pick random sounds with random pitch scales for the impact and tail sound, so it does not get too repetitive.

You can do that using an additional @ref AUD_RandomSound or @ref AUD_ParallelSound.

![Parallelizer](parallelizer.png)
![Parallelizer settings](parallelizer_settings.png)

Maybe we want to go one step further, and layer the impact sound as multiple randomized sounds:

![Layerer](layerer.png)

This approach keeps gameplay code simple while allowing rich and flexible in-editor sound design.  