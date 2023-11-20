using Godot;
using System;

public partial class BGScroll : ParallaxBackground
{
    [Export]
    public Vector2 scroll;
    public override void _Process(double delta)
    {
        this.ScrollBaseOffset += scroll * (float)delta;
    }
}
