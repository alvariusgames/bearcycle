using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState {NORMAL, TRIGGER_ATTACK, ATTACK, TRIGGER_END_ATTACK}
public class Player : FSMNode2D<PlayerState>{
    public override PlayerState InitialState {get { return PlayerState.NORMAL;}}
    public AttackWindow AttackWindow;
    public ATV ATV;
    private const float MAX_HEALTH = 100;
    private const float HEALTH_TO_CALORIES_RATIO = 1f/100f;
    public const float DEFAULT_HIT_UNIT = -6f;
    public float CurrentHealth {get; private set;} = MAX_HEALTH;
    public float TotalCalories = 0;
    private List<IFood> foodEaten = new List<IFood>();
    public IFood lastFoodEaten;
    private SafetyCheckPoint LastSafetyCheckPoint;
    private WholeBodyKinBody WholeBodyKinBody;

    public override void _Ready(){
       this.ResetActiveState(this.InitialState);
       this.setInitialSafetyCheckPoint();
       foreach(var child in this.GetChildren()){
            if(child is ATV){
                this.ATV = (ATV)child;}
            else if(child is AttackWindow){
                this.AttackWindow = (AttackWindow)child;}
            else if(child is WholeBodyKinBody){ //&& ((Node2D)child).Name.ToLower().Contains("wholebody")){
                this.WholeBodyKinBody = (WholeBodyKinBody)child;}}}

    private void setInitialSafetyCheckPoint(){
       this.LastSafetyCheckPoint = new SafetyCheckPoint();
       this.LastSafetyCheckPoint.GlobalPositionToResetTo = this.GetGlobalPosition();
       this.LastSafetyCheckPoint.BeenActivated = true;
    }

    public override void ReactStateless(float delta){
        this.CurrentHealth -= delta;
    }
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case PlayerState.NORMAL:
                this.AttackWindow.ResetActiveState(AttackWindowState.NOT_ATTACKING);
                break;
            case PlayerState.TRIGGER_ATTACK:
                var delayS = 0.2f;
                var attackDurationS = 0.3f;
                this.playBearAnimation("attack1");                
                this.SetActiveStateAfter(PlayerState.ATTACK, 200, delayS);
                this.SetActiveStateAfter(PlayerState.TRIGGER_END_ATTACK, 400, delayS + attackDurationS);
                break;
            case PlayerState.ATTACK:
                this.AttackWindow.SetActiveState(AttackWindowState.ATTACKING, 100);
                break;
            case PlayerState.TRIGGER_END_ATTACK:
                this.playBearAnimation("idleBounce1");
                this.ResetActiveState(PlayerState.NORMAL);
                break;
            default:
                throw new Exception("Invalid Player State");
        }

    }

    public void stopAllBearAnimation(){
        this.ATV.Bear.CutOutAnimationPlayer.Stop();
    }

    public void playBearAnimation(params string[] animationNames){
        Random random = new Random();
        var animationName = animationNames[random.Next(0, 
                                           animationNames.Length)];
        this.ATV.Bear.CutOutAnimationPlayer.Play(animationName);
    }

    public override void UpdateState(float delta){
        this.reactToInput(delta);
    }

    public void reactToInput(float delta){
        if(Input.IsActionJustPressed("ui_accept") && this.ATV.ActiveState == ATVState.WITH_BEAR){
            this.SetActiveState(PlayerState.TRIGGER_ATTACK, 100);
        } else {
            this.SetActiveState(PlayerState.NORMAL, 100);
        }
    }

    public void SetMostRecentSafetyCheckPoint(SafetyCheckPoint safetyCheckPoint){
        this.LastSafetyCheckPoint = safetyCheckPoint;
    }

    public void GoToMostRecentSafetyCheckPoint(){
        this.ATV.SetGlobalCenterOfTwoWheels(this.LastSafetyCheckPoint.GlobalPositionToResetTo);
    }

    public void EatFood(IFood food){
        if(!food.isConsumed){
            this.UpdateHealth(food.Calories * HEALTH_TO_CALORIES_RATIO);
            this.TotalCalories += food.Calories;
            this.foodEaten.Add(food);
            this.lastFoodEaten = food;}}
    public void UpdateHealth(float signedHealthUnits){
        var tentativeHealth = this.CurrentHealth + signedHealthUnits;
        if(tentativeHealth >= MAX_HEALTH){
            this.CurrentHealth = MAX_HEALTH;}
        else {
            this.CurrentHealth = tentativeHealth;}}

    public void GetHitBy(Node node){
        if(node is NPC){
            var npc = (NPC)node;
            this.ATV.Bear.TriggerHitSequence();
            //TODO: evaluate if this is needed or not
    }}


//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
