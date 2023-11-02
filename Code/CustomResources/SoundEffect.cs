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
}
