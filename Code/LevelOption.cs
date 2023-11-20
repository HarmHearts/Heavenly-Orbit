using Godot;
using System;

public partial class LevelOption : MenuOption
{
    public IndexLevel level;
    public LevelScore levelScore;

    public override void _Ready()
    {
    }

    public LevelOption() 
    {
    }

    public LevelOption(IndexLevel level)
    {
        this.level = level;
    }

    public void Configure()
    {
        this.GetNode<RichTextLabel>("%Title").Text = level.levelName;
        if (levelScore != null)
        {
            this.GetNode<RichTextLabel>("%HiScore").Text = "Best Run: " + TimeSpan.FromSeconds(levelScore.time).ToString(@"m\:ss\.fff") + " - " + levelScore.gems + "/" + levelScore.maxGems;
        }
        else
        {
            this.GetNode<RichTextLabel>("%HiScore").Text = "Not Played";
        }
    }

    public override void Highlighted()
    {
        base.Highlighted();
        this.Modulate = new Color(1, 1, 1, 1);
    }

    public override void Unhighlighted()
    {
        base.Unhighlighted();
        this.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    public override void Disable()
    {
        base.Disable();
        this.GetNode<RichTextLabel>("%Title").Text = "??????";
    }
}
