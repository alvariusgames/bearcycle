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

    private float SecondsInAirPressingRight = 0f;
    private float SecondsInAirPressingLeft = 0f;
    private const float ROTATION_ACCELL_UNIT = 0.03f;
    private const float MAX_SLOW_ROTATION_MAG = 0.2f;
    private const float MAX_FAST_ROTATION_MAG = 0.90f;
    private const float SEC_HOLDING_BUTTON_TO_FAST_ROT = 4f;
    private const float FRICTION_EFFECT=0.9f;
    public override void _Ready(){
        var parent = this.GetParent();
        if(parent is ATV){
            this.ATV = (ATV)parent;}}

    public override void ReactStateless(float delta){
        GD.Print(this.SecondsInAirPressingRight);
        this.ATV.RotateTwoWheels(this.phiRotationToApply);
        if(this.ATV.IsInAir() && 
           Input.IsActionPressed("ui_left") &&
           Input.IsActionPressed("ui_right")){
               this.SecondsInAirPressingLeft = 0f;
               this.SecondsInAirPressingRight = 0f;
        }
        if(this.ATV.IsInAir() &&
           Input.IsActionPressed("ui_left")){
                this.SecondsInAirPressingLeft += delta;}
        else{
                this.SecondsInAirPressingLeft = 0f;}
        if(this.ATV.IsInAir() &&
           Input.IsActionPressed("ui_right")){
                this.SecondsInAirPressingRight += delta;
        } else {
                this.SecondsInAirPressingRight = 0f;}}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case RotationManagerState.ROTATE_SLOW_FORWARD:
                this.ATV.CancelAllRotationalEnergy();
                this.ATV.CancelAllBackwardTwoWheelEnergy();
                if(Math.Abs(this.phiRotationToApply) < MAX_SLOW_ROTATION_MAG){
                    this.phiRotationToApply += ROTATION_ACCELL_UNIT;
                } else if (this.phiRotationToApply < 0f){
                    this.phiRotationToApply = 0f;}
                break;
            case RotationManagerState.ROTATE_FAST_FORWARD:
                this.ATV.CancelAllRotationalEnergy();
                this.ATV.CancelAllBackwardTwoWheelEnergy();
                if(Math.Abs(this.phiRotationToApply) < MAX_FAST_ROTATION_MAG){
                    this.phiRotationToApply += ROTATION_ACCELL_UNIT;
                } else if (this.phiRotationToApply < 0f){
                    this.phiRotationToApply = 0f;}
                break;
            case RotationManagerState.ROTATE_SLOW_BACKWARD:
                this.ATV.CancelAllRotationalEnergy();
                this.ATV.CancelAllForwardTwoWheelEnergy();
                if(Math.Abs(this.phiRotationToApply) < MAX_SLOW_ROTATION_MAG){
                    this.phiRotationToApply -= ROTATION_ACCELL_UNIT;
                } else if (this.phiRotationToApply > 0f ){
                    this.phiRotationToApply = 0f;}
                break;
            case RotationManagerState.ROTATE_FAST_BACKWARD:
                this.ATV.CancelAllRotationalEnergy();
                this.ATV.CancelAllForwardTwoWheelEnergy();
                if(Math.Abs(this.phiRotationToApply) < MAX_FAST_ROTATION_MAG){
                    this.phiRotationToApply -= ROTATION_ACCELL_UNIT;
                } else if (this.phiRotationToApply > 0f ){
                    this.phiRotationToApply = 0f;}
                break;
            case RotationManagerState.NOT_ROTATING:
                this.phiRotationToApply = 0f;
                break;}}

    public override void UpdateState(float delta){
        if(this.ATV.IsInAirNormalized() && this.ATV.ActiveState == ATVState.WITH_BEAR){
            ///If we're in the right situation to be able to accept input
            if(Input.IsActionPressed("ui_right") && Input.IsActionPressed("ui_left")){
                this.SetActiveState(RotationManagerState.NOT_ROTATING, 100);}
            else if(Input.IsActionPressed("ui_right")){
                if(this.SecondsInAirPressingRight > SEC_HOLDING_BUTTON_TO_FAST_ROT){
                    this.SetActiveState(RotationManagerState.ROTATE_FAST_FORWARD, 100);}
                else if(this.SecondsInAirPressingRight <= SEC_HOLDING_BUTTON_TO_FAST_ROT){
                   this.SetActiveState(RotationManagerState.ROTATE_SLOW_FORWARD, 100);}
                else{
                    this.SetActiveState(RotationManagerState.NOT_ROTATING, 100);}}
            else if(Input.IsActionPressed("ui_left")){
               if(this.SecondsInAirPressingLeft > SEC_HOLDING_BUTTON_TO_FAST_ROT){
                    this.SetActiveState(RotationManagerState.ROTATE_FAST_BACKWARD, 100);}
               else if (this.SecondsInAirPressingLeft <= SEC_HOLDING_BUTTON_TO_FAST_ROT){
                   this.SetActiveState(RotationManagerState.ROTATE_SLOW_BACKWARD, 100);}
               else{
                    this.SetActiveState(RotationManagerState.NOT_ROTATING, 100);}}
            else{
                this.SetActiveState(RotationManagerState.NOT_ROTATING, 100);}
        } else {
            this.SetActiveState(RotationManagerState.NOT_ROTATING, 100);}}
    }