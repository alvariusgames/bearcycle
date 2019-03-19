using Godot;
using System;

public enum AttackWindowState {ATTACKING, NOT_ATTACKING}

public class AttackWindow : FSMKinematicBody2D<AttackWindowState>
{
    public override AttackWindowState InitialState { get { return AttackWindowState.NOT_ATTACKING;}}
    public CollisionPolygon2D CollisionPolygon2D;
    public Polygon2D Polygon2D;
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
                this.Polygon2D = (Polygon2D)this.CollisionPolygon2D.GetChild(0);
                this.Polygon2D.Polygon = this.CollisionPolygon2D.Polygon;
            }
        }
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void ReactStateless(float delta){
        this.SetGlobalPosition(this.Player.ATV.GetGlobalCenterOfTwoWheels());
        if(this.Player.ATV.Direction == ATVDirection.FORWARD){
            this.SetScale(new Vector2(1,1));
            this.SetGlobalRotation(this.Player.ATV.GetNormalizedBackToFront().Angle());
       } else if (this.Player.ATV.Direction == ATVDirection.BACKWARD){
           this.SetScale(new Vector2(-1,1));
           this.SetGlobalRotation(this.Player.ATV.GetNormalizedBackToFront().Angle());
       }
    }

    public override void UpdateState(float delta){}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case AttackWindowState.NOT_ATTACKING:
                this.Polygon2D.Visible = false;
                break;
            case AttackWindowState.ATTACKING:
                this.Polygon2D.Visible = true;
                for(var i=0; i<this.GetSlideCount(); i++){
                    var coll = this.GetSlideCollision(i);
                    GD.Print("I hit something!"); 
                }
                break;
            default:
                throw new Exception("Invalid Attack Window state");
        }
    }


//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
