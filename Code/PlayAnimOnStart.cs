using Godot;
using System;

public partial class PlayAnimOnStart : AnimatedSprite2D
{
	[Export]
	public bool killOnFinish;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Die()
	{
		if(killOnFinish) this.QueueFree();
	}
}
