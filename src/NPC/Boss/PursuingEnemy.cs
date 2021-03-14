using Godot;
using System;
using System.Collections.Generic;

public enum PursuingEnemyState {LOCKED, IDLE, MOVING_TOWARD_PLAYER_GROUND, ATTACKING, MOVING_AWAY_FROM_PLAYER_GROUND}
public enum PursuitEnemyPattern {FOLLOWING_PATH2D, STATIONARY_ATTACK_1, STATIONARY_ATTACK_2};
public abstract class PursuingEnemy : FSMKinematicBody2D<PursuingEnemyState>, INPC, IVisibilityTrackable{
    private Vector2 velocity = new Vector2(0,0);
    [Export]
    public Boolean ThrowBearOffATV {get; set;} = false;
    [Export]
    public float PlayerHitUnits {get; set;} = Player.DEFAULT_HIT_UNIT;
    [Export]
    public Boolean AutoAttackPlayerWhenClose = true;
    [Export]
    public float MinDistanceToAttackPlayer = 300f;
    [Export]
    public PursuitEnemyPattern PursuitEnemyPattern {get; set;} = PursuitEnemyPattern.FOLLOWING_PATH2D;
    [Export]
    public Player PlayerToPursue {get; set;}
    [Export]
    public bool ResetPlayerAttackWindowAfterGettingHit {get; set;} = true;
    [Export]
    public NodePath VisibilityNotifierPath {get; set;}
    private VisibilityNotifier2D visibilityNotifier2D;
    public VisibilityNotifier2D VisibilityNotifier2D { get { 
        if(this.visibilityNotifier2D is null){
            this.visibilityNotifier2D = this.GetNode<VisibilityNotifier2D>(this.VisibilityNotifierPath);}
        return this.visibilityNotifier2D;}}
    public Boolean IsOnScreen(){
        var currVisible = this.VisibilityNotifier2D.IsOnScreen();
        var out_ = this.wasPreviouslyVisible || currVisible; //bias towards visible
        this.wasPreviouslyVisible = currVisible;
        return out_; }
    private Boolean wasPreviouslyVisible = true;
 
    private EnemyPathFollow2D enemyPathFollow2D;

    private EnemyPathFollow2D EnemyPathFollow2D { get { 
        if(this.enemyPathFollow2D is null && 
           this.GetParent() is EnemyPathFollow2D && 
           this.PursuitEnemyPattern.Equals(PursuitEnemyPattern.FOLLOWING_PATH2D)){
            this.enemyPathFollow2D = (EnemyPathFollow2D)this.GetParent();}
        return this.enemyPathFollow2D;      
    }}

    protected String prevAnimStr = "idle";
    protected String currentAnimStr = "idle";
    public String ActiveAnimationString { 
        get { return this.currentAnimStr;}
        set { this.prevAnimStr = this.currentAnimStr;
              this.currentAnimStr = value;}}

    public Vector2 towardPlayer { get {
        try{
            return (this.PlayerToPursue.ATV.GetGlobalCenterOfTwoWheels() - this.GlobalPosition).Normalized(); }
        catch{
            return new Vector2(0,0);}}}

    private float towardPlayerXMult { get { 
        if(this.towardPlayer.x > 0){
            return 1;}
        else {
            return -1;}
    }}

    private void inferPlayerRescurs(Node n){
        if(n is LevelNode2D){
            if(((LevelNode2D)n).Player is null){
                n._Ready();}//TODO: find better way beyond this horrible hack
            this.PlayerToPursue = ((LevelNode2D)n).Player;}
        else{
            var parent = n.GetParent();
            if(parent != null){
                this.inferPlayerRescurs(n.GetParent());}}}

    public override void _Ready(){
        base._Ready();
        this.inferPlayerRescurs(this);}

    public override PursuingEnemyState InitialState {get; set;} = 
        PursuingEnemyState.IDLE;
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case PursuingEnemyState.LOCKED:
                break;
            case PursuingEnemyState.IDLE:
                this.PursueIdle(delta);
                break;  
            case PursuingEnemyState.MOVING_TOWARD_PLAYER_GROUND:
                this.PursueMovingTowardPlayerGround(delta);
                break;
            case PursuingEnemyState.ATTACKING:
                this.PursueAttacking(delta);
                break;
            case PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND:
                this.PursueMovingAwayPlayerGround(delta);
                break;}}

    public override void ReactStateless(float delta){
        if(this.PursuitEnemyPattern.Equals(PursuitEnemyPattern.STATIONARY_ATTACK_1)){
            if(this.IsOnScreen()){
                this.applyGravity(delta);}}
        else if(this.PursuitEnemyPattern.Equals(PursuitEnemyPattern.STATIONARY_ATTACK_2)){
            this.applyGravity(delta);}
    }

    private void applyGravity(float delta){
        if(this.velocity.y < FSMKinematicBody2DBasicPhysics<int>.MAX_GRAVITY_SPEED){
            this.velocity.y += delta * FSMKinematicBody2DBasicPhysics<int>.GRAVITY;}
        this.MoveAndSlide(this.velocity);}

    public void TurnAround(){
        if(this.ActiveState.Equals(
            PursuingEnemyState.MOVING_TOWARD_PLAYER_GROUND)){
                this.SetActiveState(
                    PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND,
                    1200);
                this.ResetActiveStateAfter(
                    PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND,
                    2f);}
        else if(this.ActiveState.Equals(
            PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND)){
                this.SetActiveState(
                    PursuingEnemyState.MOVING_TOWARD_PLAYER_GROUND,
                    1200);
                this.ResetActiveStateAfter(
                    PursuingEnemyState.MOVING_TOWARD_PLAYER_GROUND,
                    2f);}
    }

    public override void UpdateState(float delta){
        if(this.ActiveState.Equals(PursuingEnemyState.LOCKED)){
            return;}

        if(this.PursuitEnemyPattern.Equals(PursuitEnemyPattern.FOLLOWING_PATH2D) && 
           !(this.EnemyPathFollow2D is null) && 
           this.EnemyPathFollow2D.IsAtEndOfPath){
               this.ResetActiveState(PursuingEnemyState.IDLE);}
 
        if(this.AutoAttackPlayerWhenClose){
            var dist = this.PlayerToPursue.ATV.Bear.GlobalPosition.DistanceTo(
                this.GlobalPosition);
            if(dist < this.MinDistanceToAttackPlayer){
                this.SetActiveState(PursuingEnemyState.ATTACKING, 100);}
            else if(this.PursuitEnemyPattern.Equals(PursuitEnemyPattern.STATIONARY_ATTACK_1) ||
                    this.PursuitEnemyPattern.Equals(PursuitEnemyPattern.STATIONARY_ATTACK_2)){
                this.ResetActiveState(PursuingEnemyState.IDLE);}
            else if(this.ActiveStatePriority < 600){
                this.ResetActiveState(this.ActiveState);}}}

    public virtual void PursueIdle(float delta){
        this.ActiveAnimationString = "idle";
    }

    private float step { get {
        return this.towardPlayerXMult * 5f;}}

    public virtual void PursueMovingTowardPlayerGround(float delta){
        this.ActiveAnimationString = "run";
        switch(this.PursuitEnemyPattern){
            case PursuitEnemyPattern.FOLLOWING_PATH2D:
                if(!(this.EnemyPathFollow2D is null)){
                    if(this.EnemyPathFollow2D.CanAdvance(this.step * delta * main.DELTA_NORMALIZER)){
                        this.EnemyPathFollow2D.Advance(this.step * delta * main.DELTA_NORMALIZER);}
                    else{
                        this.ResetActiveState(PursuingEnemyState.IDLE);}
                }
                break;
        }
    }

    public virtual void PursueMovingAwayPlayerGround(float delta){
        this.ActiveAnimationString = "run";
        switch(this.PursuitEnemyPattern){
            case PursuitEnemyPattern.FOLLOWING_PATH2D:
                if(!(this.EnemyPathFollow2D is null)){
                    if(this.EnemyPathFollow2D.CanAdvance(-this.step * delta * main.DELTA_NORMALIZER)){
                        this.EnemyPathFollow2D.Advance(-this.step * delta * main.DELTA_NORMALIZER);}
                    else{
                        this.ResetActiveState(PursuingEnemyState.IDLE);}
                }
 
                break;
        }
    } 

    public virtual void PursueAttacking(float delta){
        switch(this.PursuitEnemyPattern){
            case PursuitEnemyPattern.FOLLOWING_PATH2D:
                break;
        }
    }

    public abstract void GetHitBy(Node node);

}