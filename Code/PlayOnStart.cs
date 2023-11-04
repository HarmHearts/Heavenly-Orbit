using Godot;
using System;

public partial class PlayOnStart : AnimationPlayer
{
	[Export] private StringName targetAnimation; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Play(targetAnimation);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
