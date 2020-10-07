using Godot;
using System;

public class RemoveWallWhenHealthIs100 : KinematicBody2D{
    private Player PlayerInst;
    [Export]
    public NodePath PlayerNodePath {get; set;}
    public CollisionPolygon2D CollisionPolygon2D;
    public Vector2 InitialPos;
    public StaticBody2D StaticBody2D;

    public override void _Ready(){
        this.StaticBody2D = (StaticBody2D)this.GetParent();
        this.InitialPos = this.GlobalPosition;
        this.PlayerInst = (Player)this.GetNode(this.PlayerNodePath);
        foreach(Node child in this.GetChildren()){
            if(child is CollisionPolygon2D){
                this.CollisionPolygon2D = (CollisionPolygon2D)child;}}}

    public override void _Process(float delta){
        this.GlobalPosition = this.InitialPos;
        if(this.PlayerInst.Health >= 0.93f * Player.MAX_HEALTH){
            if(this.StaticBody2D != null){
                this.StaticBody2D.GetParent().RemoveChild(this.StaticBody2D);
                this.StaticBody2D = null;}
        }
    }

}