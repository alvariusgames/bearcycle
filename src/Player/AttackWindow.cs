using Godot;
using System;

public enum AttackWindowState {ATTACKING, NOT_ATTACKING}

public class AttackWindow : FSMKinematicBody2D<AttackWindowState>
{
    public override AttackWindowState InitialState { get { return AttackWindowState.NOT_ATTACKING;}}
    public CollisionPolygon2D CollisionPolygon2D;
    public Polygon2D Polygon2D;
    public Player Player;
    private int startingCollisionMask;
    private int startingCollisionLayer;

    private void makeCollideable(bool collidability){
        if(collidability){
            this.SetCollisionLayer(this.startingCollisionLayer);
            this.SetCollisionMask(this.startingCollisionMask);}
       else {
            this.SetCollisionLayer(0);
            this.SetCollisionMask(0);}}

    public override void _Ready(){
        this.Player = (Player)this.GetParent();
        this.startingCollisionLayer = this.GetCollisionLayer();
        this.startingCollisionMask = this.GetCollisionMask();
        foreach(Node2D child in this.GetChildren()){
            if(child is CollisionPolygon2D){
                this.CollisionPolygon2D = (CollisionPolygon2D)child;
                this.Polygon2D = (Polygon2D)this.CollisionPolygon2D.GetChild(0);
                this.Polygon2D.Polygon = this.CollisionPolygon2D.Polygon;}}}

    public override void ReactStateless(float delta){
        this.SetGlobalPosition(this.Player.ATV.GetDeFactoGlobalPosition());
        this.SetGlobalRotation(this.Player.ATV.GetDeFactorGlobalRotation());
        if(this.Player.ATV.Direction == ATVDirection.FORWARD){
            this.SetScale(new Vector2(1,1));
       } else if (this.Player.ATV.Direction == ATVDirection.BACKWARD){
           this.SetScale(new Vector2(-1,1));
       }
    //    GD.Print(this.CollisionPolygon2D.GetGlobalPosition());
    }

    public override void UpdateState(float delta){}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case AttackWindowState.NOT_ATTACKING:
                this.Polygon2D.Visible = false;
                this.makeCollideable(false);
                break;
            case AttackWindowState.ATTACKING:
                this.makeCollideable(true);
                this.Polygon2D.Visible = true;
                var collision = this.MoveAndCollide(new Vector2(0,0));
                if(collision != null){
                    if(collision.Collider is NPC){
                        ((NPC)collision.Collider).GetHitBy(this);}}
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
