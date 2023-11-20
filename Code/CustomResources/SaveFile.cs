using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class SaveFile : Resource
{
    [Export]
    public int currentWorld;
    [Export]
    public Godot.Collections.Array<WorldScore> scores;

    public SaveFile() 
    {
        scores = new Array<WorldScore> ();
        currentWorld = 0;
    }

    public void AddWorld()
    {
        scores.Add(new WorldScore());
    }

    public bool GetWorldUnlocked(int world)
    {
        try
        {
            return scores[world].Unlocked;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public int GetWorldProgress(int world)
    {
        return scores[world].unlockIndex;
    }

    public bool IsLevelUnlocked(int world, int level)
    {
        GD.Print("checking unlock for level " + level + " against " + scores[world].unlockIndex);
        return (level <= scores[world].unlockIndex);
    }

    public void AddScore(int world, int level, LevelScore score)
    {
        GD.Print("adding level at " + world + "-" + level);
        if(level < scores[world].levelScores.Count)
        {
            scores[world].levelScores[level] = score;
        }
        else
        {
            scores[world].levelScores.Add(score);
        }
    }

    public void UnlockLevel(int world, int level)
    {
        if (scores[world].unlockIndex < level) scores[world].unlockIndex = level;
        if (scores[world].unlockIndex >= GameManager.LevelIndex.worlds[world].levels.Count)
        {
            try
            {
                UnlockLevel(world + 1, 0);
            }
            catch (Exception e)
            {
                return;
            }
        }
    }

    public LevelScore GetScore(int world, int level)
    {
        GD.Print("getting score for level " + world + "-" + level);
        GD.Print("we have " + scores[world].levelScores.Count + " level scores");
        if (scores[world].levelScores.Count <= level)
        {
            GD.Print("no score found");
            return null;
        }
        return scores[world].levelScores[level];
    }
}
