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
    public bool canMove;

    [Signal]
    public delegate void WorldSelectedEventHandler(int world);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        canMove = true;
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
        if (!canMove) return;
        if (@event.IsActionPressed("Dpad_Up") && !moveTween.IsRunning())
        {
			targetPoint = currentPoint.Up;
			if(targetPoint != null && targetPoint.Accessible)
			{
                MoveToNew();
            }
			else targetPoint = null;
        }
        if (@event.IsActionPressed("Dpad_Down") && !moveTween.IsRunning())
        {
            targetPoint = currentPoint.Down;
            if (targetPoint != null && targetPoint.Accessible)
            {
                MoveToNew();
            }
            else targetPoint = null;
        }
        if (@event.IsActionPressed("Dpad_Left") && !moveTween.IsRunning())
        {
            targetPoint = currentPoint.Left;
            if (targetPoint != null && targetPoint.Accessible)
            {
                MoveToNew();
            }
            else targetPoint = null;
        }
        if (@event.IsActionPressed("Dpad_Right") && !moveTween.IsRunning())
        {
            targetPoint = currentPoint.Right;
            if (targetPoint != null && targetPoint.Accessible)
            {
                MoveToNew();
            }
            else targetPoint = null;
        }
        if(@event.IsActionPressed("Btn_A") && !moveTween.IsRunning())
        {
            canMove = false;
            GameOverlay.ScreenFader.FadeScreen("screen_wipe_grid", new Callable(this, "SelectLevel"));
            AudioSystem.PlaySFX("Select");
        }
        if (@event.IsActionPressed("Btn_B"))
        {
            canMove = false;
            GameOverlay.ScreenFader.FadeScreen("screen_fade_quick", new Callable(this, "Leave"));
            //AudioSystem.PlaySFX("Cancel");
        }
    }

    public void SelectLevel()
    {
        GameManager.GameSave.CurrentWorld = currentPoint.worldNumber;
        GameManager.SaveGame();
        GameManager.LevelSelect(currentPoint.worldNumber);
    }

    public void Leave()
    {
        GameManager.GameSave.CurrentWorld = currentPoint.worldNumber;
        GameManager.SaveGame();
        GameOverlay.ScreenFader.UnfadeScreen("screen_fade_quick");
        GameManager.TitleScreen();
    }

    public void Initialize()
    {
        moveTween = this.CreateTween();
        moveTween.TweenProperty(this, "position", currentPoint.Position, moveTime);
        FocusNewPoint();
    }

    public void MoveToNew()
    {
        moveTween = this.CreateTween();
        moveTween.TweenProperty(this, "position", targetPoint.Position, moveTime);
        currentPoint = targetPoint;
        FocusNewPoint();
    }

    public void FocusNewPoint()
    {
        GetNode<RichTextLabel>("%WorldTitle").Text = "[center]" + currentPoint.worldName;
    }

    public void SwapPoint(MapPoint point)
    {
        currentPoint = point;
        targetPoint = null;
        moveTween = this.CreateTween();
        this.Position = currentPoint.Position;
        moveTween.TweenProperty(this, "position", currentPoint.Position, moveTime);
        FocusNewPoint();
    }
}
