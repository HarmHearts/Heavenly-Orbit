using Godot;
using System;

public partial class DebugText : RichTextLabel
{
	[Export]
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//player speed
		this.Text = "speed: " + player.MoveSpeed + "\n";
		//lock status
		this.Text += player.Locked ? (player.LockedBody ? "Sun Locked" : "Moon Locked") : ("Unlocked");
        this.Text += "\n";
	}
}
