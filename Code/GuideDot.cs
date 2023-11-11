using Godot;
using System;

public partial class GuideDot : Sprite2D
{
	[Export]
	public Texture2D dot;
	[Export]
	public Texture2D arrow;

	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        player = this.Owner as Player;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        this.GlobalRotation = player.MoveDirection.Angle();
        if (player.Locked) this.Texture = arrow;
		else
		{
			this.Texture = dot;
			this.GlobalRotation = 0;
		}
		if (player.LockedBody) this.FlipV = true;
		else this.FlipV = false;
	}
}
