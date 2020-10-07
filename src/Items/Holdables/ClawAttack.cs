using Godot;
using System;

public enum ClawAttackState { NORMAL, TRIGGER_ACTION, TRIGGER_ATTACK, ATTACK, TRIGGER_END_ATTACK, LOCKED}

public class ClawAttack : FSMNode2D<ClawAttackState> {

    public override ClawAttackState InitialState { get { return ClawAttackState.NORMAL;}set{}}
    private Player player;
    public Player Player { get { return this.player; } set {this.player = value;}}

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
        this.SetActiveState(ClawAttackState.TRIGGER_ATTACK, Priorities.TriggerClawAttack);
    }

    public override void UpdateState(float delta){
    }

    public override void ReactStateless(float delta){}

    private Boolean IsAttackingInOppositeDirectionOfATV(){
        var attackWindowState = this.Player.AttackWindow.ActiveState;
        var atvDir = this.Player.ATV.Direction;
        return (attackWindowState.Equals(AttackWindowState.ATTACKING_FORWARD) &&  atvDir.Equals(ATVDirection.BACKWARD)) ||
               (attackWindowState.Equals(AttackWindowState.ATTACKING_BACKWARD) && atvDir.Equals(ATVDirection.FORWARD));
    }

    public void PlayRoarSound(){
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.Player.ATV.Bear,
            new string[] {ROAR1_SAMPLE},
            VolumeMultiplier: 0.75f);
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.Player.ATV.Bear,
            new string[] {WOOSH1_SAMPLE},
            VolumeMultiplier: 0.75f);}

    private void updateBearAttackAnimationFromDirection(){
        String animation = null;
        if(this.Player.AttackWindow.ActiveState.Equals(AttackWindowState.ATTACKING_FORWARD) ||
           this.Player.AttackWindow.ActiveState.Equals(AttackWindowState.ATTACKING_BACKWARD) || 
           this.Player.AttackWindow.ActiveState.Equals(AttackWindowState.ATTACKING_DIRECTIONLESS_DEFAULT)){
                if(this.IsAttackingInOppositeDirectionOfATV()){
                    animation = "attack_backwards1";}
                else {
                    animation = "attack1";}}
        else if(this.Player.AttackWindow.ActiveState.Equals(AttackWindowState.ATTACKING_UPWARD)){
                animation = "attack_up1";}
        else if(this.Player.AttackWindow.ActiveState.Equals(AttackWindowState.ATTACKING_DOWNWARD)){
                animation = "attack_down1";}
        if(animation != null){}}
            //this.Player.ATV.Bear.AnimationPlayer.AdvancedPlay(animation, skipIfAlreadyPlaying: true);}}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case ClawAttackState.NORMAL:
                this.Player.AttackWindow.ResetActiveState(AttackWindowState.NOT_ATTACKING);
                break;
            case ClawAttackState.TRIGGER_ATTACK:
                var attackDurationS = 0.5f;
                this.PlayRoarSound();
                this.SetActiveState(ClawAttackState.ATTACK, 200);
                this.Player.AttackWindow.TriggerAttack();
                //this.Player.ATV.Bear.AnimationPlayer.Stop();
                this.updateBearAttackAnimationFromDirection();
                this.SetActiveStateAfter(ClawAttackState.TRIGGER_END_ATTACK, 400, attackDurationS);
                break;
            case ClawAttackState.ATTACK:
                this.updateBearAttackAnimationFromDirection();
                break;
            case ClawAttackState.TRIGGER_END_ATTACK:
                //this.Player.ATV.Bear.AnimationPlayer.AdvancedPlay("idleBounce1");
                this.ResetActiveState(ClawAttackState.NORMAL);
                break;
            case ClawAttackState.LOCKED:
                break;}}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

