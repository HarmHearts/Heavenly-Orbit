using Godot;
using System;

[Tool]
public partial class GravityZone : NinePatchRect
{
	[Export]
	public Vector2 gravity;

    private PackedScene pulseEffect = GD.Load<PackedScene>("res://Scenes/Constructs/pulse_effect.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        CollisionShape2D shape = this.FindChild("Collider") as CollisionShape2D;
        ((RectangleShape2D)shape.Shape).Size = this.Size;
        shape.Position = this.Size / 2;
        TextureRect fill = this.FindChild("Fill") as TextureRect;
        fill.Size = this.Size - new Vector2(8, 8);
        fill.Position = new Vector2(4, 4);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Resize()
    {
        if (!Engine.IsEditorHint()) return;
        CollisionShape2D shape = this.FindChild("Collider", true) as CollisionShape2D;
        ((RectangleShape2D)shape.Shape).Size = this.Size;
        shape.Position = this.Size / 2;
        TextureRect fill = this.FindChild("Fill") as TextureRect;
        fill.Size = this.Size - new Vector2(8, 8);
        fill.Position = new Vector2(4, 4);
    }

    public void MakePulse(Vector2 pos)
    {
        Node2D effect = pulseEffect.Instantiate() as Node2D;
        this.AddChild(effect);
        effect.GlobalPosition = GetClosestPointOnEdge(pos);
    }

	public void OnEntered(Node2D body)
	{
		if (body is PlayerCollide)
		{
			((PlayerPlanet)body.GetParent()).Gravity = this.gravity;
            AudioSystem.PlaySFX("Gravity");
            MakePulse(body.GlobalPosition);
		}
	}

    public void OnExited(Node2D body)
    {
        if (body is PlayerCollide)
        {
			if (((PlayerPlanet)body.GetParent()).Gravity == this.gravity)
			{
                ((PlayerPlanet)body.GetParent()).Gravity = Vector2.Zero;
                AudioSystem.PlaySFX("Gravity");
                MakePulse(body.GlobalPosition);
            }
        }
    }

    public Vector2 GetClosestPointOnEdge(Vector2 point)
    {
        float topPos = this.GlobalPosition.Y;
        float bottomPos = this.GlobalPosition.Y + this.Size.Y;
        float leftPos = this.GlobalPosition.X;
        float rightPos = this.GlobalPosition.X + this.Size.X;

        return new Vector2(Mathf.Clamp(point.X, leftPos, rightPos), Mathf.Clamp(point.Y, topPos, bottomPos));
    }
}
