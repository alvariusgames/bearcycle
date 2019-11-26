using Godot;
using System;
using System.Collections.Generic;

public static class SoundHandler {
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private static Dictionary<string, AudioStream> audioStreams = new Dictionary<string, AudioStream>();
    private static List<IAudioStreamPlayer> activeAudioStreamPlayers = new List<IAudioStreamPlayer>();
    private static List<IAudioStreamPlayer> activeAudioSamplePlayers = new List<IAudioStreamPlayer>();
    private static void GarbageCollect(){
        foreach(var audioStreamPlayer in activeAudioStreamPlayers.ToArray()){
            try{
                if(audioStreamPlayer.IsPlaying() == false){
                  activeAudioStreamPlayers.Remove(audioStreamPlayer);
                  if(audioStreamPlayer.GetParent() != null){
                      audioStreamPlayer.GetParent().RemoveChild(audioStreamPlayer.SelfNode);}}
            } catch(Exception e){
                activeAudioSamplePlayers.Remove(audioStreamPlayer);}}
        foreach(var audioSamplePlayer in activeAudioSamplePlayers.ToArray()){
            try{
            if(audioSamplePlayer?.IsPlaying() == false){
                activeAudioStreamPlayers.Remove(audioSamplePlayer);
                if(audioSamplePlayer.GetParent() != null){
                    audioSamplePlayer.GetParent().RemoveChild(audioSamplePlayer.SelfNode);}}
            } catch(Exception e){
               activeAudioSamplePlayers.Remove(audioSamplePlayer);}}}

    private static float SampleVolume = 10f;
    private const float MAX_SAMPLE_VOLUME = 10f;
    public static float GetSampleVolume(float mulitiplier = 1f){
        var vol = SampleVolume * mulitiplier;
        if(vol > MAX_SAMPLE_VOLUME){
            return MAX_SAMPLE_VOLUME;
        } else {
            return vol;}}
    public static void SetSampleVolume(float SampleVolume){
        SoundHandler.SampleVolume = SampleVolume;
    }


    private static float StreamVolume = 10f;
    private const float MAX_STREAM_VOLUME = 10f;
    public static float GetStreamVolume(float multiplier = 1f){
        var vol = StreamVolume * multiplier;
        if(vol > MAX_STREAM_VOLUME){
            return MAX_STREAM_VOLUME;
        } else {
            return vol;}} 
    public static void SetStreamVolume(float StreamVolume){
        SoundHandler.StreamVolume = StreamVolume;
    }
    public static void TempFadeAllVolume(float multiplier = 0.99f){
        foreach(var audioStreamPlayer in activeAudioStreamPlayers){
            try{
                audioStreamPlayer.VolumeDb *= multiplier;}
            catch{}}

        foreach(var audioSamplePlayer in activeAudioSamplePlayers){
            try{audioSamplePlayer.VolumeDb *= multiplier;}
            catch{}}}

    public static void EndTempFadeAllVolume(){
        foreach(var audioStreamPlayer in activeAudioStreamPlayers){
            try{audioStreamPlayer.VolumeDb = GetStreamVolume();}
            catch{}}

        foreach(var audioSamplePlayer in activeAudioSamplePlayers){
            try{audioSamplePlayer.VolumeDb = GetSampleVolume();}
            catch{}}}


    public static void PlayStream<StreamPlayerType>(Node caller, 
                                                    String[] StreamPaths, 
                                                    float VolumeMultiplier = 1f,
                                                    Boolean Loop = false,
                                                    Boolean SkipIfAlreadyPlaying = false) where StreamPlayerType : IAudioStreamPlayer, new() {
        GarbageCollect();
        var rnd = new Random();
        var index = rnd.Next(StreamPaths.Length);
        var StreamPath = StreamPaths[index];

        AudioStreamOGGVorbis audioStream = default(AudioStreamOGGVorbis);         
        if(audioStreams.ContainsKey(StreamPath)){
            audioStream = (AudioStreamOGGVorbis)audioStreams[StreamPath];
        } else {
            audioStream = (AudioStreamOGGVorbis)GD.Load(StreamPath);
            audioStreams[StreamPath] = audioStream;}

        if(SkipIfAlreadyPlaying){
            foreach(var child in caller.GetChildren()){
                if(child is IAudioStreamPlayer){
                    var player = child as IAudioStreamPlayer;
                    if(player.Stream == audioStream){
                        GD.Print("Already playing, skipping...");
                        return;}}}}

        audioStream.Loop = Loop;

        var audioStreamPlayer = new StreamPlayerType();
        audioStreamPlayer.Stream = audioStream;
        audioStreamPlayer.PauseMode = Node.PauseModeEnum.Process;
        caller.AddChild(audioStreamPlayer.SelfNode);
        audioStreamPlayer.VolumeDb = GetStreamVolume(VolumeMultiplier);
        audioStreamPlayer.Play();
        activeAudioStreamPlayers.Add(audioStreamPlayer);}

    public static void StopStream(Node caller, String StreamPath){
        if(audioStreams.ContainsKey(StreamPath)){
            var audioStream = audioStreams[StreamPath];
            foreach(var player in activeAudioStreamPlayers){
                try{
                    if(player.Stream == audioStream){
                        player.Stop();}}
                catch(Exception e){}}}
        GarbageCollect();
    }
    public static void StopAllStream(){
        foreach(var player in activeAudioStreamPlayers){
            try{
                player.Stop();}
            catch{}}
        GarbageCollect();}

    public static void PauseAllStream(){
        foreach(var player in activeAudioStreamPlayers){
            try{
                player.StreamPaused = true;}
            catch{}}
        GarbageCollect();}

    public static void UnpauseAllStream(){
        foreach(var player in activeAudioStreamPlayers){
            try{
                player.StreamPaused = false;}
            catch{}}
        GarbageCollect();}

 


    public static void PlaySample<StreamPlayerType>(Node caller, 
                                                    String[] StreamPaths, 
                                                    float VolumeMultiplier = 1f,
                                                    Boolean Loop = false,
                                                    Boolean SkipIfAlreadyPlaying = false,
                                                    float PitchScale = 1.0f) where StreamPlayerType : IAudioStreamPlayer, new() {
        GarbageCollect();
        var rnd = new Random();
        var index = rnd.Next(StreamPaths.Length);
        var StreamPath = StreamPaths[index];

        AudioStreamSample audioStream = default(AudioStreamSample);
        if(audioStreams.ContainsKey(StreamPath)){
            audioStream = (AudioStreamSample)audioStreams[StreamPath];
        } else {
            audioStream = (AudioStreamSample)GD.Load(StreamPath);
            audioStreams[StreamPath] = audioStream;}

        if(SkipIfAlreadyPlaying){
            foreach(var child in caller.GetChildren()){
                if(child is IAudioStreamPlayer){
                    var player = child as IAudioStreamPlayer;
                    player.PitchScale = PitchScale;
                    player.VolumeDb = GetSampleVolume(VolumeMultiplier);
                    if(player.Stream == audioStream){
                        return;}}}}

        if(Loop){
            audioStream.LoopMode = AudioStreamSample.LoopModeEnum.Forward;
            audioStream.LoopBegin = 0;
            audioStream.LoopEnd = 100000;}

        var audioStreamPlayer = new StreamPlayerType();
        audioStreamPlayer.Stream = audioStream;
        audioStreamPlayer.PauseMode = Node.PauseModeEnum.Process;
        audioStreamPlayer.PitchScale = PitchScale;
        caller.AddChild(audioStreamPlayer.SelfNode);
        audioStreamPlayer.VolumeDb = GetSampleVolume(VolumeMultiplier);
        audioStreamPlayer.Play();
        activeAudioSamplePlayers.Add(audioStreamPlayer);}

    public static void StopSample(Node caller, String StreamPath){
        if(audioStreams.ContainsKey(StreamPath)){
            var audioStream = audioStreams[StreamPath];
            foreach(var player in activeAudioSamplePlayers){
                if(player.Stream == audioStream){
                    player.Stop();
                }}}
        GarbageCollect();}

    public static void StopAllSample(){
        foreach(var player in activeAudioSamplePlayers){
            player.Stop();}
        GarbageCollect();}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
