using System;
using System.Collections.Generic;
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

    protected override void EnterTreeSpec() =>
        ModuleEnterTree();
    protected abstract void ModuleEnterTree();

    public override void _ExitTree()
    {
        if (!Engine.IsEditorHint())
            return;
    }

    public override void _Notification(int what)
    {
        if (what != NotificationChildOrderChanged)
            return;
        
        List<AUD_Sound> children = [];
        foreach(Node node in GetChildren())
            if (node is AUD_Sound sound)
                children.Add(sound);

        OnSoundChildChanged(children);

        if (Engine.IsEditorHint())
            UpdateConfigurationWarnings();
    }

    /// <summary>
    /// Called when a sound child entered the tree. <br/>
    /// This callback is typically used to auto-reference it in its attributes.
    /// </summary>
    /// <param name="sound">The new child node.</param>
    protected abstract void OnSoundChildChanged(List<AUD_Sound> sounds);
}