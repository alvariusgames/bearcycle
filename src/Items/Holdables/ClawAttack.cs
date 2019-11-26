using Godot;
using System;

public enum ClawAttackState { NORMAL, TRIGGER_ACTION, TRIGGER_ATTACK, ATTACK, TRIGGER_END_ATTACK}

public class ClawAttack : FSMNode2D<ClawAttackState>, IHoldable {

    public override ClawAttackState InitialState { get { return ClawAttackState.NORMAL;}}
    private Player player;
    public Player Player { get { return this.player; } set {this.player = value;}}

    public bool IsDepleted { get { return false;} set {}}

    public int NumActionCallsToDepleted { get {return 2048;}}

    public int CurrentNumActionCalls { get { return 0;} set {}}
    public int NumActionCallsLeft { get { return 0; }}

    private bool isBeingHeld = false;
    public bool IsBeingHeld { get { return this.isBeingHeld;} set {this.isBeingHeld = value;}}

    private const String ROAR1_SAMPLE = "res://media/samples/bear/roar1.wav";
    private const String WOOSH1_SAMPLE = "res://media/samples/bear/woosh1.wav";

    public Texture UIDisplayIcon { get {
        if(main.PlatformType == PlatformType.MOBILE){ 
            return (Texture)GD.Load("res://media/sprites/items/holdables/icons/pause_button.png");}
        else {
            return (Texture)GD.Load("res://media/sprites/items/holdables/icons/claw_attack_icon.png");}
        } set {}}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){}

    public void ReactToActionPress(float delta){
        this.SetActiveState(ClawAttackState.TRIGGER_ATTACK, 100);
        if(this.Player.ATV.Bear.ActiveState != BearState.ON_ATV){
            this.Player.ATV.Bear.ResetActiveState(BearState.ON_ATV);
        }
    }

    public void ReactToActionHold(float delta){}

    public void PostDepletionAction(float delta){}

    public override void UpdateState(float delta){
    }

    public override void ReactStateless(float delta){}

    public void PlayRoarSound(){
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.Player.ATV.Bear,
            new string[] {ROAR1_SAMPLE},
            VolumeMultiplier: 0.75f);
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.Player.ATV.Bear,
            new string[] {WOOSH1_SAMPLE},
            VolumeMultiplier: 0.75f);}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case ClawAttackState.NORMAL:
                this.Player.AttackWindow.ResetActiveState(AttackWindowState.NOT_ATTACKING);
                break;
            case ClawAttackState.TRIGGER_ATTACK:
                var attackDurationS = 0.5f;
                this.Player.playBearAnimation("attack1");
                this.PlayRoarSound();
                this.SetActiveState(ClawAttackState.ATTACK, 200);
                this.SetActiveStateAfter(ClawAttackState.TRIGGER_END_ATTACK, 400, attackDurationS);
                break;
            case ClawAttackState.ATTACK:
                this.Player.AttackWindow.SetActiveState(AttackWindowState.ATTACKING, 100);
                break;
            case ClawAttackState.TRIGGER_END_ATTACK:
                this.Player.playBearAnimation("idleBounce1");
                this.ResetActiveState(ClawAttackState.NORMAL);
                break;}}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

