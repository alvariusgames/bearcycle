using Godot;
using System;

public class OneUp : KinematicBody2D, IConsumeable, INonRefreshable{
    [Export]
    public int UUID {get; set;} = 0;
    public int LevelNum {get; set;} = 0;
    public override void _Ready(){
        this.LevelNum = this.GetParentLevel(this).LevelNum;}

    private ILevel GetParentLevel(Node node){
        if(node is ILevel){
            return (ILevel)node;}
        else{
            return this.GetParentLevel(node.GetParent());}}

    public void consume(Node2D node){
        var player = ((WholeBodyKinBody)node).Player;
        //Increment number of lives in memory and in the Db..
        player.NumLives++;
        var activeSlot = DbHandler.ActiveSlot;
        activeSlot.NumLives = player.NumLives;
        DbHandler.ActiveSlot = activeSlot;
        //and save me to nonrefreshables Db so I don't refresh
        DbHandler.SaveNonRefreshable(this);
        //Delete me
        SoundHandler.PlaySample<MyAudioStreamPlayer>(player,
                                                     new string[]{"res://media/samples/player/1up.wav"},
                                                     PauseAllOtherSoundWhilePlaying: true);
        this.GetParent().RemoveChild(this);
    }
}
