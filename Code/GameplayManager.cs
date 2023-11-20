using Godot;
using System.Diagnostics;
using System.Linq;
using System;

public partial class GameplayManager : Node2D
{
	[Export]
	public string levelsPath;
	[Export]
	public string targetLevel;
	[Export]
	public string backgroundsPath;
    public IndexLevel level;
	private int gemCount;
	private Stopwatch timer;
	private Node loadedBackground;
	public Level loadedLevel;
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
    private bool busy;

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
        //LoadLevel();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timerText.Text = timer.Elapsed.ToString(@"m\:ss\.fff");
        crystalText.Text = gemCount + "/" + loadedLevel.gems;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Btn_Start"))
        {
            if (busy) return;
            PauseGame();
        }
        if (@event.IsActionReleased("Btn_A"))
        {
            hud.GetNode<AnimationPlayer>("%MiniSunAnim").Stop();
            if (busy && player.Locked) player.ForceUnlock();
        }
        if (@event.IsActionReleased("Btn_B"))
        {
            hud.GetNode<AnimationPlayer>("%MiniMoonAnim").Stop();
            if (busy && player.Locked) player.ForceUnlock();
        }
        if (busy) return;
        if (@event.IsActionPressed("Btn_A") || @event.IsActionPressed("Btn_B"))
        {
            if (!initialized && !playing && loaded)
            {
                GetReady();
                this.GetTree().Root.SetInputAsHandled();
                return;
            }
            if (initialized && !playing)
            {
                LetsGo();
            }
        }
        if (@event.IsActionPressed("Btn_A") && playing)
		{
			hud.GetNode<AnimationPlayer>("%MiniSunAnim").Play("flash");
		}
        if (@event.IsActionPressed("Btn_B") && playing)
        {
            hud.GetNode<AnimationPlayer>("%MiniMoonAnim").Play("flash");
        }
        if (@event.IsActionPressed("Btn_Select"))
        {
            player.ForceDeath();
        }
    }

    public void LoadLevel(string path)
	{
        targetLevel = path;
		player.Position = new Vector2(-9999, 9999);
        if (loadedLevel != null) loadedLevel.QueueFree();
        loadedLevelScene = GD.Load<PackedScene>(path);
		loadedLevel = null;
		loadedLevel = loadedLevelScene.Instantiate() as Level;
		this.AddChild(loadedLevel);
        loadedLevel.Position = Vector2.Zero;
		SetBackground();
        gemCount = 0;
        timer = new Stopwatch();
		titleText.Text = "[center]" + loadedLevel.levelName;
		worldText.Text = "[center]World " + (loadedLevel.world == 0 ? "?" : loadedLevel.world) + " - Level " + (loadedLevel.number == 0 ? "?" : loadedLevel.number);
        GetNode("PauseScreen").GetNode<RichTextLabel>("%InfoText").Text = "[center]-World " + (loadedLevel.world == 0 ? "?" : loadedLevel.world) + " - Level " + (loadedLevel.number == 0 ? "?" : loadedLevel.number) + "-\n" + loadedLevel.levelName;
        LevelImport();
        SpawnPlayer();
        FinishLoad();
        InitializeLevel();
    }

    public void LoadLevel(IndexLevel level)
    {
        this.level = level;
        LoadLevel(level.levelPath);
    }

	public void ReloadLevel()
	{
        //StartLoadTimer();
        player.Position = new Vector2(-9999, 9999);
        if (loadedLevel == null) return;
		loadedLevel.QueueFree();
        loadedLevel = null;
        loadedLevel = loadedLevelScene.Instantiate() as Level;
        this.AddChild(loadedLevel);
        loadedLevel.Position = Vector2.Zero;
        gemCount = 0;
		timer = new Stopwatch();
		LevelImport();
        SpawnPlayer();
        FinishLoad();
        GetReady();
    }

    public void FinishLoad()
    {
        loaded = true;
        busy = false;
        hud.GetNode<CanvasItem>("%FinishContainer").Visible = false;
        GameOverlay.ScreenFader.UnfadeScreen("screen_wipe_grid", true);
        GD.Print("level loaded");
    }

    public void LevelImport()
    {
        //set floor texture
        ((TileSetAtlasSource)loadedLevel.GetNode<TileMap>("Floor").TileSet.GetSource(0)).Texture = loadedLevel.floorTexture;
        //set wall texture
        ((TileSetAtlasSource)loadedLevel.GetNode<TileMap>("Wall").TileSet.GetSource(0)).Texture = loadedLevel.wallTexture;
        //connect to finish line
        loadedLevel.GetNode<FinishLine>("%FinishLine").OnWin += Win;
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

	public void InitializeLevel()
	{
        hud.GetNode<CanvasItem>("%ReadyContainer").Visible = false;
        hud.GetNode<CanvasItem>("%GoContainer").Visible = false;
		hud.GetNode<CanvasItem>("%TitleContainer").Visible = true;
		player.InputEnabled = false;
		initialized = false;
		playing = false;
		busy = false;
    }

	public void GetReady()
	{
        hud.GetNode<CanvasItem>("%ReadyContainer").Visible = true;
        hud.GetNode<CanvasItem>("%GoContainer").Visible = false;
        hud.GetNode<CanvasItem>("%TitleContainer").Visible = false;
		AudioSystem.PlaySFX("Select");
        player.InputEnabled = true;
        initialized = true;
		playing = false;
		busy = false;
    }

	public void LetsGo()
	{
        hud.GetNode<CanvasItem>("%ReadyContainer").Visible = false;
        hud.GetNode<CanvasItem>("%GoContainer").Visible = true;
        hud.GetNode<CanvasItem>("%TitleContainer").Visible = false;
		initialized = true;
		playing = true;
		busy = false;
        timer.Start();
        hud.GetNode("%GoContainer").GetNode<Timer>("GoTimer").Start();
		AudioSystem.PlaySFX("Go!");
    }

	private void GoTimeOut()
	{
        hud.GetNode<CanvasItem>("%GoContainer").Visible = false;

    }

	private void ResetTimer(float time)
	{
        busy = true;
		if (loadTime != null) loadTime.QueueFree();
        loadTime = new Timer();
        this.AddChild(loadTime);
        loadTime.WaitTime = time;
        loadTime.OneShot = true;
        loadTime.Timeout += DeathReset;
        loadTime.Start();
		player.InputEnabled = false;
    }

    public void CrystalCollected()
	{
		gemCount += 1;
	}

	public void DeathReset()
	{
        GameOverlay.ScreenFader.FadeScreen("screen_wipe_grid", new Callable(this, MethodName.ReloadLevel), true);
    }

    public void Win()
    {
        //stop timer
        timer.Stop();
        //change status
        busy = true;
        //swap in victory player
        PackedScene victoryPlayer = GD.Load<PackedScene>("res://Scenes/Constructs/victory_player.tscn");
        VictoryPlayer newGuy = victoryPlayer.Instantiate() as VictoryPlayer;
        newGuy.CopyPlayer(player.MoveDirection, player.MoveSpeed, player.BodyDistance, player.Rotation, player.RotationSpeed);
        this.AddChild(newGuy);
        player.ForceUnlock();
        newGuy.Position = player.Position;
        camera.target = newGuy;
        player.Reset();
        player.Position = new Vector2(-9999, 9999);
        player.InputEnabled = false;
        //trigger finish text
        hud.GetNode<CanvasItem>("%FinishContainer").Visible = true;
        //play sound
        AudioSystem.PlaySFX("Victory"); //TODO: replace with real win sound
        //wait for win animation to end
        WinTimer(3.8f);
    }

    private void WinTimer(float time)
    {
        if (loadTime != null) loadTime.QueueFree();
        loadTime = new Timer();
        this.AddChild(loadTime);
        loadTime.WaitTime = time;
        loadTime.OneShot = true;
        loadTime.Timeout += FinishWin;
        loadTime.Start();
    }

    private void FinishWin()
    {
        GameManager.SaveFile.AddScore(level.worldNumber, level.levelNumber, new LevelScore(timer.Elapsed.TotalSeconds, gemCount, loadedLevel.gems, 0));
        GameManager.SaveFile.UnlockLevel(level.worldNumber, level.levelNumber + 1);
        GameManager.SaveGame();
        GameOverlay.ScreenFader.FadeScreen("screen_wipe_grid", new Callable(this, MethodName.GoToResults), true);
    }

    private void GoToResults()
    {
        GameManager.LevelResults(timer.Elapsed, gemCount, loadedLevel);
    }

    public void PauseGame()
    {
        try
        {
            GameOverlay.ScreenFader.FadeScreen("screen_fade_quick", new Callable(this, MethodName.PauseScreen));
        }
        catch (TransitionInterruptException e){
            GD.Print(e);
            return;
        }
        AudioSystem.PlaySFX("Pause");
        timer.Stop();
        GetTree().Paused = true;
    }

    public void UnpauseGame()
    {
        if(playing) timer.Start();
        PauseScreen();
    }

    public void PauseScreen()
    {
        GetNode<CanvasLayer>("PauseScreen").Visible = !GetNode<CanvasLayer>("PauseScreen").Visible;
        GameOverlay.ScreenFader.UnfadeScreen("screen_fade_quick", true);
    }
}
