using Godot;
using System;

public partial class WorldMapCursor : Node2D
{
	[Export]
	private float moveTime;
    [Export]
    private float spinSpeed;
    [Export]
	private MapPoint currentPoint;
	private MapPoint targetPoint;
	private Tween moveTween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        moveTween = this.CreateTween();
        moveTween.TweenProperty(this, "position", currentPoint.Position, moveTime);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        this.Rotation += spinSpeed * (float)delta;
        foreach(Node child in this.GetChildren())
        {
            Node2D node2D = child as Node2D;
            node2D.GlobalRotation = 0;
        }
	}

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("Btn_A") && !moveTween.IsRunning())
        {
            //hi ruby!!!!
            //put the selecting a level code here!
        }
        if (@event.IsActionPressed("Dpad_Up") && !moveTween.IsRunning())
        {
			targetPoint = currentPoint.Up;
			if(targetPoint != null && targetPoint.Accessible)
			{
				moveTween = this.CreateTween();
                moveTween.TweenProperty(this, "position", targetPoint.Position, moveTime);
                currentPoint = targetPoint;
			}
			else targetPoint = null;
        }
        if (@event.IsActionPressed("Dpad_Down") && !moveTween.IsRunning())
        {
            targetPoint = currentPoint.Down;
            if (targetPoint != null && targetPoint.Accessible)
            {
                moveTween = this.CreateTween();
                moveTween.TweenProperty(this, "position", targetPoint.Position, moveTime);
                currentPoint = targetPoint;
            }
            else targetPoint = null;
        }
        if (@event.IsActionPressed("Dpad_Left") && !moveTween.IsRunning())
        {
            targetPoint = currentPoint.Left;
            if (targetPoint != null && targetPoint.Accessible)
            {
                moveTween = this.CreateTween();
                moveTween.TweenProperty(this, "position", targetPoint.Position, moveTime);
                currentPoint = targetPoint;
            }
            else targetPoint = null;
        }
        if (@event.IsActionPressed("Dpad_Right") && !moveTween.IsRunning())
        {
            targetPoint = currentPoint.Right;
            if (targetPoint != null && targetPoint.Accessible)
            {
                moveTween = this.CreateTween();
                moveTween.TweenProperty(this, "position", targetPoint.Position, moveTime);
                currentPoint = targetPoint;
            }
            else targetPoint = null;
        }
    }
}
