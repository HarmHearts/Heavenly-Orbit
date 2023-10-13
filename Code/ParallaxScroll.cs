using Godot;
using System;

public partial class ParallaxScroll : Node2D
{
	[Export]
	public float scroll;
	[Export]
	public int width;
	
	Vector2 lastPos;
	Vector2 offset;
	ShaderMaterial material;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		lastPos = this.GlobalPosition;
		material = (ShaderMaterial)this.Material;
		offset = Vector2.Zero;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 newPos = this.GlobalPosition;
		offset += (newPos - lastPos) * scroll;
        if (offset.X > width) offset.X -= width;
        if (offset.X < -width) offset.X += width;
        if (offset.Y > width) offset.Y -= width;
        if (offset.Y < -width) offset.Y += width;
        ((ShaderMaterial)this.Material).SetShaderParameter("offset", offset);
		lastPos = newPos;
    }
}
