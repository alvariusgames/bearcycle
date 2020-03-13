using Godot;
using System;

public enum RocketState { HELD, NOT_HELD}
public class RocketBooster : FSMKinematicBody2D<RocketState>, IHoldable, IInteractable
{

    public int InteractPriority { get { return 200;}}
    public override RocketState InitialState { get { return RocketState.NOT_HELD;}}
    private Vector2 StartingPosition;
    private Player player;
    public Player Player { get { return this.player; } set {this.player = value;}}

    private bool isDepleted;
    public bool IsDepleted { get { return this.isDepleted;} set {this.isDepleted = value;}}

    public int NumActionCallsToDepleted { get {return 300;}}

    private int currentNumActionCalls = 0;
    public int CurrentNumActionCalls { get { return this.currentNumActionCalls;} set {this.currentNumActionCalls = value;}}
    public int NumActionCallsLeft { get { return this.NumActionCallsToDepleted - this.currentNumActionCalls; }}

    private bool isBeingHeld = false;
    public bool IsBeingHeld { get { return this.isBeingHeld;} set {this.isBeingHeld = value;}}

    public Texture UIDisplayIcon { get { 
        return (Texture)GD.Load("res://media/sprites/items/holdables/icons/spring_icon.png");} set {}}

    private CollisionPolygon2D CollisionPolygon2D;

    private uint startingCollisionLayer;
    private uint startingCollisionMask;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        this.StartingPosition = this.GetGlobalPosition();
        this.startingCollisionLayer = this.GetCollisionLayer();
        this.startingCollisionMask = this.GetCollisionMask();
 
        foreach(Node2D child in this.GetChildren()){
            if(child is CollisionPolygon2D){
                this.CollisionPolygon2D = (CollisionPolygon2D)child;}
        }
    }
    public void InteractWith(Player player){
        player.PickupHoldable(this);}


     private void makeCollideable(bool collidability){
        if(collidability){
            this.SetCollisionLayer(this.startingCollisionLayer);
            this.SetCollisionMask(this.startingCollisionMask);}
       else {
            this.SetCollisionLayer(0);
            this.SetCollisionMask(0);}}

    public void ReactToActionHold(float delta){
            var forwardVec = this.Player.ATV.CurrentNormal.Rotated((float)Math.PI * 3f / 2f);
            this.Player.ATV.SetVelocityOfTwoWheels((forwardVec* 500) + this.Player.ATV.GetVelocityOfTwoWheels());
            this.CurrentNumActionCalls++;
    }

    public void ReactToActionPress(float delta){}

    public void PickupPreAction(){}

    public void PostDepletionAction(float delta){
        this.IsBeingHeld = false;
        this.CurrentNumActionCalls = 0;
        this.IsDepleted = false;
        this.SetGlobalPosition(this.StartingPosition);
    }

    public override void UpdateState(float delta){
        if(this.IsBeingHeld){
            this.ResetActiveState(RocketState.HELD);
        } else {
            this.ResetActiveState(RocketState.NOT_HELD);
        }
    }

    public override void ReactStateless(float delta){
        if(this.CurrentNumActionCalls >= this.NumActionCallsToDepleted){
            this.IsDepleted = true;
        }
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case(RocketState.HELD):
                this.makeCollideable(false);
                this.SetGlobalPosition(this.Player.ATV.GetDeFactoGlobalPosition());
            break;
            case(RocketState.NOT_HELD):
                this.makeCollideable(true);
                break;
        }
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
