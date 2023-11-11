using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class AudioSystem : Node
{
	private static AudioStreamPlayer sfxStreamPulse;
    private static AudioStreamPlayer sfxStreamWave;
    private static AudioStreamPlayer sfxStreamNoise;
    private static Dictionary<StringName, SoundEffect> soundEffects;
	private static Dictionary<StringName, AudioStreamPlayer> loopingEffects;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		sfxStreamPulse = this.FindChild("SFXPlayerPulse", true) as AudioStreamPlayer;
        sfxStreamWave = this.FindChild("SFXPlayerWave", true) as AudioStreamPlayer;
        sfxStreamNoise = this.FindChild("SFXPlayerNoise", true) as AudioStreamPlayer;
        LoadSFX("res://Sound/SFX/");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static void LoadSFX(string path)


	{
		soundEffects = new Dictionary<StringName, SoundEffect>();
		using var dir = DirAccess.Open(path);
		if(dir != null)
		{
			foreach(string file in dir.GetFiles())
			{
				GD.Print("Loading sound effect " + path + file);
				SoundEffect newEffect = ResourceLoader.Load<SoundEffect>(path + file.TrimSuffix(".remap"));
				soundEffects.Add(newEffect.name, newEffect);
			}
		}
	}

	public static void PlaySFX(StringName name)
	{
		//get target sound effect
		SoundEffect target = soundEffects[name];
        //if looping sound
        if (target.looping)
        {
			if (loopingEffects.ContainsKey(name)) return;
            AudioStreamPlayer newPlayer = new AudioStreamPlayer();
            newPlayer.Stream = target.GetAudio();
            sfxStreamPulse.GetParent().AddChild(newPlayer);
            newPlayer.Play();
			loopingEffects.Add(name, newPlayer);
        }
		//if one shot sound
		else
		{
			switch(target.channel)
			{
				case SoundEffect.AudioChannel.Pulse:
                    sfxStreamPulse.Stream = target.GetAudio();
                    sfxStreamPulse.Play();
					break;
				case SoundEffect.AudioChannel.Wave:
                    sfxStreamWave.Stream = target.GetAudio();
                    sfxStreamWave.Play();
                    break;
                case SoundEffect.AudioChannel.Noise:
                    sfxStreamNoise.Stream = target.GetAudio();
                    sfxStreamNoise.Play();
                    break;
                default:
					break;
			}
        }
    }

	public static void StopSFX(StringName name)
	{
		AudioStreamPlayer target = loopingEffects[name];
		if(target != null)
		{
			target.Stop();
			target.QueueFree();
			loopingEffects.Remove(name);
		}
	}
}
