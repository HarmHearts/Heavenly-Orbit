using Godot;
using System;

public partial class WorldMapCamera : Camera2D
{
    [Export]
    private float interpolation;
    [Export]
    private float baseSpeed;
    [Export]
    private Node2D target;
    private float leftBound;
    private float rightBound;
    private float upBound;
    private float downBound;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        target = GetNode<Node2D>("%Cursor");
        leftBound = this.LimitLeft + (GetViewportRect().Size.X / 2);
        rightBound = this.LimitRight - (GetViewportRect().Size.X / 2);
        upBound = this.LimitTop + (GetViewportRect().Size.Y / 2);
        downBound = this.LimitBottom - (GetViewportRect().Size.Y / 2);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //move camera
        this.Position = this.Position.MoveToward(target.Position, baseSpeed * (float)delta);
        this.Position = this.Position.Lerp(target.Position, interpolation * (float)delta);
        //do limits
        this.Position = new Vector2(Mathf.Clamp(this.Position.X, leftBound, rightBound), Mathf.Clamp(this.Position.Y, upBound, downBound));
        ForceUpdateScroll();
    }
}
