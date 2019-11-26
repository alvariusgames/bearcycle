using Godot;
using System;


public class LevelPortal : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public LevelPortalChain LevelPortalChain;
    public TouchScreenButton TouchScreenButton;
    public ILevel level;
    public ILevel GetLevel(){return this.level;}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        if(this.Name.Contains("1")){
            this.level = Level.AssembleLevelFrom(level1.ConstTitle, DbHandler.GetLevelStatsRecordFor(1));
        }
        if(this.GetParent() is LevelPortalChain){
            this.LevelPortalChain = (LevelPortalChain)this.GetParent();}
        foreach(Node2D child in this.GetChildren()){
            if(child is TouchScreenButton){
                this.TouchScreenButton = (TouchScreenButton)child;}
    }}

    public override void _Process(float delta){
        var prevPos = this.GlobalPosition;
        var col = this.MoveAndCollide(new Vector2(0,0));
        this.GlobalPosition = prevPos;

        if(col == null && this.TouchScreenButton.IsPressed()){
            var levelSelectPlayer = this.LevelPortalChain.LevelSelect.LevelSelectPlayer;
            levelSelectPlayer.MoveTo(this);
        }
    }

    public static LevelPortal None { get { return null;}}

}
