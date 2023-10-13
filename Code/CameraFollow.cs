using Godot;
using System;

public partial class CameraFollow : Node2D
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
		if(!playerScript.locked)
		{
            targetPoint += playerScript.moveDirection * (playerScript.moveSpeed * lookAhead);
		}
		else
		{
			if(!playerScript.lockedBody) targetPoint += target.Transform.X * rotationReach * (playerScript.bodyDistance / playerScript.maxDistance);
			else targetPoint += -target.Transform.X * rotationReach * (playerScript.bodyDistance / playerScript.maxDistance);
        }
		//move camera
        this.Position = this.Position.MoveToward(targetPoint, baseSpeed * (float)delta);
        this.Position = this.Position.Lerp(targetPoint, interpolation * (float)delta);
    }
}
