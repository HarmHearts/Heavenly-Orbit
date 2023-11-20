using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelIndex : Resource
{
    public List<IndexWorld> worlds;

    public void IndexLevels()
    {
        worlds = new List<IndexWorld>();
        string path = "res://Scenes/Levels";
        using var dir = DirAccess.Open(path);
        if (dir != null)
        {
            int index = 0;
            foreach (string folder in dir.GetDirectories())
            {
                if (folder.Contains("World"))
                {
                    GD.Print("Logging new world " + folder);
                    IndexWorld world = new IndexWorld();
                    world.worldPath = path + "/" + folder;
                    world.worldNumber = index;
                    //get levels
                    world.levels = new List<IndexLevel>();
                    using var levelDir = DirAccess.Open(path + "/" + folder);
                    if(levelDir != null)
                    {
                        GD.Print(path + "/" + folder);
                        int levelIndex = 0;
                        foreach(string file in levelDir.GetFiles())
                        {
                            GD.Print("Loading level " + file);
                            PackedScene scene = GD.Load<PackedScene>(path + "/" + folder + "/" + file);
                            IndexLevel level = new IndexLevel();
                            level.levelName = scene.GetState().GetNodePropertyValue(0, 0).As<string>();
                            level.levelPath = path + "/" + folder + "/" + file;
                            level.levelNumber = scene.GetState().GetNodePropertyValue(0, 2).As<int>();
                            levelIndex += 1;
                            level.worldNumber = index;
                            world.levels.Add(level);
                        }
                    }
                    world.levels = world.levels.OrderBy(x => x.levelNumber).ToList();
                    worlds.Add(world);
                    index += 1;
                }
            }
        }
    }

    public IndexLevel GetLevel(int world, int level)
    {
        return worlds[world].levels[level];
    }

    public IndexWorld GetWorld(int world)
    {
        return worlds[world];
    }

    public IndexLevel[] GetLevels(int world)
    {
        GD.Print("getting levels for world " + world);
        GD.Print("we have " + worlds[world].levels.Count + " levels");
        return worlds[world].levels.ToArray();
    }

    public int GetLevelCount(int world)
    {
        return worlds[world].levels.Count;
    }
}

public struct IndexWorld
{
    public string worldPath;
    public int worldNumber;
    public List<IndexLevel> levels;
}

public struct IndexLevel
{
    public string levelName;
    public string levelPath;
    public int levelNumber;
    public int worldNumber;
}
