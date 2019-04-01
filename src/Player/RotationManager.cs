using Godot;
using System;

public enum RotationManagerState{OPEN_TO_ROTATING_FROM_INPUT, NOT_OPEN_TO_ROTATING_FROM_INPUT};

public class RotationManager : FSMNode2D<RotationManagerState>{
    public override RotationManagerState InitialState {
        get {return RotationManagerState.NOT_OPEN_TO_ROTATING_FROM_INPUT;}}
    
    public ATV ATV;

    public override void _Ready(){
        var parent = this.GetParent();
        if(parent is ATV){
            this.ATV = (ATV)parent;}}

    public override void ReactStateless(float delta){
        if(this.ATV.IsInAirNormalized()){
            this.SetActiveState(RotationManagerState.OPEN_TO_ROTATING_FROM_INPUT, 100);
        } else {
            this.SetActiveState(RotationManagerState.NOT_OPEN_TO_ROTATING_FROM_INPUT, 100);}}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case RotationManagerState.OPEN_TO_ROTATING_FROM_INPUT:
                if(Input.IsActionPressed("ui_left")){
                    this.ATV.RotateTwoWheels(0.1f);
                } else if(Input.IsActionPressed("ui_right")){
                    this.ATV.RotateTwoWheels(-0.1f);
                }
                break;
            case RotationManagerState.NOT_OPEN_TO_ROTATING_FROM_INPUT:
                break;
        }
    }

    public override void UpdateState(float delta){}

}
