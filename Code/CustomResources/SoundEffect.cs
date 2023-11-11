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
    public bool looping;

    public AudioStream GetAudio()
    {
        int index = (int)(GD.Randi() % audioList.Count);
        return audioList[index];
    }

    public SoundEffect() : this($"Default", null, AudioChannel.Wave, false) { }

    public SoundEffect(StringName name, Array<AudioStream> audioList, AudioChannel channel, bool looping)
    {
        this.name = name;
        this.audioList = audioList;
        this.channel = channel;
        this.looping = looping;
    }
}
