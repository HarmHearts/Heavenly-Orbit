using Godot;
using System;

public partial class PlatformRailLine : Line2D
{
	private PackedScene capSprite = GD.Load<PackedScene>("res://Scenes/Constructs/rail_cap.tscn");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int i = 0; i < this.GetParent<Path2D>().Curve.PointCount; i++)
		{
			this.AddPoint(this.GetParent<Path2D>().Curve.GetPointPosition(i));
			this.AddPoint(this.GetParent<Path2D>().Curve.GetPointPosition(i));
			Node2D newScene = capSprite.Instantiate() as Node2D;
			this.AddChild(newScene);
			newScene.Position = this.GetParent<Path2D>().Curve.GetPointPosition(i);
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}