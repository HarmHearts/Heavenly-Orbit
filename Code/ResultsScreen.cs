using Godot;
using System;

public partial class ResultsScreen : CanvasLayer
{
    public void QuitOut()
    {
        GameOverlay.ScreenFader.FadeScreen("screen_fade_grid", new Callable(GameManager.instance, "ToWorldMap"));
    }

    public void Continue()
    {

    }
}
