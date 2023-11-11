using Godot;
using System;
using System.Diagnostics;

public partial class GameplayManager : Node2D
{
	[Export]
	public string levelsPath;
	[Export]
	public string targetLevel;
	[Export]
	public string backgroundsPath;
	private int gemCount;
	private int maxGems;
	private Stopwatch timer;
	private Node loadedBackground;
	private Level loadedLevel;
	private Player player;
	private CameraFollow camera;
	private Node hud;
	private RichTextLabel timerText;
	private RichTextLabel crystalText;
	private RichTextLabel titleText;
	private RichTextLabel worldText;

	private bool loaded;
	private bool initialized;
	private bool playing;
	private bool paused;

	private Timer loadTime;

	//scene references
    private PackedScene loadedLevelScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		camera = this.FindChild("Camera") as CameraFollow;
		player = this.FindChild("Player") as Player;
		hud = this.FindChild("GameplayHUD");
		timerText = hud.GetNode("%Timer") as RichTextLabel;
		crystalText = hud.GetNode("%Crystals") as RichTextLabel;
		titleText = hud.GetNode<RichTextLabel>("%LevelTitle");
		worldText = hud.GetNode<RichTextLabel>("%WorldTitle");
        hud.GetNode("%GoContainer").GetNode<Timer>("GoTimer").Timeout += GoTimeOut;
        LoadLevel();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timerText.Text = timer.Elapsed.ToString(@"m\:ss\.fff");
        crystalText.Text = gemCount + "/" + maxGems;
    }

    public override void _Input(InputEvent @event)
    {
		if(@event.IsActionPressed("Btn_A"))
		{
			hud.GetNode<AnimationPlayer>("%MiniSunAnim").Play("flash");
		}
		if(@event.IsActionReleased("Btn_A"))
		{
			hud.GetNode<AnimationPlayer>("%MiniSunAnim").Stop();
        }
        if (@event.IsActionPressed("Btn_B"))
        {
            hud.GetNode<AnimationPlayer>("%MiniMoonAnim").Play("flash");
        }
        if (@event.IsActionReleased("Btn_B"))
        {
            hud.GetNode<AnimationPlayer>("%MiniMoonAnim").Stop();
        }
        if (@event.IsActionPressed("Btn_A") || @event.IsActionPressed("Btn_B"))
		{
			if(!initialized && !playing && loaded)
			{
				GetReady();
				this.GetTree().Root.SetInputAsHandled();
				player.InputEnabled = true;
				return;
			}
			if(initialized && !playing)
			{
				LetsGo();
			}
		}
        if (@event.IsActionPressed("Btn_Start"))
        {
            ReloadLevel();
        }
    }

	public void LevelImport()
	{
        //set floor texture
        ((TileSetAtlasSource)loadedLevel.GetNode<TileMap>("Floor").TileSet.GetSource(0)).Texture = loadedLevel.floorTexture;
        //set wall texture
        ((TileSetAtlasSource)loadedLevel.GetNode<TileMap>("Wall").TileSet.GetSource(0)).Texture = loadedLevel.wallTexture;
	}

    public void LoadLevel()
	{
		StartLoadTimer();
		player.Position = new Vector2(-9999, 9999);
        if (loadedLevel != null) loadedLevel.QueueFree();
        loadedLevelScene = GD.Load<PackedScene>(levelsPath + "/" + targetLevel + ".tscn");
		loadedLevel = null;
		loadedLevel = loadedLevelScene.Instantiate() as Level;
		this.AddChild(loadedLevel);
		SetBackground();
        gemCount = 0;
        timer = new Stopwatch();
		titleText.Text = "[center]" + loadedLevel.levelName;
		worldText.Text = "[center]World " + (loadedLevel.world == 0 ? "?" : loadedLevel.world) + " - Level " + (loadedLevel.number == 0 ? "?" : loadedLevel.number);
		InitializeLevel();
    }

	public void ReloadLevel()
	{
        StartLoadTimer();
        player.Position = new Vector2(-9999, 9999);
        if (loadedLevel == null) return;
		loadedLevel.QueueFree();
        loadedLevel = null;
        loadedLevel = loadedLevelScene.Instantiate() as Level;
        this.AddChild(loadedLevel);
		gemCount = 0;
		timer = new Stopwatch();
		GetReady();
    }

	public void SpawnPlayer()
	{
        player.Reset();
        player.GlobalPosition = ((Node2D)loadedLevel.GetNode("%PlayerStart")).GlobalPosition;
		player.RotationSpeed = 2.5f * (loadedLevel.initialPlayerSpin ? 1 : -1);
        camera.SetTarget(player);
		camera.Position = player.Position;
    }

	public void SetBackground()
	{
		if(loadedBackground != null) loadedBackground.QueueFree();
		loadedBackground = null;
		PackedScene targetBG = GD.Load<PackedScene>(backgroundsPath + "/" + loadedLevel.background + ".tscn");
		loadedBackground = targetBG.Instantiate();
		this.AddChild(loadedBackground);
	}

	public void PostLoad()
	{
		//set loaded status
        loaded = true;
		LevelImport();
        SpawnPlayer();
        //scan gems in level
        maxGems = 0;
		foreach (Node gem in loadedLevel.GetTree().GetNodesInGroup("Crystal"))
		{
			maxGems += 1;
			gem.GetChild<CrystalCollectible>(0).OnCollect += CrystalCollected;
		}
		GD.Print("level loaded");
    }

	public void InitializeLevel()
	{
		hud.GetNode<CanvasItem>("%ReadyContainer").Visible = false;
        hud.GetNode<CanvasItem>("%GoContainer").Visible = false;
		hud.GetNode<CanvasItem>("%TitleContainer").Visible = true;
		player.InputEnabled = false;
		initialized = false;
		playing = false;
		paused = false;
    }

	public void GetReady()
	{
        hud.GetNode<CanvasItem>("%ReadyContainer").Visible = true;
        hud.GetNode<CanvasItem>("%GoContainer").Visible = false;
        hud.GetNode<CanvasItem>("%TitleContainer").Visible = false;
		AudioSystem.PlaySFX("Select");
		initialized = true;
		playing = false;
		paused = false;
    }

	public void LetsGo()
	{
        hud.GetNode<CanvasItem>("%ReadyContainer").Visible = false;
        hud.GetNode<CanvasItem>("%GoContainer").Visible = true;
        hud.GetNode<CanvasItem>("%TitleContainer").Visible = false;
		initialized = true;
		playing = true;
		paused = false;
        timer.Start();
        hud.GetNode("%GoContainer").GetNode<Timer>("GoTimer").Start();
		AudioSystem.PlaySFX("Go!");
    }

	private void GoTimeOut()
	{
        hud.GetNode<CanvasItem>("%GoContainer").Visible = false;

    }

	private void StartLoadTimer()
	{
		if (loadTime != null) loadTime.QueueFree();
        loadTime = new Timer();
        this.AddChild(loadTime);
        loadTime.WaitTime = 0.01;
        loadTime.OneShot = true;
        loadTime.Timeout += PostLoad;
        loadTime.Start();
		loaded = false;
    }

    public void CrystalCollected()
	{
		gemCount += 1;
	}
}
