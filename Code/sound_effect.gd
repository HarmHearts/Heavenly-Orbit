extends Resource
class_name SoundEffect

enum effect_channel {PULSE1, PULSE2, NOISE, WAVE}
@export var name : StringName
@export var audio_streams := Array[AudioStream]
@export var looping : bool
