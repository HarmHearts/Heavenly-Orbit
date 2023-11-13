using Godot;
using System;

public partial class MovingPlatform : PathFollow2D, IToggleable
{
	[Export]
	public float speed;
	[Export]
	private bool moving;
	[Export]
	private bool reverse;
	[Export]
	private bool toggleable;

    public Vector2 motionVector;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((RectangleShape2D)this.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape).Size = this.GetNode<NinePatchRect>("PlatformGraphic").Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(moving)
		{
			motionVector = this.GlobalPosition;
			if(reverse) this.Progress -= speed * (float)delta;
            else this.Progress += speed * (float)delta;
			motionVector -= this.GlobalPosition;
			motionVector *= -1;
		}
		else
		{
			motionVector = Vector2.Zero;
		}
	}

	public void StartMovement()
	{
		moving = true;
	}

	public void ToggleMovement()
	{
		moving = !moving;
	}

	public void Reverse()
	{
		reverse = !reverse;
	}

	public void Toggle()
	{
		if (toggleable) moving = !moving;
	}
}
