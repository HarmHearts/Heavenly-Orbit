using Godot;
using System;

public partial class ParticleBurstEffect : GpuParticles2D
{
    [Export]
    private bool playOnReady = false;
    [Export]
    private bool killOnFinish = false;

    private bool started = false;

    public override void _Ready()
    {
        if(playOnReady) _OnPlay();
    }

    public override void _Process(double delta)
    {
        if(killOnFinish && started)
        {
            bool die = false;
            foreach (Node sub in this.GetChildren())
            {
                GpuParticles2D system = sub as GpuParticles2D;
                if(!system.Emitting) die = true;
            }
            if (die) this.QueueFree();
        }
    }

    public void _OnPlay()
    {
        this.Restart();
        started = true;
        foreach(Node sub in this.GetChildren())
        {
            GpuParticles2D system = sub as GpuParticles2D;
            system.Restart();
        }
    }
}
