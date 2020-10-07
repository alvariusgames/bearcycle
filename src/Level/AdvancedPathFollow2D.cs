using Godot;
using System;
public enum AdvancedPathFollowState{GOING_FORWARD, GOING_BACKWARD, STATIONARY}

public class AdvancedPathFollow2D : FSMPathFollow2D<AdvancedPathFollowState>{
    public override AdvancedPathFollowState InitialState {get; set;} =  AdvancedPathFollowState.GOING_FORWARD;
    [Export]
    public float Speed = 5f;
    [Export]
    public int StopAtBottomSec { get; set;} = 0;
    [Export]
    public int StopAtTopSec { get; set;} = 0;

    private float offset = 0;
    public Vector2 prevChildPosition = new Vector2(0,0);

    // Called when the node enters the scene tree for the first time.

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void ReactToState(float delta){
    switch(this.ActiveState){
        case AdvancedPathFollowState.STATIONARY:
            break;
        case AdvancedPathFollowState.GOING_FORWARD:
            this.Offset += this.Speed * delta * main.DELTA_NORMALIZER;
            break;
        case AdvancedPathFollowState.GOING_BACKWARD:
            this.Offset -= this.Speed * delta * main.DELTA_NORMALIZER;
            break;}
  }

    public override void ReactStateless(float delta){}
    public override void UpdateState(float delta){

        if(this.UnitOffset == 0){
            this.SetActiveState(AdvancedPathFollowState.STATIONARY, 100);
            this.SetActiveStateAfter(AdvancedPathFollowState.GOING_FORWARD, 100, this.StopAtBottomSec);}
        if(this.UnitOffset == 1){
            this.SetActiveState(AdvancedPathFollowState.STATIONARY, 100);
            this.SetActiveStateAfter(AdvancedPathFollowState.GOING_BACKWARD, 100, this.StopAtTopSec);
        }}

}