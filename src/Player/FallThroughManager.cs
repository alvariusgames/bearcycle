using Godot;
using System;

public enum FallThroughManagerState { NOT_FALLING_THROUGH, FALLING_THROUGH}
public class FallThroughManager : FSMNode2D<FallThroughManagerState>{
    public override FallThroughManagerState InitialState { get { 
        return FallThroughManagerState.NOT_FALLING_THROUGH;}}
    public ATV ATV;
    public const float DOWN_PRESS_THRESH_SEC = 1.5f;
    public float numSecondsPressingDown = 0f;
    public override void _Ready(){
        this.ATV = (ATV)this.GetParent();
        this.numSecondsPressingDown = 0f;
    }

    public override void ReactStateless(float delta){
        if(Input.IsActionPressed("ui_down") && 
          !Input.IsActionPressed("ui_right") &&
          !Input.IsActionPressed("ui_left") && 
          !Input.IsActionPressed("ui_up") && 
          !this.ATV.IsInAirNormalized()){
            this.numSecondsPressingDown += delta;}
        else {
            this.numSecondsPressingDown = 0f;}
    }
    public override void UpdateState(float delta){
        if(!this.ATV.IsInAirNormalized() && this.numSecondsPressingDown >= DOWN_PRESS_THRESH_SEC){
            this.ResetActiveState(FallThroughManagerState.FALLING_THROUGH);}
        else{
            this.ResetActiveState(FallThroughManagerState.NOT_FALLING_THROUGH);} 
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case FallThroughManagerState.NOT_FALLING_THROUGH:
                break;
            case FallThroughManagerState.FALLING_THROUGH:
                var bit2 = ZoneCollider.ZoneNumToCollisionLayer[2];
                this.ATV.Bear.SetCollisionMaskBit(bit2, false);
                this.ATV.FrontWheel.SetCollisionMaskBit(bit2, false);
                this.ATV.BackWheel.SetCollisionMaskBit(bit2, false);
                break;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
