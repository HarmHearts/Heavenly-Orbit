using Godot;
using System;

[Tool]
public partial class GravityZone : TextureRect
{
	[Export]
	public Vector2 gravity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        CollisionShape2D shape = this.FindChild("Collider") as CollisionShape2D;
        ((RectangleShape2D)shape.Shape).Size = this.Size;
        shape.Position = this.Size / 2;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Resize()
    {
        if (!Engine.IsEditorHint()) return;
        CollisionShape2D shape = this.FindChild("Collider") as CollisionShape2D;
        ((RectangleShape2D)shape.Shape).Size = this.Size;
        shape.Position = this.Size / 2;
    }

	public void OnEntered(Node2D body)
	{
		GD.Print("A");
		if (body is PlayerCollide)
		{
			((PlayerPlanet)body.GetParent()).Gravity = this.gravity;
		}
	}

    public void OnExited(Node2D body)
    {
        if (body is PlayerCollide)
        {
			if (((PlayerPlanet)body.GetParent()).Gravity == this.gravity)
			{
                ((PlayerPlanet)body.GetParent()).Gravity = Vector2.Zero;
            }
        }
    }
}
