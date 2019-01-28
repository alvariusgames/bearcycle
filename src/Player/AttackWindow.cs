using Godot;
using System;

public class AttackWindow : KinematicBody2D
{
    public CollisionPolygon2D CollisionPolygon2D;
    public Player Player;

    public override void _Ready()
    {
        this.CollisionLayer = 0;
        this.SetCollisionLayerBit(5, true);
        this.CollisionMask = 0;
        this.SetCollisionMaskBit(5,true);
        this.Player = (Player)this.GetParent();
        foreach(Node2D child in this.GetChildren()){
            if(child is CollisionPolygon2D){
                this.CollisionPolygon2D = (CollisionPolygon2D)child;
                var visiblePolygon = (Polygon2D)this.CollisionPolygon2D.GetChild(0);
                visiblePolygon.Polygon = this.CollisionPolygon2D.Polygon;
                GD.Print(visiblePolygon);
            }
        }
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void _Process(float delta){
        this.SetGlobalPosition(this.Player.ATV.GetGlobalCenterOfTwoWheels());
        if(this.Player.ATV.Direction == ATVDirection.FORWARD){
            this.SetScale(new Vector2(1,1));
            this.SetGlobalRotation(this.Player.ATV.GetNormalizedBackToFront().Angle());
       } else if (this.Player.ATV.Direction == ATVDirection.BACKWARD){
           this.SetScale(new Vector2(-1,1));
//           this.SetGlobalTransform(this.GlobalTransform.Inverse());
           this.SetGlobalRotation(this.Player.ATV.GetNormalizedBackToFront().Angle());
       }
//        this.CollisionPolygon2D.SetPosition(new Vector2(0,0));
        //this.CollisionPolygon2D.SetGlobalPosition(this.Player.ATV.GetGlobalCenterOfTwoWheels());
        //GD.Print(this.GetGlobalPosition());
    }
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
