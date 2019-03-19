using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState {NORMAL, TRIGGER_ATTACK, ATTACK}
public class Player : FSMNode2D<PlayerState>
{
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

    public override void _Ready(){
       this.ResetActiveState(this.InitialState);
       foreach(var child in this.GetChildren()){
            if(child is ATV){
                this.ATV = (ATV)child;}
            else if(child is AttackWindow){
                this.AttackWindow = (AttackWindow)child;}}}

    public override void ReactStateless(float delta){
        this.CurrentHealth -= delta;
    }
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case PlayerState.NORMAL:
                this.AttackWindow.ResetActiveState(AttackWindowState.NOT_ATTACKING);
                break;
            case PlayerState.TRIGGER_ATTACK:
                var attackDurationS = 0.2f;
                this.SetActiveState(PlayerState.ATTACK, 200);
                this.ResetActiveStateAfter(PlayerState.NORMAL, attackDurationS);
                break;
            case PlayerState.ATTACK:
                this.AttackWindow.SetActiveState(AttackWindowState.ATTACKING, 100);
                break;
            default:
                throw new Exception("Invalid Player State");
        }

    }
    public override void UpdateState(float delta){
        this.reactToInput(delta);
    }

    public void reactToInput(float delta){
        if(Input.IsActionJustPressed("ui_accept")){
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
        GD.Print("Player hit!");

    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
