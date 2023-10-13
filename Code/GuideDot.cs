using Godot;
using System;

public partial class GuideDot : Sprite2D
{
	[Export]
	public Texture2D dot;
	[Export]
	public Texture2D arrow;
	[Export]
	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        this.GlobalRotation = player.moveDirection.Angle();
        if (player.locked) this.Texture = arrow;
		else
		{
			this.Texture = dot;
			this.GlobalRotation = 0;
		}
		if (player.lockedBody) this.FlipV = true;
		else this.FlipV = false;
	}
}
