using Godot;
using System;

public partial class testing : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Position += new Vector2(28 * (float)delta, 10 * (float)delta);
	}
}
