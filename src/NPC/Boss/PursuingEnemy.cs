using Godot;
using System;
using System.Collections.Generic;

public enum PursuingEnemyState {LOCKED, IDLE, MOVING_TOWARD_PLAYER_GROUND, ATTACKING, MOVING_AWAY_FROM_PLAYER_GROUND, TRIGGER_JUMP, IN_AIR}
public enum EnemyPursuePattern {AUTO_1, STATIONARY_ATACK_1};
public abstract class PursuingEnemy : FSMKinematicBody2DBasicPhysics<PursuingEnemyState>, INPC{
    [Export]
    public Boolean AutoAttackPlayerWhenClose = true;
    [Export]
    public float MinDistanceToAttackPlayer = 300f;
    [Export]
    public EnemyPursuePattern PursuitPattern {get; set;} = EnemyPursuePattern.AUTO_1;
    [Export]
    public Player PlayerToPursue {get; set;}
    [Export]
    public bool ResetPlayerAttackWindowAfterGettingHit {get; set;} = true;
    [Export]
    public NodePath NPCNonPhysicsColliderPath {get; set;}
    private NPCNonPhysicsCollider npcNonPhysicsCollider;
    private Vector2 initialNPCNonPhysicsColliderPosition = new Vector2(0,0);
    public NPCNonPhysicsCollider NPCNonPhysicsCollider { get {
        if(this.npcNonPhysicsCollider is null){
            this.npcNonPhysicsCollider = this.GetNode<NPCNonPhysicsCollider>(
                NPCNonPhysicsColliderPath);}
        return this.npcNonPhysicsCollider;}}

    private void inferPlayerRescurs(Node n){
        if(n is LevelNode2D){
            n._Ready(); //TODO: find better way beyond this horrible hack
            this.PlayerToPursue = ((LevelNode2D)n).Player;}
        else{
            var parent = n.GetParent();
            if(parent != null){
                this.inferPlayerRescurs(n.GetParent());}}}

    public override void _Ready(){
        base._Ready();
        this.initialNPCNonPhysicsColliderPosition = 
            this.NPCNonPhysicsCollider.Position;
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
        this.NPCNonPhysicsCollider.Position = 
            this.initialNPCNonPhysicsColliderPosition;}

    public void TurnAround(){
        this.velocity = 0f * this.velocity;
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
 
        if(this.AutoAttackPlayerWhenClose){
            var dist = this.PlayerToPursue.ATV.Bear.GlobalPosition.DistanceTo(
                this.GlobalPosition);
            if(dist < this.MinDistanceToAttackPlayer){
                this.SetActiveState(PursuingEnemyState.ATTACKING, 100);}
            else{
                if(this.ActiveStatePriority < 600){
                    this.ResetActiveState(this.ActiveState);}}}}

    public abstract void PursueIdle(float delta);
    public abstract void PursueMovingTowardPlayerGround(float delta);
    public abstract void PursueAttacking(float delta);
    public abstract void PursueMovingAwayPlayerGround(float delta);
    public abstract void GetHitBy(Node node);

}
