using Godot;
using System;

public partial class ScreenFader : AnimationPlayer
{
	private Callable fadeTarget;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AnimationFinished += this.FadeCompleted;
        fadeTarget = new Callable(this, MethodName.EmptyCall);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void FadeScreen(StringName transition)
    {
        if (this.IsPlaying()) throw new Exception("Transition is already in progress");
        this.Play(transition);
		fadeTarget = new Callable(this, MethodName.EmptyCall);
    }

    public void FadeScreen(StringName transition, Callable target)
	{
		if (this.IsPlaying()) throw new Exception("Transition is already in progress");
		this.Play(transition);
		fadeTarget = target;
	}

    public void UnfadeScreen(StringName transition)
    {
        if (this.IsPlaying()) throw new Exception("Transition is already in progress");
        this.PlayBackwards(transition);
        fadeTarget = new Callable(this, MethodName.EmptyCall);
    }

    public void UnfadeScreen(StringName transition, Callable target)
	{
        if (this.IsPlaying()) throw new Exception("Transition is already in progress");
        this.PlayBackwards(transition);
        fadeTarget = target;
    }

	public void FadeCompleted(StringName anim)
	{
		fadeTarget.Call();
	}

	public void EmptyCall()
	{
		return;
	}
}
