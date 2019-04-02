using Godot;
using System;

public enum RotationManagerState{ROTATE_SLOW_FORWARD, ROTATE_FAST_FORWARD,
                                 ROTATE_SLOW_BACKWARD, ROTATE_FAST_BACKWARD,
                                 NOT_ROTATING};

public class RotationManager : FSMNode2D<RotationManagerState>{
    public override RotationManagerState InitialState {
        get {return RotationManagerState.NOT_ROTATING;}}
    
    public ATV ATV;
    private float phiRotationToApply = 0f;

    private float TimeElapsedSinceLastLeftPress = 0f;
    private float TimeElapsedSinceLastRightPress = 0f;
    private const float MASH_MAX_PERIOD_S = 0.1f;
    private const float ROTATION_ACCELL_UNIT_SLOW = 0.005f;
    private const float ROTATION_ACCELL_UNIT_FAST = 0.03f;
    private const float MAX_ROTATION_MAGNITUDE = 0.05f;
    private const float MASH_BOOST_SLOWDOWN_EFFECT=0.99f;
    private const float FRICTION_EFFECT=0.9f;

    public override void _Ready(){
        var parent = this.GetParent();
        if(parent is ATV){
            this.ATV = (ATV)parent;}}

    public override void ReactStateless(float delta){
        this.ATV.RotateTwoWheels(this.phiRotationToApply);
        if(Input.IsActionJustPressed("ui_left")){
            this.TimeElapsedSinceLastLeftPress = 0f;}
        else {
            this.TimeElapsedSinceLastLeftPress += delta;}
        if(Input.IsActionJustPressed("ui_right")){
            this.TimeElapsedSinceLastRightPress = 0f;
        } else {
            this.TimeElapsedSinceLastRightPress += delta;}
        GD.Print(this.TimeElapsedSinceLastRightPress);
        }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case RotationManagerState.ROTATE_SLOW_FORWARD:
                this.ATV.CancelAllRotationalEnergy();
                this.ATV.CancelAllBackwardTwoWheelEnergy();
                if(Math.Abs(this.phiRotationToApply) < MAX_ROTATION_MAGNITUDE){
                    this.phiRotationToApply += ROTATION_ACCELL_UNIT_SLOW;
                } else if (this.phiRotationToApply < 0f){
                    this.phiRotationToApply = 0f;}
                break;
            case RotationManagerState.ROTATE_FAST_FORWARD:
                break;
            case RotationManagerState.ROTATE_SLOW_BACKWARD:
                this.ATV.CancelAllRotationalEnergy();
                this.ATV.CancelAllForwardTwoWheelEnergy();
                if(Math.Abs(this.phiRotationToApply) < MAX_ROTATION_MAGNITUDE){
                    this.phiRotationToApply -= ROTATION_ACCELL_UNIT_SLOW;
                } else if (this.phiRotationToApply > 0f ){
                    this.phiRotationToApply = 0f;}
                break;
            case RotationManagerState.ROTATE_FAST_BACKWARD:
                break;
            case RotationManagerState.NOT_ROTATING:
                this.phiRotationToApply = 0f;
                break;
            /* 
            case RotationManagerState.OPEN_TO_ROTATING_FROM_INPUT:
                if(Input.IsActionPressed("ui_left") && this.ATV.ActiveState == ATVState.WITH_BEAR){
                    this.ATV.CancelAllRotationalEnergy();
                    this.ATV.CancelAllForwardTwoWheelEnergy();
                    if(Math.Abs(this.phiRotationToApply) < MAX_ROTATION_MAGNITUDE){
                        this.phiRotationToApply -= ROTATION_ACCELL_UNIT_HOLDING;
                    } else if (this.phiRotationToApply > 0f ){
                        this.phiRotationToApply = 0f;
                    } else {
                        //We're in "Mash Boost" of mashing of rotation
                        this.phiRotationToApply *= MASH_BOOST_SLOWDOWN_EFFECT;
                    }
                    if(this.TimeElapsedSinceLastLeftPress <= MASH_MAX_PERIOD_S){
                        this.phiRotationToApply -= ROTATION_ACCELL_UNIT_MASHING;
                    }
                } else if(Input.IsActionPressed("ui_right") && this.ATV.ActiveState == ATVState.WITH_BEAR){
                    this.ATV.CancelAllRotationalEnergy();
                    this.ATV.CancelAllBackwardTwoWheelEnergy();
                    if(Math.Abs(this.phiRotationToApply) < MAX_ROTATION_MAGNITUDE){
                        this.phiRotationToApply += ROTATION_ACCELL_UNIT_HOLDING;
                    } else if (this.phiRotationToApply < 0f){
                        this.phiRotationToApply = 0f;
                    } else {
                        //We're in "Mash Boost" of mashing of rotation
                        this.phiRotationToApply *= MASH_BOOST_SLOWDOWN_EFFECT;
                    }
 
                    if(this.TimeElapsedSinceLastRightPress <= MASH_MAX_PERIOD_S){
                        this.phiRotationToApply += ROTATION_ACCELL_UNIT_MASHING;
                    }
                } else {
                    this.phiRotationToApply = 0f;
                }
                break;
            case RotationManagerState.NOT_OPEN_TO_ROTATING_FROM_INPUT:
                this.phiRotationToApply = 0f;
                break;
                */
        }
    }

    public override void UpdateState(float delta){
        if(this.ATV.IsInAirNormalized() && this.ATV.ActiveState == ATVState.WITH_BEAR){
            ///If we're in the right situation to be able to accept input
            if(Input.IsActionPressed("ui_right")){
                this.SetActiveState(RotationManagerState.ROTATE_SLOW_FORWARD, 100);
            }
            else if(Input.IsActionPressed("ui_left")){
                this.SetActiveState(RotationManagerState.ROTATE_SLOW_BACKWARD, 100);
            }
            else {
                this.SetActiveState(RotationManagerState.NOT_ROTATING, 100);
            }
        } else {
            this.SetActiveState(RotationManagerState.NOT_ROTATING, 100);}
    }
}
