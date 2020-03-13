using Godot;
using System;

public enum AudioPlayerWatcherActivity {UNPAUSE_ALL_WHEN_PLAYER_FINISHED}

public class AudioPlayerWatcher<StreamPlayerType> : Node where StreamPlayerType : IAudioStreamPlayer, new(){

    private AudioPlayerWatcherActivity playerWatcherActivity;

    private IAudioStreamPlayer audioPlayer;

    public AudioPlayerWatcher(AudioPlayerWatcherActivity activity){
        this.playerWatcherActivity = activity;}

    public override void _Ready(){
        this.audioPlayer = (IAudioStreamPlayer)this.GetParent();}

    public override void _Process(float delta){
        switch(this.playerWatcherActivity){
            case AudioPlayerWatcherActivity.UNPAUSE_ALL_WHEN_PLAYER_FINISHED:
                const float ARBITRARY_THRESH_SEC = 0.6f;
                var playbackPos = this.audioPlayer.GetPlaybackPosition();
                var length = this.audioPlayer.Stream.GetLength();
                if(playbackPos + ARBITRARY_THRESH_SEC  >= length){
                    SoundHandler.UnpauseAllSample();
                    SoundHandler.UnpauseAllStream();
                }
                break;
          }
    }
//  {
//      
//  }
}

