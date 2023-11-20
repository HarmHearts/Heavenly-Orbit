using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class GameSave : Resource
{
    [Export]
    private int _currentWorld;
    [Export]
    private Array<int> _worldProgress;
    [Export]
    private Dictionary<Vector2I,  LevelScore> _scoreMap;

    public int CurrentWorld {  get => _currentWorld; set => _currentWorld = value; }
    public int WorldCount { get => _worldProgress.Count; }

    public GameSave()
    {
        _currentWorld = 0;
        _worldProgress = new Array<int>();
        _scoreMap = new Dictionary<Vector2I, LevelScore>();
    }

    public void AddWorld()
    {
        _worldProgress.Add(-1);
    }

    public bool WorldExists(int world)
    {
        return world < _worldProgress.Count;
    }
    
    public int GetWorldProgress(int world)
    {
        return _worldProgress[world];
    }

    public bool IsWorldUnlocked(int world)
    {
        return (_worldProgress[world] >= 0);
    }

    public bool IsLevelUnlocked(int world, int level)
    {
        return level <= _worldProgress[world];
    }

    public void ProgressWorld(int world, int progress)
    {
        if (_worldProgress[world] < progress) _worldProgress[world] = progress;
        if(progress >= GameManager.LevelIndex.GetLevelCount(world))
        {
            try
            {
                ProgressWorld(world + 1, 0);
            }
            catch (Exception e)
            {
                return;
            }
        }
    }

    public bool HasLevelScore(int world, int level)
    {
        return _scoreMap.ContainsKey(new Vector2I(world, level));
    }

    public LevelScore GetLevelScore(int world, int level)
    {
        try
        {
            return _scoreMap[new Vector2I(world, level)];
        }
        catch(Exception e)
        {
            return null;
        }
    }

    public void AddLevelScore(int world, int level, LevelScore score)
    {
        if(HasLevelScore(world, level))
        {
            _scoreMap[new Vector2I(world, level)] = score;
        }
        else
        {
            _scoreMap.Add(new Vector2I(world, level), score);
        }
    }

}
