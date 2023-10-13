using Godot;
using System;

public partial class PlayerCollide : AnimatableBody2D
{
	[Signal]
	public delegate void OnCollideEventHandler(KinematicCollision2D coll, bool moon);

	[Export]
	private bool moon;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		KinematicCollision2D coll = MoveAndCollide(Vector2.Zero, true);

		if (coll != null)
		{
			EmitSignal(SignalName.OnCollide, coll, moon);
		}
	}
}
