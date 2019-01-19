using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState {NORMAL}
public class Player : FSMNode2D<PlayerState>
{
    public ATV ATV;
    private const float MAX_HEALTH = 100;
    private const float HEALTH_TO_CALORIES_RATIO = 1f/100f;
    public const float DEFAULT_HIT_UNIT = -6f;
    public float currentHealth {get; private set;} = MAX_HEALTH;
    public float totalCalories = 0;
    private List<IFood> foodEaten = new List<IFood>();
    private SafetyCheckPoint LastSafetyCheckPoint;

    public override void _Ready(){
       foreach(var child in this.GetChildren()){
            if(child is ATV){
                this.ATV = (ATV)child;}}}

    public override void ReactStateless(float delta){
        this.currentHealth -= delta;
    }
    public override void ReactToState(float delta){

    }
    public override void UpdateState(float delta){

    }

    public void SetMostRecentSafetyCheckPoint(SafetyCheckPoint safetyCheckPoint){
        this.LastSafetyCheckPoint = safetyCheckPoint;
    }

    public void GoToMostRecentSafetyCheckPoint(){
        this.ATV.SetGlobalCenterOfTwoWheels(this.LastSafetyCheckPoint.GlobalPositionToResetTo);
    }

    public void EatFood(IFood food){
        this.UpdateHealth(food.Calories * HEALTH_TO_CALORIES_RATIO);
        this.totalCalories += food.Calories;
        this.foodEaten.Add(food);
    }

    public void UpdateHealth(float signedHealthUnits){
        var tentativeHealth = this.currentHealth + signedHealthUnits;
        if(tentativeHealth >= MAX_HEALTH){
            this.currentHealth = MAX_HEALTH;}
        else {
            this.currentHealth = tentativeHealth;}}

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
