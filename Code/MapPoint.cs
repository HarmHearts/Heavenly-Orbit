using Godot;
using System;

public partial class MapPoint : Node2D
{
	[Export]
	public string worldName;
	[Export]
	public int worldNumber;
	[Export]
	private MapPoint _right;
	[Export]
	private MapPoint _left;
    [Export]
    private MapPoint _up;
    [Export]
    private MapPoint _down;
    [Export]
	private bool _accessible;

	public MapPoint Right { get => _right; }
    public MapPoint Left { get => _left; }
    public MapPoint Up { get => _up; }
    public MapPoint Down { get => _down; }
	public bool Accessible 
	{ 
		get => _accessible; 
		set 
		{ 
			_accessible = value;
            if (_accessible)
            {
                this.Modulate = new Color(1, 1, 1, 1);
            }
            else
            {
                this.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
            }
        } 
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (Accessible)
		{
			this.Modulate = new Color(1, 1, 1, 1);
		}
		else
		{
            this.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
