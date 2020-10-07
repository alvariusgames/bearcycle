using Godot;
using System;
using System.Collections.Generic;

public enum FallThroughManagerState {TRIGGER_TRANSITION_FALLING_THROUGH_TO_NOT,
                                    TRANSITION_FALLING_THROUGH_TO_NOT, NOT_FALLING_THROUGH, FALLING_THROUGH}
public class FallThroughManager : FSMNode2D<FallThroughManagerState>{
    public override FallThroughManagerState InitialState { get { 
        return FallThroughManagerState.NOT_FALLING_THROUGH;}set{}}
    public ATV ATV;
    public const float DOWN_PRESS_THRESH_SEC = 1.5f;
    public float numSecondsPressingDown = 0f;
    public override void _Ready(){
        this.ATV = (ATV)this.GetParent();
        this.numSecondsPressingDown = 0f;
    }

    public override void ReactStateless(float delta){
        this.trackDownEventHold(delta);
        this.trackDownEventMash(delta);
    }

    private void trackDownEventHold(float delta){
        if(Input.IsActionPressed("ui_down") && 
          !Input.IsActionPressed("ui_right") &&
          !Input.IsActionPressed("ui_left") && 
          !Input.IsActionPressed("ui_up") && 
          !this.ATV.IsInAirNormalized()){
            this.numSecondsPressingDown += delta;}
        else {
            this.numSecondsPressingDown = 0f;}

    }

    private List<float> mashDownTimers = new List<float>();
    private const float SEC_TO_STORE_MASH = 0.6f;
    private const int NUM_MASHES_THRESH = 4;
    private void trackDownEventMash(float delta){
        if(Input.IsActionJustPressed("ui_down")){
            this.mashDownTimers.Add(delta);}
        while(this.mashDownTimers.Count >= NUM_MASHES_THRESH+1){
            //only store the X newest entries
            this.mashDownTimers.RemoveAt(
                this.mashDownTimers.Count-1);}
        for(int i=0;i < this.mashDownTimers.Count; i++){
            this.mashDownTimers[i] += delta;
            if(this.mashDownTimers[i] > SEC_TO_STORE_MASH){
                this.mashDownTimers.RemoveAt(i);}}
    }
    public override void UpdateState(float delta){
        var userWantsToGoDown = 
            (this.numSecondsPressingDown >= DOWN_PRESS_THRESH_SEC) ||
            (this.mashDownTimers.Count >= NUM_MASHES_THRESH);

        if(!this.ATV.IsInAirNormalized() && 
            userWantsToGoDown){
            this.ResetActiveState(FallThroughManagerState.FALLING_THROUGH);}
        else if(this.ActiveState.Equals(FallThroughManagerState.FALLING_THROUGH)){
            //If we were previously going through a platform and are not now, add a delay
            this.ResetActiveState(FallThroughManagerState.TRIGGER_TRANSITION_FALLING_THROUGH_TO_NOT);} 
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case FallThroughManagerState.TRIGGER_TRANSITION_FALLING_THROUGH_TO_NOT:
                this.setPlayerOneWayCollisionBitsToFalse();
                this.ResetActiveStateAfter(FallThroughManagerState.NOT_FALLING_THROUGH, 0.6f);
                this.ResetActiveState(FallThroughManagerState.TRANSITION_FALLING_THROUGH_TO_NOT);
                break; 
            case FallThroughManagerState.TRANSITION_FALLING_THROUGH_TO_NOT:
                this.setPlayerOneWayCollisionBitsToFalse();
                break;
            case FallThroughManagerState.FALLING_THROUGH:
                this.setPlayerOneWayCollisionBitsToFalse();
                break;
            case FallThroughManagerState.NOT_FALLING_THROUGH:
                break;
        }
    }

    private void setPlayerOneWayCollisionBitsToFalse(){
        var bit = Consts.L_PlatformOneWay; 
        this.ATV.Bear.SetCollisionMaskBit(bit, false);
        this.ATV.FrontWheel.SetCollisionMaskBit(bit, false);
        this.ATV.BackWheel.SetCollisionMaskBit(bit, false);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
