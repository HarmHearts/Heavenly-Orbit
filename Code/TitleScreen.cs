using Godot;
using System;

public partial class TitleScreen : Node2D
{
    public bool intro;
    public override void _Ready()
    {
        intro = true;
        if(GameManager.GameSave == null)
        {
            GetNode<Node>("%Continue").Free();
            GetNode<MenuList>("%Options").CallDeferred("Scan");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("Btn_Start"))
        {
            if(intro)
            {
                intro = false;
                GetNode<AnimationPlayer>("%AnimationPlayer").Seek(2);
                PressStart();
            }
        }
    }

    public void PressStart()
    {
        GetNode<CanvasLayer>("%MenuCanvas").Offset = Vector2.Zero;
        GetNode<CanvasItem>("%PressStart").Visible = false;
        GetNode<MenuList>("%Options").CallDeferred("Enable");
        AudioSystem.PlaySFX("Select");
    }

    public void NewGame()
    {
        GameManager.CreateSaveFile();
        //start new game
        GameOverlay.ScreenFader.FadeScreen("screen_wipe_box", new Callable(GameManager.instance, "ToWorldMap"));
    }

    public void Continue()
    {
        //go to world map at current world
        GameOverlay.ScreenFader.FadeScreen("screen_wipe_box", new Callable(GameManager.instance, "ToWorldMap"));
    }

    public void Options()
    {
        //load options scene
    }

    public void QuitGame()
    {
        GameManager.QuitGame();
    }
}
