using Godot;
using System;

public partial class PlanetSparkles : GpuParticles2D
{
	private ParticleProcessMaterial particle;
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		particle = this.ProcessMaterial as ParticleProcessMaterial;
        player = this.GetTree().CurrentScene.FindChild("Player") as Player;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		particle.Direction = new Vector3(player.MoveDirection.X, player.MoveDirection.Y, 0);
	}
}
