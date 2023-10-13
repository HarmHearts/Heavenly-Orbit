using Godot;
using System;

public partial class PlayerPlanet : Node2D
{
	[Export]
	public Texture2D normalPlanet;
	[Export]
	public Texture2D lockedPlanet;
	[Export]
	public bool planetToggle;

	private Sprite2D planetSprite;
	private Player player;

	[Signal]
	public delegate void WallCollisionEventHandler();
	[Signal]
	public delegate void BounceCollisionEventHandler(Vector2 normal);
	[Signal]
	public delegate void UnlockEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		planetSprite = this.GetChild(0) as Sprite2D;
		player = GetTree().CurrentScene as Player;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//fix rotation
		this.GlobalRotation = 0;
		//flip sprite
		if (this.GlobalPosition.X < this.GetParent<Node2D>().GlobalPosition.X)
		{
			planetSprite.FlipH = true;
		}
		else
		{
			planetSprite.FlipH = false;
		}
	}

	public void LockSprite()
	{
		planetSprite.Texture = lockedPlanet;
	}

	public void UnlockSprite()
	{
		planetSprite.Texture = normalPlanet;
	}
}
