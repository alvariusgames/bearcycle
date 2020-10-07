using Godot;
using System;

public enum AttackWindowState {TRIGGER_ATTACK, ATTACKING_DIRECTIONLESS_DEFAULT, ATTACKING_FORWARD, ATTACKING_BACKWARD, ATTACKING_UPWARD, ATTACKING_DOWNWARD, NOT_ATTACKING}

public class PlayerAttackWindow : FSMKinematicBody2D<AttackWindowState>
{
    public override AttackWindowState InitialState { get { return AttackWindowState.NOT_ATTACKING;}set{}}
    public CollisionPolygon2D CollisionPolygon2D;
    public Polygon2D Polygon2D;
    public Player Player;
    private uint startingCollisionMask;
    private uint startingCollisionLayer;
    private int numTimesChangedDirectionDuringAttack = 0;
    const int MAX_NUM_TIMES_CHANGED_DIRECTION = 2;
    private float RotationOffset = 0f;

    private void makeCollideable(bool collidability){
        if(collidability){
            this.CollisionLayer = this.startingCollisionLayer;
            this.CollisionMask = this.startingCollisionMask;}
       else {
            this.CollisionLayer = 0;
            this.CollisionMask = 0;}}

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
        this.GlobalPosition = this.Player.ATV.GetDeFactoGlobalPosition();
        this.GlobalRotation = this.Player.ATV.GetDeFactorGlobalRotation() + this.RotationOffset;
    }
    
    private void updateStateBasedInputsAndPrevAttacks(){
        //Prioritize up/down keys over left/right keys for attack (more intuitive
        //when you are accellerating and want to attack up/down)
        if(Input.IsActionPressed("ui_up") && this.numTimesChangedDirectionDuringAttack < MAX_NUM_TIMES_CHANGED_DIRECTION ){
            if(!this.ActiveState.Equals(AttackWindowState.ATTACKING_UPWARD)){
                this.numTimesChangedDirectionDuringAttack++;}
            this.SetActiveState(AttackWindowState.ATTACKING_UPWARD, 100);}
        else if(Input.IsActionPressed("ui_down") && this.numTimesChangedDirectionDuringAttack < MAX_NUM_TIMES_CHANGED_DIRECTION ){
            if(!this.ActiveState.Equals(AttackWindowState.ATTACKING_DOWNWARD)){
                this.numTimesChangedDirectionDuringAttack++;}
            this.SetActiveState(AttackWindowState.ATTACKING_DOWNWARD, 100);}
        else if(Input.IsActionPressed("ui_right") && this.numTimesChangedDirectionDuringAttack < MAX_NUM_TIMES_CHANGED_DIRECTION){
            if(!this.ActiveState.Equals(AttackWindowState.ATTACKING_FORWARD)){
                this.numTimesChangedDirectionDuringAttack++;}
            this.SetActiveState(AttackWindowState.ATTACKING_FORWARD, 100);}
        else if(Input.IsActionPressed("ui_left") && this.numTimesChangedDirectionDuringAttack < MAX_NUM_TIMES_CHANGED_DIRECTION ){
            if(!this.ActiveState.Equals(AttackWindowState.ATTACKING_BACKWARD)){
                this.numTimesChangedDirectionDuringAttack++;}
            this.SetActiveState(AttackWindowState.ATTACKING_BACKWARD, 100);}
        else {
            this.SetActiveState(AttackWindowState.ATTACKING_DIRECTIONLESS_DEFAULT, 50);
            if(this.numTimesChangedDirectionDuringAttack == 0){
                this.numTimesChangedDirectionDuringAttack = 1;}
        }}

    public override void UpdateState(float delta){}

    public void TriggerAttack(){
        this.numTimesChangedDirectionDuringAttack = 0;
        this.updateStateBasedInputsAndPrevAttacks();
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case AttackWindowState.NOT_ATTACKING:
                this.Polygon2D.Visible = false;
                this.makeCollideable(false);
                break;
            case AttackWindowState.ATTACKING_DIRECTIONLESS_DEFAULT:
                //When no button is pressed, attack the direction facing
                this.RotationOffset = 0f;
                if(this.Player.ATV.Direction == ATVDirection.FORWARD){
                    this.Scale = new Vector2(1,1);
                } else if (this.Player.ATV.Direction == ATVDirection.BACKWARD){
                    this.Scale = new Vector2(-1,1);}
                this.commonAttackFunctionality(delta);
                break;
            case AttackWindowState.ATTACKING_FORWARD:
                this.RotationOffset = 0f;
                this.Scale = new Vector2(1,1);
                this.commonAttackFunctionality(delta);
                break;
            case AttackWindowState.ATTACKING_BACKWARD:
                this.RotationOffset = 0f;
                this.Scale = new Vector2(-1,1);
                this.commonAttackFunctionality(delta);
                break;
            case AttackWindowState.ATTACKING_DOWNWARD:
                if(this.Scale.x > 0){
                    this.RotationOffset = 0.5f * (float)Math.PI;}
                else{
                    this.RotationOffset = 1.5f * (float)Math.PI;}
                this.commonAttackFunctionality(delta);
                break;
            case AttackWindowState.ATTACKING_UPWARD:
                if(this.Scale.x > 0){
                    this.RotationOffset = 1.5f * (float)Math.PI;}
                else{
                    this.RotationOffset = 0.5f * (float)Math.PI;}
                this.commonAttackFunctionality(delta);
                break;
            default:
                throw new Exception("Invalid Attack Window state");
        }
    }

    private void commonAttackFunctionality(float delta){
        this.ReactStateless(delta);
        this.makeCollideable(true);
        this.Polygon2D.Visible = true;
        var collision = this.MoveAndCollide(new Vector2(0,0));
        if(collision != null){
            if(collision.Collider is IFood){
                this.Player.EatFood((IFood)collision.Collider);}
            if(collision.Collider is INPC){
                ((INPC)collision.Collider).GetHitBy(this);
                if(((INPC)collision.Collider).ResetPlayerAttackWindowAfterGettingHit){
                    this.ResetActiveState(AttackWindowState.NOT_ATTACKING);
                    return;
                }}
}
        this.updateStateBasedInputsAndPrevAttacks();
    }
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
