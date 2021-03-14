using Godot;
using System;
using System.Collections.Generic;

public enum GameInfoPauserDisplayLocations { RIGHT_OF_HUD}
public class GameInfoPauser : KinematicBody2D{
    [Export]
    public Godot.Collections.Array<NodePath> InfoNodesToDisplayPaths {get; set;}
    private Queue<Node2D> infoNodesToDisplay;
    public Queue<Node2D> InfoNodesToDisplay { get {
        if(this.infoNodesToDisplay is null){
            this.infoNodesToDisplay = new Queue<Node2D>();
            foreach(var path in this.InfoNodesToDisplayPaths){
                this.infoNodesToDisplay.Enqueue(this.GetNode<Node2D>(path));
            }}
        return this.infoNodesToDisplay;
    }}

    [Export]
    public GameInfoPauserDisplayLocations DisplayLocation {get; set;}

    [Export]
    public float SecondsToDisplayInfoNode {get; set;} = 5f;

    private Boolean hasBeenCompleted = false;
    private Boolean hasBeenStarted = false;
    private float timer = 0f;
    private Node2D currentlyDisplayingInfoNode;

    private Node2D infoNodeParent;
    public Node2D InfoNodeParent { get {
        if(this.infoNodeParent is null){
            var displHandl = this.LevelFrame.PlayerStatsDisplayHandler;
            if(this.DisplayLocation.Equals(
               GameInfoPauserDisplayLocations.RIGHT_OF_HUD)){
                this.infoNodeParent = displHandl.RightOfHudLocation;}}
        return this.infoNodeParent;
    }}

    private LevelFrame levelFrame;
    private LevelFrame LevelFrame { get {
        if(this.levelFrame is null){
            this.levelFrame = this.getLevelFrameRecurs(this);}
        return this.levelFrame;
    }}

    private LevelFrame getLevelFrameRecurs(Node node){
        LevelFrame output;
        if(node is LevelFrame){
            return (LevelFrame)node;}
        else{
            output = this.getLevelFrameRecurs(node.GetParent());}
        return output;}

    public override void _Ready(){
        base._Ready();
        this.PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float delta){
        base._Process(delta);
        var collision = this.MoveAndCollide(new Vector2(0,0));
        if(collision != null &&
           collision.Collider is WholeBodyKinBody &&
           !this.hasBeenStarted){
            this.hasBeenStarted = true;
            this.LevelFrame.UnderHudTransp.Visible = true;
            this.GetTree().Paused = true;}
        if(this.hasBeenStarted && !this.hasBeenCompleted){
            this.displayInfoNodesProcess(delta);}
    }

    private void displayInfoNodesProcess(float delta){
        this.timer += delta;

        if(this.timer > this.SecondsToDisplayInfoNode){
            this.timer = 0f;

            this.currentlyDisplayingInfoNode.GetParent().RemoveChild(this.currentlyDisplayingInfoNode);
            this.currentlyDisplayingInfoNode._Process(delta);
            this.currentlyDisplayingInfoNode = null;}

        if((this.InfoNodesToDisplay.Count == 0) && 
           (this.currentlyDisplayingInfoNode is null)){
            this.hasBeenCompleted = true;
            this.LevelFrame.UnderHudTransp.Visible = false;
            this.GetTree().Paused = false;
            return;}
        else if(this.currentlyDisplayingInfoNode is null){             
            this.currentlyDisplayingInfoNode = this.InfoNodesToDisplay.Dequeue();
            this.currentlyDisplayingInfoNode.GetParent().RemoveChild(this.currentlyDisplayingInfoNode);
            this.InfoNodeParent.AddChild(this.currentlyDisplayingInfoNode);
            this.currentlyDisplayingInfoNode.GlobalPosition = this.infoNodeParent.GlobalPosition;
            this.currentlyDisplayingInfoNode.Position = new Vector2(0,0);}

    }
}