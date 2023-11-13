using Godot;
using System;

public partial class Spinner : AnimatedSprite2D, IToggleable
{
	[Export]
	private float spinSpeed;
	[Export]
	private bool active;
	[Export]
	private bool toggleable;

	private Tween tween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.SpeedScale = spinSpeed;
		if(active) this.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnEntered(Node2D body)
	{
		if(body.GetOwner<Node2D>() is Player && body.GetOwner<Player>().RotationSpeed != spinSpeed && body.GetOwner<Player>().alive && active)
		{
			if(tween != null)
			{
				if (tween.IsRunning()) return;
				else tween.Kill();
			}
			Player player = body.GetOwner<Player>();
			tween = GetTree().CreateTween().BindNode(player);
			tween.TweenProperty(player, "_rotationSpeed", spinSpeed, 0.5);
		}
	}

	public void Activate()
	{
		active = true;
		this.Play();
	}

	public void Deactivate()
	{
		active = false;
		this.Pause();
	}

	public void Toggle()
	{
		if (!toggleable) return;
		active = !active;
		if (active) this.Play();
		else this.Pause();
	}
}