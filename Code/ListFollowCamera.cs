using Godot;
using System;

public partial class ListFollowCamera : Camera2D
{
    [Export]
    public Control target;
    [Export]
    public float speed;
    [Export]
    public float smooth;
    [Export]
    public float verticalRange;

    public override void _Process(double delta)
    {
        if(Mathf.Abs(target.GlobalPosition.Y - this.GlobalPosition.Y) > verticalRange)
        {
            this.Position = this.Position.MoveToward(target.GlobalPosition, speed * (float)delta);
            this.Position = this.Position.Lerp(target.GlobalPosition, smooth * (float)delta);
        }
    }
}
