using System.Collections.Generic;
using Godot;

namespace GaudioProcessTree.Nodes.Modules;
/// <summary>
/// Instead of playing all the layers together, the sequencer waits for each child to finish playing to play the next one. <br/>
/// The children order in tree is the order in which the sounds will be played.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Finished"/> fires once the whole sequence has completed playing, that is, its last children has fired Finished.
/// </para>
/// </remarks>
[GlobalClass, Tool, Icon("res://addons/GaudioProcessTree/icons/gaudio_sequencer.svg")]
public partial class AUD_Sequencer : AUD_Layerer
{
    private LinkedListNode<AUD_Sound> _currentSound;
    public override void Play()
    {
        _currentSound = _layers.First;
        PlayCurrent();
    }

    private void PlayCurrent()
    {
        if (_currentSound == null)
        {
            ForwardFinished();
            return;
        }

        _currentSound.Value.Finished += PlayNext;
        _currentSound.Value.Play();
    }

    private void PlayNext()
    {
        _currentSound.Value.Finished -= PlayNext;

        _currentSound = _currentSound.Next;
        PlayCurrent();
    }

    public override void Stop()
    {
        if (_currentSound == null)
            return;

        _currentSound.Value.Finished -= PlayNext;
        _currentSound.Value.Stop();
        _currentSound = null;
    }
}