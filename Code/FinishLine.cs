using Godot;
using System;

[Tool]
public partial class FinishLine : NinePatchRect
{
	private bool win = false;
	[Signal]
	public delegate void OnWinEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        CollisionShape2D shape = this.FindChild("Collider", true) as CollisionShape2D;
        ((RectangleShape2D)shape.Shape).Size = this.Size;
        shape.Position = this.Size / 2;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Resize()
	{
		if (!Engine.IsEditorHint()) return;
        CollisionShape2D shape = this.FindChild("Collider", true) as CollisionShape2D;
        ((RectangleShape2D)shape.Shape).Size = this.Size;
		shape.Position = this.Size / 2;
    }

	public void OnEntered(Node2D body)
	{
		if(body.IsInGroup("Player") && !win)
		{
			win = true;
			EmitSignal(SignalName.OnWin);
		}
	}
}
