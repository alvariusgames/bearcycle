using Godot;
using System;

public class SafetyCheckPoint : KinematicBody2D, IConsumeable{
    public Boolean BeenActivated = false;
    public Vector2 GlobalPositionToResetTo;
    public CollisionShape2D CollisionShape2D;
    public override void _Ready(){
        this.GlobalPositionToResetTo = this.GetGlobalPosition();
        foreach(var child in this.GetChildren()){
            this.CollisionShape2D = (CollisionShape2D)child;}}

    public void consume(Node2D collider){
        if(collider is WholeBodyKinBody){
            var player = ((WholeBodyKinBody)collider).Player;
            player.SetMostRecentSafetyCheckPoint(this);
            this.BeenActivated = true;
            this.CollisionShape2D.Disabled = true;}}}
