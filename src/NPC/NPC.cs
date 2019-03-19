using Godot;
using System;

public class NPC : RigidBody2D{

    public Sprite Sprite;
   public CollisionShape2D CollisionShape2D;
   public bool isHit;

    public void GetHitBy(Node node){
        GD.Print("NPC hit " + node.Name);
        this.CollisionShape2D.Disabled = true;
        this.Sprite.Visible = false;
        this.isHit = true;  
    }
    public override void _Ready(){
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.Sprite = (Sprite)child;
            }
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
            }}}}