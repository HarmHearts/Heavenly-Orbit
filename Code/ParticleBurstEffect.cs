using Godot;
using System;

public partial class ParticleBurstEffect : GpuParticles2D
{
    public void _OnPlay()
    {
        this.Restart();
        foreach(Node sub in this.GetChildren())
        {
            GpuParticles2D system = sub as GpuParticles2D;
            system.Restart();
        }
    }
}
