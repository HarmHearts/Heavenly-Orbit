using Godot;
using System;

public partial class PlanetSparkles : GpuParticles2D
{
	[Export]
	private float speedInfluence;
	[Export]
	private float baseSpeed;
	[Export]
	private float speedVariance;
	[Export]
	private bool bodyToggle;
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
		particle.InitialVelocityMax = baseSpeed + speedVariance + (player.MoveSpeedNormalized * speedInfluence);
        particle.InitialVelocityMin = baseSpeed + (player.MoveSpeedNormalized * speedInfluence);
		this.Emitting = (!player.Locked || player.LockedBody != bodyToggle);
    }
}
