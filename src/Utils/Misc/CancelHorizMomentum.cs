using Godot;
using System;

public class CancelHorizMomentum : KinematicBody2D{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public Player Player;
    private Boolean isCollidingWithPlayer = false;
    private Vector2 initialGlobalPos;

    public override void _Ready(){
        this.initialGlobalPos = this.GlobalPosition;}

    public override void _Process(float delta){
        var col = this.MoveAndCollide(new Vector2(0,0));
        this.initialGlobalPos = this.GlobalPosition;
        var isCol = false;
        if(col != null && col.Collider is WholeBodyKinBody){
            isCol = true;
            this.Player = ((WholeBodyKinBody)col.Collider).Player;
            this.Player.ATV.tempStopAllMovement(exceptGravity: true);}
        if(!isCol && this.isCollidingWithPlayer){
            // if we were colliding and now we aren't, let player go </3
            this.Player.ATV.resumeMovement();}
        this.isCollidingWithPlayer = isCol;}
}