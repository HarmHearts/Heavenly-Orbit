using Godot;
using System;

public partial class Level : Node2D
{
	[Export]
	public string levelName;
	[Export]
	public int world;
	[Export]
	public int number;
	[Export]
	public int gems;
	[Export]
	public string background;
	[Export]
	public bool initialPlayerSpin;
	[Export]
	public Texture2D floorTexture;
	[Export]
	public Texture2D wallTexture;
	[Export]
	public Vector2 wrapBounds;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        CountGems();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

	public void CountGems()
	{
        gems = this.GetNode<TileMap>("Entities").GetUsedCellsById(0, 0).Count;
    }

	public void ToggleWalls()
	{
		foreach(Node node in GetTree().GetNodesInGroup("Toggle"))
		{
			((IToggleable)node).Toggle();
		}
	}
}
