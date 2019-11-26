using Godot;
using System;

public interface IAudioStreamPlayer : IDisposable{
   float VolumeDb { get; set;}
    Node.PauseModeEnum PauseMode { get; set; }
    AudioStream Stream { get; set; }
    void Play(float fromPosition = 0f);
    void Stop();
    Boolean IsPlaying();
    Node GetParent();
    Boolean StreamPaused { get; set;}
    Node SelfNode { get; } 
    //TODO: fix this horrible workaround and get better at static type programming
    float PitchScale { get; set; }
}