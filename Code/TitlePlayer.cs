using Godot;
using System;

public partial class TitlePlayer : Player
{
    public override void _Ready()
    {
        this.alive = false;
        this.InputEnabled = false;
    }
    public override void _Process(double delta)
    {
        this.Rotation += RotationSpeed * (float)delta;
    }
}
