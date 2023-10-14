using Godot;
using System;

public partial class LockEffect : GpuParticles2D
{
    public void Play()
    {
        this.Emitting = true;
    }
}
