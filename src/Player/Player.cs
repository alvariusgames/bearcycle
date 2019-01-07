using Godot;
using System;

public enum PlayerState {NORMAL}
public class Player : FSMNode2D<PlayerState>
{
    public ATV ATV;
    private SafetyCheckPoint LastSafetyCheckPoint;

    public override void _Ready(){
       foreach(var child in this.GetChildren()){
            if(child is ATV){
                this.ATV = (ATV)child;}}}

    public override void ReactStateless(float delta){

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
//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
