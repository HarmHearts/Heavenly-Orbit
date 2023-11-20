using Godot;
using System;
using System.IO;

public partial class GameManager : Node2D
{
	public const string SAVE_PATH = "user://savefile.res";

    private static PackedScene titleScreen;
	private static PackedScene worldMap;
	private static PackedScene levelSelect;
    private static PackedScene gameplay;
    private static PackedScene levelResults;

    public static GameManager instance;
    private static Node currentScene;
	private static int currentLevel;

	private static SaveFile _saveFile;
	private static LevelIndex _levelIndex;

	public static SaveFile SaveFile
	{
		get => _saveFile;
	}
	public static LevelIndex LevelIndex
	{
		get => _levelIndex;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		titleScreen = GD.Load<PackedScene>("res://Scenes/Screens/title_screen.tscn");
		worldMap = GD.Load<PackedScene>("res://Scenes/Screens/world_map.tscn");
		levelSelect = GD.Load<PackedScene>("res://Scenes/Screens/level_select.tscn");
        gameplay = GD.Load<PackedScene>("res://Scenes/Screens/gameplay.tscn");
        levelResults = GD.Load<PackedScene>("res://Scenes/Screens/level_results.tscn");
		//index levels
		IndexLevels();
		LoadSave();
        //go to title
        currentScene = titleScreen.Instantiate();
        instance.AddChild(currentScene);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static void SaveGame()
	{
		GD.Print("Saving Game");
        ResourceSaver.Save(_saveFile, SAVE_PATH);
    }

	public static void QuitGame()
	{
		SaveGame();
		GD.Print("Goodbye!");
        instance.GetTree().Quit();
    }

	public static void TitleScreen()
	{
		currentScene.QueueFree();
        currentScene = null;
        currentScene = titleScreen.Instantiate();
        instance.AddChild(currentScene);
    }

	public void ToWorldMap()
	{
		WorldMap();
        GameOverlay.ScreenFader.UnfadeScreen("screen_fade_quick");
    }

	public static void WorldMap()
	{
        currentScene.QueueFree();
        currentScene = null;
        currentScene = worldMap.Instantiate();
        instance.AddChild(currentScene);
    }

	public static void LevelSelect(int world)
	{
        currentScene.QueueFree();
        currentScene = null;
        currentScene = levelSelect.Instantiate();
        instance.AddChild(currentScene);
		((LevelSelect)currentScene).worldNum = world;
        ((LevelSelect)currentScene).LoadLevels();
		GameOverlay.ScreenFader.UnfadeScreen("screen_wipe_grid");
    }

	public static void PlayNextLevel() //TODO: add level identifier
	{
		currentLevel += 1;
		PlayLevel(LevelIndex.GetLevel(SaveFile.currentWorld, currentLevel));
    }

	public static void ReplayLevel()
	{
        PlayLevel(LevelIndex.GetLevel(SaveFile.currentWorld, currentLevel));
    }

    public static void PlayLevel(IndexLevel level) //TODO: add level identifier
    {
        GD.Print("playing " + level.levelPath);
		if(!Godot.FileAccess.FileExists(level.levelPath))
		{
			GD.Print("Level does not exist oops");
			return;
		}
        //setup gameplay
        currentScene.QueueFree();
        currentScene = null;
        currentScene = gameplay.Instantiate();
        instance.AddChild(currentScene);
        ((GameplayManager)currentScene).LoadLevel(level);
		currentLevel = level.levelNumber;
    }

    public static void LevelResults(TimeSpan time, int gems, Level level)
	{
		currentScene.QueueFree();
		currentScene = null;
		currentScene = levelResults.Instantiate();
		instance.AddChild(currentScene);
		GameOverlay.ScreenFader.UnfadeScreen("screen_fade_quick", true);
		currentScene.GetNode<RichTextLabel>("%InfoText").Text = "[center]" + level.levelName;
        currentScene.GetNode<RichTextLabel>("%TimeText").Text = "[right]" + time.ToString(@"m\:ss\.fff");
		currentScene.GetNode<RichTextLabel>("%GemsText").Text = "[right]" + gems + "/" + level.gems;
        currentScene.GetNode<MenuOption>("%Options/Option1").OnSelect += PlayNextLevel;
        currentScene.GetNode<MenuOption>("%Options/Option2").OnSelect += ReplayLevel;
        AudioSystem.PlaySFX("Select");
    }

	public static string GetLevelPath(int world, int level)
	{
		return LevelIndex.GetLevel(world, level).levelPath;
	}

	public static void IndexLevels()
	{
		_levelIndex = new LevelIndex();
		_levelIndex.IndexLevels();
	}

	public static void LoadSave()
	{
        if (!Godot.FileAccess.FileExists(SAVE_PATH))
        {
			_saveFile = null;
			return;
        }
		else
		{
			GD.Print("Save file found. loading save");
            _saveFile = ResourceLoader.Load<SaveFile>(SAVE_PATH);
			//TEST
			int wi = 0;
			foreach(WorldScore world in _saveFile.scores)
			{
				int li = 0;
				foreach(LevelScore score in  world.levelScores)
				{
					GD.Print("We have a score at " + wi + "-" + li);
					li += 1;
				}
				wi += 1;
			}
        }
	}

	public static void CreateSaveFile()
	{
		GD.Print("No save file found. creating new save file");
		//create new object
		_saveFile = new SaveFile();
		//tally all levels and worlds
		string path = "res://Scenes/Levels";
        using var dir = DirAccess.Open(path);
        if (dir != null)
        {
            foreach (string folder in dir.GetDirectories())
            {
				if(folder.Contains("World"))
				{
					GD.Print("Logging new world " + folder);
					_saveFile.AddWorld();
				}
            }
        }
		_saveFile.scores[0].unlockIndex = 0;
		ResourceSaver.Save(_saveFile, SAVE_PATH);
    }


}
