using Godot;
using System;

public partial class VictoryPlayer : Player
{
	[Export]
	private Node2D sun;
	[Export]
	private Node2D moon;
	private bool readied = false;
	private bool dir;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!readied)
		{
			if(BodyDistance <= 8)
			{
				readied = true;
				this.GetChild<AnimationPlayer>(0).Play("player_victory");
			}
			BodyDistance = Mathf.MoveToward(BodyDistance, 0, 60 * (float)delta);
            sun.Position = new Vector2(BodyDistance, 0);
            moon.Position = new Vector2(-BodyDistance, 0);
        }
        MoveSpeed = Mathf.MoveToward(MoveSpeed, 0, (4 + (MoveSpeed * 0.5f)) * (float)delta);
        RotationSpeed = Mathf.MoveToward(RotationSpeed, 0, 1 * (float)delta);
		this.Position += MoveDirection * MoveSpeed * (float)delta;
		this.Rotation += RotationSpeed * (float)delta;
	}

	public void CopyPlayer(Vector2 move, float speed, float distance, float rotation, float rotationSpeed)
	{
		base.MoveDirection = move;
		base.MoveSpeed = speed;
		base.BodyDistance = distance;
		this.GlobalRotation = rotation;
		this.RotationSpeed = rotationSpeed;
		base.InputEnabled = false;
		readied = false;
	}
}
