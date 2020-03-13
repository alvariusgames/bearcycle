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

    private static float SampleVolumeLinearUnits = DEFAULT_SAMPLE_VOLUME_LINEAR;
    public const float DEFAULT_SAMPLE_VOLUME_LINEAR = 0.7f;
    public static float GetSampleVolumeLinearUnits(float mulitiplier = 1f){
        return SoundHandler.SampleVolumeLinearUnits * mulitiplier;}

    public static float GetSampleVolumeDbUnits(float mulitiplier = 1f){
        return GD.Linear2Db(SoundHandler.GetSampleVolumeLinearUnits(mulitiplier));}

    public static void SetSampleVolumeLinearUnits(float sampleVolumeLinearUnits){
        SoundHandler.SampleVolumeLinearUnits = sampleVolumeLinearUnits;}

    private static float StreamVolumeLinearUnits = DEFAULT_STREAM_VOLUME_LINEAR;
    public const float DEFAULT_STREAM_VOLUME_LINEAR = 0.7f;
    public static float GetStreamVolumeLinearUnits(float multiplier = 1f){
        return SoundHandler.StreamVolumeLinearUnits * multiplier;}
    public static float GetStreamVolumeDbUnits(float mulitiplier = 1f){
        return GD.Linear2Db(SoundHandler.GetStreamVolumeLinearUnits(mulitiplier));}
    public static void SetStreamVolumeLinearUnits(float streamVolumeLinearUnits,
                                                  Boolean updateExistingStreams = false){
        SoundHandler.StreamVolumeLinearUnits = streamVolumeLinearUnits;
        var streamVolumeDbUnits = SoundHandler.GetStreamVolumeDbUnits();
        if(updateExistingStreams){
            foreach(var audioStreamPlayer in activeAudioStreamPlayers){
                try{
                    audioStreamPlayer.VolumeDb = streamVolumeDbUnits;}
                catch{}
            }
        }
    }
    public static void TempSetAllStreamAndSample(float linearUnits){
        foreach(var audioStreamPlayer in activeAudioStreamPlayers){
            try{
                audioStreamPlayer.VolumeDb = GD.Linear2Db(SoundHandler.GetStreamVolumeLinearUnits() * linearUnits);}
            catch{}}

        foreach(var audioSamplePlayer in activeAudioSamplePlayers){
            try{
                audioSamplePlayer.VolumeDb = GD.Linear2Db(SoundHandler.GetSampleVolumeLinearUnits() * linearUnits);}
            catch{}}}

    public static void EndTempSetAllStreamAndSample(){
        foreach(var audioStreamPlayer in activeAudioStreamPlayers){
            try{audioStreamPlayer.VolumeDb = GetStreamVolumeDbUnits();}
            catch{}}

        foreach(var audioSamplePlayer in activeAudioSamplePlayers){
            try{audioSamplePlayer.VolumeDb = GetSampleVolumeDbUnits();}
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
                        return;}}}}

        audioStream.Loop = Loop;

        var audioStreamPlayer = new StreamPlayerType();
        audioStreamPlayer.Stream = audioStream;
        audioStreamPlayer.PauseMode = Node.PauseModeEnum.Process;
        caller.AddChild(audioStreamPlayer.SelfNode);
        audioStreamPlayer.VolumeDb = GetStreamVolumeDbUnits(VolumeMultiplier);
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

    public static void PauseAllSample(){
        foreach(var player in activeAudioSamplePlayers){
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
    public static void UnpauseAllSample(){
        foreach(var player in activeAudioSamplePlayers){
            try{
                player.StreamPaused = false;}
            catch{}}
        GarbageCollect();}

    public static void PlaySample<StreamPlayerType>(Node caller,
                                                    String StreamPath,
                                                    float VolumeMultiplier = 1f,
                                                    Boolean Loop = false,
                                                    Boolean SkipIfAlreadyPlaying = false,
                                                    float PitchScale = 1.0f,
                                                    Boolean PauseAllOtherSoundWhilePlaying = false) where StreamPlayerType : IAudioStreamPlayer, new(){
        SoundHandler.PlaySample<StreamPlayerType>(caller,
                                                  new String[]{StreamPath},
                                                  VolumeMultiplier,
                                                  Loop,
                                                  SkipIfAlreadyPlaying,
                                                  PitchScale,
                                                  PauseAllOtherSoundWhilePlaying);}

    public static void PlaySample<StreamPlayerType>(Node caller, 
                                                    String[] StreamPaths, 
                                                    float VolumeMultiplier = 1f,
                                                    Boolean Loop = false,
                                                    Boolean SkipIfAlreadyPlaying = false,
                                                    float PitchScale = 1.0f,
                                                    Boolean PauseAllOtherSoundWhilePlaying = false) where StreamPlayerType : IAudioStreamPlayer, new() {
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
                    player.VolumeDb = GetSampleVolumeDbUnits(VolumeMultiplier);
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
        audioStreamPlayer.VolumeDb = GetSampleVolumeDbUnits(VolumeMultiplier);
        if(PauseAllOtherSoundWhilePlaying){
            SoundHandler.PauseAllStream();
            audioStreamPlayer.SelfNode.AddChild(
                new AudioPlayerWatcher<StreamPlayerType>(AudioPlayerWatcherActivity.UNPAUSE_ALL_WHEN_PLAYER_FINISHED));}
        audioStreamPlayer.Play();
        activeAudioSamplePlayers.Add(audioStreamPlayer);
        }

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

    public static float HumanSoundUnitsToStreamPlayerUnits(int humanSoundUnits){
        var linear = humanSoundUnits / 100f;
        return GD.Linear2Db(linear);}

    public static int StreamPlayerUnitsToHumanSoundUnits(float streamPlayerUnits){
        var db = streamPlayerUnits;
        return (int)(GD.Db2Linear(db) * 100);}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
