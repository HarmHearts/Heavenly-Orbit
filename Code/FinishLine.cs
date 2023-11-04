using Godot;
using System;

[Tool]
public partial class FinishLine : NinePatchRect
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
    }
}
