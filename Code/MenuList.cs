using Godot;
using System;
using System.Collections.Generic;

public partial class MenuList : Control
{
    [Export]
    private Control cursor;
    [Export]
    private Vector2 cursorOffset;
    [Export]
    private bool hasControl;
    [Export]
    private bool wrapping;
    [Export]
    private MenuList parent;
	private List<MenuOption> options;
	private int optionIndex;

    public int MenuIndex
    {
        get => optionIndex; set => optionIndex = value;
    }

    [Signal]
    public delegate void OnCancelEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Scan();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (!hasControl) return;
        this.GetTree().Root.SetInputAsHandled();
        if (@event.IsActionPressed("Dpad_Up"))
        {
            if (optionIndex > 0 || wrapping)
            {
                if (!options[(optionIndex - 1) % options.Count].accessible)
                {
                    AudioSystem.PlaySFX("Bump");
                    return;
                }
                options[optionIndex].Unhighlighted();
                optionIndex -= 1;
                optionIndex = optionIndex % options.Count;
                cursor.GlobalPosition = (options[optionIndex]).GlobalPosition + cursorOffset;
                options[optionIndex].Highlighted();
                AudioSystem.PlaySFX("Move");
            }
            else
            {
                AudioSystem.PlaySFX("Bump");
            }
        }
        if (@event.IsActionPressed("Dpad_Down"))
        {
            if (optionIndex < options.Count - 1 || wrapping)
            {
                if (!options[(optionIndex + 1) % options.Count].accessible)
                {
                    AudioSystem.PlaySFX("Bump");
                    return;
                }
                options[optionIndex].Unhighlighted();
                optionIndex += 1;
                optionIndex = optionIndex % options.Count;
                cursor.GlobalPosition = (options[optionIndex]).GlobalPosition + cursorOffset;
                options[optionIndex].Highlighted();
                AudioSystem.PlaySFX("Move");
            }
            else
            {
                AudioSystem.PlaySFX("Bump");
            }
        }
        if (@event.IsActionPressed("Btn_A"))
        {
            AudioSystem.PlaySFX("Select");
            options[optionIndex].Selected();
        }
        if (@event.IsActionPressed("Btn_B"))
        {
            this.CancelMenu();
        }
    }

    public void Scan()
    {
        options = new List<MenuOption>();
        foreach (Node node in this.GetChildren())
        {
            if (node is MenuOption) options.Add((MenuOption)node);
        }
        optionIndex = 0;
    }

    public virtual void SetCursor()
    {
        optionIndex = optionIndex % options.Count;
        cursor.GlobalPosition = (options[optionIndex]).GlobalPosition + cursorOffset;
        options[optionIndex].Highlighted();
    }

    public virtual void Enable()
    {
        hasControl = true;
        SetCursor();
    }

    public virtual void Disable() 
    { 
        hasControl = false; 
    }

    public virtual void CancelMenu()
    {
        //return to parent
        if(parent != null)
        {
            parent.Enable();
            this.Disable();
            //TODO: cancel sound
        }
        EmitSignal(SignalName.OnCancel);
    }

}
