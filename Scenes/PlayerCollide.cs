using Godot;
using System;

public partial class PlayerCollide : AnimatableBody2D
{
    [Signal]
    public delegate void OnCollideEventHandler(KinematicCollision2D coll, bool moon);

    [Export]
    private bool sun;

    private Player player;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = this.Owner as Player;
    }

    public override void _Process(double delta)
    {
        KinematicCollision2D coll = MoveAndCollide(player.GetPlanetMotion(sun) * (float)delta * 4, true);

        if (coll != null)
        {
            EmitSignal(SignalName.OnCollide, coll, sun);
        }
    }
}
