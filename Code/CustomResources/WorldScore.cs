using Godot;
using Godot.Collections;
using System;

public partial class WorldScore : Resource
{
    [Export]
    public int unlockIndex;
    [Export]
    public Godot.Collections.Array<LevelScore> levelScores;

    public bool Unlocked { get => unlockIndex >= 0; }

    public WorldScore()
    {
        unlockIndex = -1; //TEST
        levelScores = new Array<LevelScore>();
    }
}
