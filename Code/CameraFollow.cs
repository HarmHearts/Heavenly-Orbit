using Godot;
using System;

public partial class CameraFollow : Camera2D
{
	[Export]
	public Node2D target;
	[Export]
	public float interpolation;
	[Export]
	public float baseSpeed;
	[Export]
	public float lookAhead;
	[Export]
	public float rotationReach;

	private Player playerScript;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerScript = target as Player;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//calculate target point
		Vector2 targetPoint = target.Position;
		//lookahead
		if(!playerScript.Locked)
		{
            targetPoint += playerScript.MoveDirection * (playerScript.MoveSpeed * lookAhead);
		}
		else
		{
			if(!playerScript.LockedBody) targetPoint += target.Transform.X * rotationReach * playerScript.BodyDistanceNormalized;
			else targetPoint += -target.Transform.X * rotationReach * playerScript.BodyDistanceNormalized;
        }
		//move camera
        this.Position = this.Position.MoveToward(targetPoint, baseSpeed * (float)delta);
        this.Position = this.Position.Lerp(targetPoint, interpolation * (float)delta);
		ForceUpdateScroll();
    }
}
