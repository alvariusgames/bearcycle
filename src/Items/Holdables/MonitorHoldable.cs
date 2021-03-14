using Godot;
using System;


public enum MonitorHoldableState { NOT_HELD, HELD };
public abstract class MonitorHoldable : FSMKinematicBody2D<MonitorHoldableState>, IHoldable, IInteractable{
    //Abstract items
    public abstract Texture UIDisplayIcon { get; set;}

    public abstract void ReactToActionPress(float delta);

    public abstract void ReactToActionHold(float delta);
    public abstract void ReactToActionRelease(float delta);

    //Everything else
    public int InteractPriority { get { return 200;}}
    public override MonitorHoldableState InitialState { get { return MonitorHoldableState.NOT_HELD;}set{}}
    private Vector2 StartingPosition;
    private Player player;
    public Player Player { get { return this.player; } set {this.player = value;}}

    private bool isDepleted;
    public bool IsDepleted { get { return this.isDepleted;} set {this.isDepleted = value;}}

    public abstract int NumActionCallsToDepleted {get; set;}

    private int currentNumActionCalls = 0;
    public int CurrentNumActionCalls { get { return this.currentNumActionCalls;} set {this.currentNumActionCalls = value;}}

    public int NumActionCallsLeft { get { return this.NumActionCallsToDepleted - this.currentNumActionCalls; }}

    private bool isBeingHeld = false;
    public bool IsBeingHeld { get { return this.isBeingHeld;} set {this.isBeingHeld = value;}}
    private CollisionPolygon2D CollisionPolygon2D;

    private uint startingCollisionLayer;
    private uint startingCollisionMask;
   
    private AnimatedSprite MonitorAnimSprite;
    private AnimatedSprite CloudAnimSprite;
    private Node OriginalParent;
    public abstract Boolean DisplayInProgressBar {get;}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        this.OriginalParent = this.GetParent();
        this.StartingPosition = this.GetGlobalPosition();
        this.startingCollisionLayer = this.GetCollisionLayer();
        this.startingCollisionMask = this.GetCollisionMask();
 
        foreach(Node2D child in this.GetChildren()){
            if(child is CollisionPolygon2D){
                this.CollisionPolygon2D = (CollisionPolygon2D)child;}
            if(child is AnimatedSprite){
                if(child.Name.ToLower().Contains("cloud")){
                    this.CloudAnimSprite = (AnimatedSprite)child;}
                if(child.Name.ToLower().Contains("monitor")){
                    this.MonitorAnimSprite = (AnimatedSprite)child;
                }
            }
        }
    }

    public void InteractWith(Player player, float delta){
        player.PickupHoldable(this);}

     private void makeCollideable(bool collidability){
        if(collidability){
            this.SetCollisionLayer(this.startingCollisionLayer);
            this.SetCollisionMask(this.startingCollisionMask);}
       else {
            this.SetCollisionLayer(0);
            this.SetCollisionMask(0);}}


    public void PickupPreAction(){
        this.MonitorAnimSprite.Visible = false;
        this.CloudAnimSprite.Visible = true;
        this.Player.AboveHeadManager.MakeInteractablePromptTempInvisible();
        this.Player.AboveHeadManager.AddAboveHead(this);}

    public virtual void PostDepletionAction(){
        this.Player.AboveHeadManager.RemoveAboveHead(this);
        this.OriginalParent.AddChild(this);
        this.IsBeingHeld = false;
        this.CurrentNumActionCalls = 0;
        this.IsDepleted = false;
        this.SetGlobalPosition(this.StartingPosition);

    }

    public override void UpdateState(float delta){
        if(this.IsBeingHeld){
            this.ResetActiveState(MonitorHoldableState.HELD);
        } else {
            this.ResetActiveState(MonitorHoldableState.NOT_HELD);
        }
    }

    public override void ReactStateless(float delta){
        if(this.CurrentNumActionCalls >= this.NumActionCallsToDepleted){
            this.IsDepleted = true;
        }
    }

    private float flashTimer = 0f;
    private void flashModulateMonitor(float delta){
        var mod = 0.85f + 0.15f * (float)Math.Cos(this.flashTimer * 8f);
        this.MonitorAnimSprite.Modulate = new Color(mod,mod,mod);
        this.flashTimer += delta;
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case(MonitorHoldableState.HELD):
                this.MonitorAnimSprite.Modulate = new Color(1f,1f,1f,1f);
                this.makeCollideable(false);
                this.MonitorAnimSprite.Visible = false;
                this.CloudAnimSprite.Visible = true;
            break;
            case(MonitorHoldableState.NOT_HELD):
                this.flashModulateMonitor(delta);
                this.MonitorAnimSprite.Visible = true;
                this.CloudAnimSprite.Visible = false; 
                this.makeCollideable(true);
                break;
        }
    }

}

