using Godot;
using System;

public partial class GameOverlay : CanvasLayer
{
	public static ScreenFader ScreenFader { get; set; }
	public static ShaderMaterial PaletteShader { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenFader = this.GetNode<ScreenFader>("%ScreenFader");
		PaletteShader = this.GetNode<CanvasItem>("%PaletteFilter").Material as ShaderMaterial;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
