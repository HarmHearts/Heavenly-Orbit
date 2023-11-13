using Godot;
using System;

public partial class FloorButton : Node2D
{
	[Export]
	private bool sticky;

    private bool pressed;

    [Signal]
	public delegate void ButtonPressedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnEntered(Node2D body)
	{
		if (pressed) return;
		pressed = true;
		EmitSignal(SignalName.ButtonPressed);
		this.GetNode<Sprite2D>("ButtonSprite").Frame = 1;
		//TODO: add real sounds
		AudioSystem.PlaySFX("Lock");
	}

	public void OnExited(Node2D body)
	{
		if (!sticky)
		{
			pressed = false;
            this.GetNode<Sprite2D>("ButtonSprite").Frame = 0;
            //TODO: add real sounds
            AudioSystem.PlaySFX("Launch");
        }
	}
}
