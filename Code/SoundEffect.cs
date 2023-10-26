using Godot;
using System;
using System.Collections.Generic;

public partial class SoundEffect : Resource
{
    public enum EffectChannel { Pulse1,  Pulse2, Wave, Noise }
    [Export]
    public StringName name;
    [Export]
    public Godot.Collections.Array<AudioStream> audioStreams;
    [Export]
    public bool looping = false;
    public bool playing = false;
    [Export]
    public EffectChannel channel;
    private int lastRandomIndex;

    public AudioStream GetAudio()
    {
        if(audioStreams.Count == 0) return null;
        //return single audio
        if(audioStreams.Count == 1) return audioStreams[0];
        //get random alternate
        int newRandom = GD.RandRange(0, audioStreams.Count - 1);
        if(newRandom == lastRandomIndex) newRandom = (newRandom + 1) % (audioStreams.Count - 1);
        lastRandomIndex = newRandom;
        return audioStreams[newRandom];
    }
}
