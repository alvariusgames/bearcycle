using Godot;
using System;

public enum RotationManagerState{OPEN_TO_ROTATING_FROM_INPUT, NOT_OPEN_TO_ROTATING_FROM_INPUT};

public class RotationManager : FSMNode2D<RotationManagerState>{
    public override RotationManagerState InitialState {
        get {return RotationManagerState.NOT_OPEN_TO_ROTATING_FROM_INPUT;}}
    
    public ATV ATV;
    private float phiRotationToApply = 0f;

    private const float ROTATION_ACCELL_UNIT_HOLDING = 0.005f;
    private const float ROTATION_ACCELL_UNIT_MASHING = 0.03f;
    private const float MAX_ROTATION_MAGNITUDE = 0.05f;
    private const float MASH_BOOST_SLOWDOWN_EFFECT=0.99f;
    private const float FRICTION_EFFECT=0.9f;

    public override void _Ready(){
        var parent = this.GetParent();
        if(parent is ATV){
            this.ATV = (ATV)parent;}}

    public override void ReactStateless(float delta){
        this.ATV.RotateTwoWheels(this.phiRotationToApply);}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
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
                    if(Input.IsActionJustPressed("ui_accept")){
                        //this.phiRotationToApply -= ROTATION_ACCELL_UNIT_MASHING;
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
 
                    if(Input.IsActionJustPressed("ui_accept")){
                        this.phiRotationToApply += ROTATION_ACCELL_UNIT_MASHING;
                    }
                } else {
                    this.phiRotationToApply = 0f;
                }
                break;
            case RotationManagerState.NOT_OPEN_TO_ROTATING_FROM_INPUT:
                this.phiRotationToApply = 0f;
                break;
        }
    }

    public override void UpdateState(float delta){
        if(this.ATV.IsInAirNormalized() && this.ATV.ActiveState == ATVState.WITH_BEAR){
            this.SetActiveState(RotationManagerState.OPEN_TO_ROTATING_FROM_INPUT, 100);
        }else{
            this.SetActiveState(RotationManagerState.NOT_OPEN_TO_ROTATING_FROM_INPUT, 100);
            }}}
