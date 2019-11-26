using Godot;
using System;

public abstract class LevelNode2D : Node2D, ILevel
{
    public Player Player;
    public Node2D NodeInst { get { return this;}}
    public abstract int LevelNum{ get;}
    public abstract String Title {get;}
    public abstract String MusicPath{get;}

    private Boolean spaceRock1Collected = false;
    public Boolean SpaceRock1Collected { get { return this.spaceRock1Collected;} set{ this.spaceRock1Collected = value;}}
    private Boolean spaceRock2Collected = false;
    public Boolean SpaceRock2Collected { get { return this.spaceRock2Collected;} set{ this.spaceRock2Collected = value;}}
    private Boolean spaceRock3Collected = false;
    public Boolean SpaceRock3Collected { get { return this.spaceRock3Collected;} set{ this.spaceRock3Collected = value;}}
    private Boolean calledOnce = false;

    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child is Player){
                this.Player = (Player)child;
                this.Player.ActiveLevel = this;
            }
        }
    }

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Process(float delta){
        if(!this.calledOnce){
            SoundHandler.SetStreamVolume(0f);
            SoundHandler.PlayStream<MyAudioStreamPlayer>(this,
                new string[] {this.MusicPath},
                Loop: true);
            this.calledOnce = true;}}}