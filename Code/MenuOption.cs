using Godot;
using System;

public partial class MenuOption : Control
{
	[Export]
	public bool accessible;

	[Signal]
	public delegate void OnSelectEventHandler();
    [Signal]
    public delegate void OnHighlightEventHandler();
    [Signal]
    public delegate void OnUnhighlightEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public MenuOption()
	{
		accessible = true;
	}

    public virtual void Selected()
	{
		EmitSignal(SignalName.OnSelect);
	}

	public virtual void Highlighted()
	{
        EmitSignal(SignalName.OnHighlight);
    }

	public virtual void Unhighlighted()
	{
        EmitSignal(SignalName.OnUnhighlight);
    }

	public virtual void Enable()
	{
		accessible = true;
	}

    public virtual void Disable()
    {
		accessible = false;
    }
}
