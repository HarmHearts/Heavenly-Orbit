using Godot;
using System;

public partial class PlayerPlanet : Node2D
{
	[Export]
	public Texture2D normalPlanet;
	[Export]
	public Texture2D lockedPlanet;
	[Export]
	public Texture2D shockedPlanet;
	[Export]
	public bool planetToggle;

	private Vector2 _gravity;

	private Sprite2D planetSprite;
	private Player player;

    PackedScene deathExplosion = GD.Load<PackedScene>("res://Scenes/Constructs/big_explosion.tscn");

    public Vector2 Gravity
    {
        get => _gravity;
        set
        {
            _gravity = value;
            player.CalculateGravity();
			EmitSignal(SignalName.GravityField);
        }
    }

    [Signal]
    public delegate void GravityFieldEventHandler(); 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		planetSprite = this.GetChild(0) as Sprite2D;
		player = this.Owner as Player;
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

	public void ShockSprite()
	{
        planetSprite.Texture = shockedPlanet;
    }

	public void Explode()
	{
		planetSprite.Texture = null;
		this.AddChild(deathExplosion.Instantiate());
		((Node2D)this.FindChild("Shadow")).Visible = false;
        ((Node2D)this.FindChild("PlanetSparkles")).Visible = false;
    }

	public void Reset()
	{
        planetSprite.Texture = normalPlanet;
        ((Node2D)this.FindChild("Shadow")).Visible = true;
        ((Node2D)this.FindChild("PlanetSparkles")).Visible = true;
    }
}
