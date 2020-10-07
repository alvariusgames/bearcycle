using Godot;
using System;

public class NPCAttackWindow : KinematicBody2D{
    public uint InitialCollisionMask;
    public uint InitialCollisionLayer;

    public override void _Ready(){
        this.InitialCollisionLayer = this.CollisionLayer;
        this.InitialCollisionMask = this.CollisionMask;
    }
    public void GetHitBy(Node n){}

    public void MakeCollideable(bool collidability){
        if(collidability){
            this.CollisionLayer = this.InitialCollisionLayer;
            this.CollisionMask = this.InitialCollisionMask;}
       else {
            this.CollisionLayer = 0;
            this.CollisionMask = 0;}}

}
