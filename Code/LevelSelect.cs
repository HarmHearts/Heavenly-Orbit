using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Formats.Asn1.AsnWriter;

public partial class LevelSelect : Node2D
{
    public int worldNum;

    private PackedScene levelOption;

    public override void _Ready()
    {
        levelOption = GD.Load<PackedScene>("res://Scenes/Constructs/level_menu_option.tscn");
    }

    public void LoadLevels()
    {
        foreach(IndexLevel level in GameManager.LevelIndex.GetLevels(worldNum))
        {
            LevelOption newOption = levelOption.Instantiate() as LevelOption;
            this.GetNode("%Options").AddChild(newOption);
            newOption.level = level;
            newOption.levelScore = GameManager.GameSave.GetLevelScore(worldNum, level.levelNumber);
            newOption.CallDeferred("Configure");
            newOption.OnSelect += LevelSelected;
            if(!GameManager.GameSave.IsLevelUnlocked(worldNum, level.levelNumber))
            {
                newOption.CallDeferred("Disable");
            }
        }
        ((MenuList)this.GetNode("%Options")).Scan();
        ((MenuList)this.GetNode("%Options")).CallDeferred("SetCursor");
    }

    public void LevelSelected()
    {
        this.GetNode<MenuList>("%Options").Disable();
        GameOverlay.ScreenFader.FadeScreen("screen_fade_quick", new Callable(this, "SelectLevel"));
    }

    public void SelectLevel()
    {
        int level = this.GetNode<MenuList>("%Options").MenuIndex;
        GameManager.PlayLevel(GameManager.LevelIndex.GetLevel(worldNum, level));
    }

    public void Leave()
    {
        this.GetNode<MenuList>("%Options").Disable();
        GameOverlay.ScreenFader.FadeScreen("screen_fade_quick", new Callable(GameManager.instance, "ToWorldMap"), true);
    }
}
