using Godot;
using System;

public partial class LevelScore : Resource
{
    [Export]
    public double time;
    [Export]
    public int gems;
    [Export]
    public int maxGems;
    [Export]
    public int rating;

    public LevelScore()
    {

    }

    public LevelScore(double time, int gems, int maxGems, int rating)
    {
        this.time = time;
        this.gems = gems;
        this.maxGems = maxGems;
        this.rating = rating;
    }

}
