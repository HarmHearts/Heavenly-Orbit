using Godot;
using System;

public partial class PauseScreen : CanvasLayer
{
    [Export]
    private Control cursor;
    [Export]
    private Vector2 cursorOffset;
    private int optionIndex = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        this.GetTree().Root.SetInputAsHandled();
        if (@event.IsActionPressed("Dpad_Up"))
        {
            if(optionIndex > 0)
            {
                optionIndex -= 1;
                cursor.GlobalPosition = ((Control)GetNode<Node>("%Options").GetChildren()[optionIndex]).GlobalPosition + cursorOffset;
                AudioSystem.PlaySFX("Move");
            }
            //TODO: else play nope sound
        }
        if (@event.IsActionPressed("Dpad_Down"))
        {
            if (optionIndex < 2)
            {
                optionIndex += 1;
                cursor.GlobalPosition = ((Control)GetNode<Node>("%Options").GetChildren()[optionIndex]).GlobalPosition + cursorOffset;
                AudioSystem.PlaySFX("Move");
            }
            //TODO: else play nope sound
        }
        if (@event.IsActionPressed("Btn_Start") || @event.IsActionPressed("Btn_B"))
        {
            Unpause();
        }
        if(@event.IsActionPressed("Btn_A"))
        {
            switch(optionIndex)
            {
                case 0:
                    Unpause();
                    break;
                case 1:
                    ((GameplayManager)this.GetParent()).ReloadLevel();
                    Unpause();
                    break;
                case 2:
                    GetTree().Quit();
                    break;
                default: 
                    break;
            }
        }
        if (@event.IsActionReleased("Btn_A"))
        {
            this.GetParent().GetNode("GameplayHUD").GetNode<AnimationPlayer>("%MiniSunAnim").Stop();
            Player player = this.GetParent().GetNode<Player>("Player");
            if (player.Locked && player.LockedBody)
            {
                player.ForceUnlock();
            }
        }
        if (@event.IsActionReleased("Btn_B"))
        {
            this.GetParent().GetNode("GameplayHUD").GetNode<AnimationPlayer>("%MiniMoonAnim").Stop();
            Player player = this.GetParent().GetNode<Player>("Player");
            if (player.Locked && !player.LockedBody)
            {
                player.ForceUnlock();
            }
        }
    }

    public void Setup()
    {
        optionIndex = 0;
        cursor.GlobalPosition = ((Control)GetNode<Node>("%Options").GetChildren()[optionIndex]).GlobalPosition + cursorOffset;
    }

    public void Unpause()
    {
        try
        {
            this.GetParent().GetNode<ScreenFader>("%ScreenFader").FadeScreen("screen_fade_quick", new Callable(this.GetParent(), "UnpauseGame"), true);
        }
        catch (TransitionInterruptException e)
        {
            GD.Print(e);
            return;
        }
        Setup();
        AudioSystem.PlaySFX("Pause");
        GetTree().Paused = false;
    }
}
