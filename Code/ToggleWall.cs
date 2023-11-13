using Godot;
using System;

public partial class ToggleWall : Sprite2D, IToggleable
{
	[Export]
	private bool raised;
	private Tween tween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (raised)
        {
            this.GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D").SetDeferred("disabled", false);
			this.Frame = 0;
        }
        else
        {
            this.GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D").SetDeferred("disabled", true);
			this.Frame = 3;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Toggle()
	{
		raised = !raised;
		if(tween != null) tween.Kill();
        tween = this.CreateTween();
		if(raised)
		{
			this.GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D").SetDeferred("disabled", false);
            tween.TweenProperty(this, "frame", 0, 0.4);
        }
		else
		{
            this.GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D").SetDeferred("disabled", true);
            tween.TweenProperty(this, "frame", 3, 0.4);
        }
    }
}
