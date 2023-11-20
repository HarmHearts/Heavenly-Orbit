using Godot;
using Godot.Collections;
using System;

public partial class SoundEffect : Resource
{
    public enum AudioChannel { Pulse, Wave, Noise}

    [Export]
    public StringName name;
    [Export]
    public Array<AudioStream> audioList;
    [Export]
    public AudioChannel channel;
    [Export]
    public int priority;
    [Export]
    public bool looping;

    public AudioStream GetAudio()
    {
        int index = (int)(GD.Randi() % audioList.Count);
        return audioList[index];
    }

    public SoundEffect() : this($"Default", null, AudioChannel.Wave, 0, false) { }

    public SoundEffect(StringName name, Array<AudioStream> audioList, AudioChannel channel, int priority, bool looping)
    {
        this.name = name;
        this.audioList = audioList;
        this.channel = channel;
        this.priority = priority;
        this.looping = looping;
    }
}
