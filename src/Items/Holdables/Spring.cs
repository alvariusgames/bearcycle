using Godot;
using System;

public class Spring : MonitorHoldable {
    public override Texture UIDisplayIcon { get { 
        return (Texture)GD.Load("res://media/sprites/items/holdables/icons/spring_icon.png");} set {}}
    public const String BOING_SAMPLE = "res://media/samples/items/spring.wav";
    [Export]
    public int JumpVelocity {get; set;} = 2000;
    public override Boolean DisplayInProgressBar {get {return false;}}
    public override int NumActionCallsToDepleted { get {return 1;} set{}}

    public override void ReactToActionPress(float delta){
        if(!this.Player.ATV.IsInAirNormalized()){
            this.Player.ATV.SetVelocityOfTwoWheels((this.Player.ATV.CurrentNormal * this.JumpVelocity) + this.Player.ATV.GetVelocityOfTwoWheels());
            this.IsDepleted = true;
            this.CurrentNumActionCalls++;
            SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.Player.ATV.Bear,
                new string[] {BOING_SAMPLE});}}
    public override void ReactToActionHold(float delta){}
    public override void ReactToActionRelease(float delta){}

}

/*
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public int InteractPriority { get { return 200;}}
    public override SpringState InitialState { get { return SpringState.NOT_HELD;}set{}}
    private Vector2 StartingPosition;
    private Player player;
    public Player Player { get { return this.player; } set {this.player = value;}}

    private bool isDepleted;
    public bool IsDepleted { get { return this.isDepleted;} set {this.isDepleted = value;}}

    public int NumActionCallsToDepleted { get {return 1;}}

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
    public const String BOING_SAMPLE = "res://media/samples/items/spring.wav";

    private AnimatedSprite MonitorAnimSprite;
    private AnimatedSprite CloudAnimSprite;
    [Export]
    public int JumpVelocity {get; set;} = 2000;
    private Node OriginalParent;
    public Boolean DisplayInProgressBar {get;} = false;

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

    public void InteractWith(Player player){
        player.PickupHoldable(this);}

     private void makeCollideable(bool collidability){
        if(collidability){
            this.SetCollisionLayer(this.startingCollisionLayer);
            this.SetCollisionMask(this.startingCollisionMask);}
       else {
            this.SetCollisionLayer(0);
            this.SetCollisionMask(0);}}

    public void ReactToActionPress(float delta){
        if(!this.Player.ATV.IsInAirNormalized()){
            this.Player.ATV.SetVelocityOfTwoWheels((this.Player.ATV.CurrentNormal * this.JumpVelocity) + this.Player.ATV.GetVelocityOfTwoWheels());
            this.IsDepleted = true;
            this.CurrentNumActionCalls++;
            SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.Player.ATV.Bear,
                new string[] {BOING_SAMPLE});
        }
    }

    public void ReactToActionHold(float delta){}

    public void PickupPreAction(){
        this.MonitorAnimSprite.Visible = false;
        this.CloudAnimSprite.Visible = true;
        this.Player.AboveHeadManager.MakeInteractablePromptTempInvisible();
        this.Player.AboveHeadManager.AddAboveHead(this);}

    public void PostDepletionAction(){
        this.Player.AboveHeadManager.RemoveAboveHead(this);
        this.OriginalParent.AddChild(this);
        this.IsBeingHeld = false;
        this.CurrentNumActionCalls = 0;
        this.IsDepleted = false;
        this.SetGlobalPosition(this.StartingPosition);

    }

    public override void UpdateState(float delta){
        if(this.IsBeingHeld){
            this.ResetActiveState(SpringState.HELD);
        } else {
            this.ResetActiveState(SpringState.NOT_HELD);
        }
    }

    public override void ReactStateless(float delta){
        if(this.CurrentNumActionCalls >= this.NumActionCallsToDepleted){
            this.IsDepleted = true;
        }
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case(SpringState.HELD):
                this.makeCollideable(false);
                this.MonitorAnimSprite.Visible = false;
                this.CloudAnimSprite.Visible = true;
            break;
            case(SpringState.NOT_HELD):
                this.MonitorAnimSprite.Visible = true;
                this.CloudAnimSprite.Visible = false; 
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
*/