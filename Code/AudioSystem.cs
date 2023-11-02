using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class AudioSystem : Node
{
	private static AudioStreamPlayer sfxStream;
	private static Dictionary<StringName, SoundEffect> soundEffects;
	private static Dictionary<StringName, AudioStreamPlayer> loopingEffects;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		sfxStream = this.FindChild("SFXPlayer", true) as AudioStreamPlayer;
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
				SoundEffect newEffect = GD.Load<SoundEffect>(path + file);
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
            sfxStream.GetParent().AddChild(newPlayer);
            newPlayer.Play();
			loopingEffects.Add(name, newPlayer);
        }
		//if one shot sound
		else
		{
            sfxStream.Stream = target.GetAudio();
            sfxStream.Play();
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
