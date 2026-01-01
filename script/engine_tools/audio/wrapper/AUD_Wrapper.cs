using System;
using Godot;

[GlobalClass, Tool]
public abstract partial class AUD_Wrapper : AUD_Sound
{
    public override float VolumeDb
    {
        get => BaseVolumeDb + RelativeVolumeDb;
        protected set => BaseVolumeDb = value - RelativeVolumeDb; 
    }

    public override float PitchScale
    {
        get => BasePitchScale * RelativePitchScale;
        protected set => BasePitchScale = value / RelativePitchScale; 
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        if (!Engine.IsEditorHint())
            return;
        
        Callable childEntered = Callable.From<Node>(OnChildEnteredTree);
        Callable childExiting = Callable.From<Node>(OnChildExitingTree);

        if (!IsConnected(Node.SignalName.ChildEnteredTree, childEntered))
            Connect(Node.SignalName.ChildEnteredTree, childEntered);

        if (!IsConnected(Node.SignalName.ChildExitingTree, childExiting))
            Connect(Node.SignalName.ChildExitingTree, childExiting);
    }

    public override void _ExitTree()
    {
        if (!Engine.IsEditorHint())
            return;
    }

    private void OnChildEnteredTree(Node node)
    {
        if (!Engine.IsEditorHint())
            return;

        if (node is not AUD_Sound sound)
            return;
        
        OnSoundChildEnteredTree(sound);
        UpdateConfigurationWarnings();
    }
    /// <summary>
    /// Called when a sound child entered the tree. <br/>
    /// This callback is typically used to auto-reference it in its attributes.
    /// </summary>
    /// <param name="sound">The new child node.</param>
    protected abstract void OnSoundChildEnteredTree(AUD_Sound sound);

    private void OnChildExitingTree(Node node)
    {
        if (!Engine.IsEditorHint())
            return;
        
        if (node is not AUD_Sound sound)
            return;

        OnSoundChildExitingTree(sound);
        UpdateConfigurationWarnings();
    }
    /// <summary>
    /// Called when a sound child is about to exit the tree. <br/>
    /// This callback is typically used to auto-dereference it in its attributes.
    /// </summary>
    /// <param name="sound">The exiting child node.</param>
    protected abstract void OnSoundChildExitingTree(AUD_Sound sound);
}