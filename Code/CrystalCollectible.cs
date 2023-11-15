using Godot;
using System;

public partial class CrystalCollectible : Area2D
{
    private PackedScene collectSparkle = GD.Load<PackedScene>("res://Scenes/Constructs/collect_sparkle.tscn");

    [Signal]
    public delegate void OnCollectEventHandler();

    public override void _Ready()
    {
        OnCollect += GetNode<GameplayManager>("/root/Gameplay").CrystalCollected;
    }

    private void OnBodyEntered(PhysicsBody2D body)
    {
        AudioSystem.PlaySFX("Collect");
        Node2D effect = collectSparkle.Instantiate() as Node2D;
        this.GetParent().AddSibling(effect);
        effect.GlobalPosition = this.GlobalPosition;
        EmitSignal(SignalName.OnCollect);
        this.GetParent().QueueFree();
    }
}
