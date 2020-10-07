using Godot;
using System;

public enum LevelSelectViewState { EARTH_VIEW, SPACE_VIEW, TRANSITIONING_TO_EARTH_VIEW, TRANSITIONING_TO_SPACE_VIEW}

public class LevelSelectView : FSMNode2D<LevelSelectViewState>{

    public override LevelSelectViewState InitialState { get { return LevelSelectViewState.EARTH_VIEW;}set{}}
    private float targetMultiplier = 1f;
    private float ongoingMultipler = 1f;

    public void SetToEarthView(float transitionSec = 2f){
        this.targetMultiplier = 1f;
        this.ResetActiveState(LevelSelectViewState.TRANSITIONING_TO_EARTH_VIEW);
        this.ResetActiveStateAfter(LevelSelectViewState.EARTH_VIEW, transitionSec);
 
    }

    public void TryToggleSpaceView(float transitionSec = 2f){
        if(this.ActiveState.Equals(LevelSelectViewState.EARTH_VIEW)){
            this.SetToSpaceView(transitionSec);}
        if(this.ActiveState.Equals(LevelSelectViewState.SPACE_VIEW)){
            this.SetToEarthView(transitionSec);}
    }

    public void SetToSpaceView(float transitionSec = 2f){
        if(main.PlatformType == PlatformType.DESKTOP){
            this.SetToSpaceViewDesktop(transitionSec);}
        else{
            this.SetToSpaceViewMobile(transitionSec);
        }
    }
    public void SetToSpaceViewDesktop(float transitionSec){
        this.targetMultiplier = 0.5f;
        this.ResetActiveState(LevelSelectViewState.TRANSITIONING_TO_SPACE_VIEW);
        this.ResetActiveStateAfter(LevelSelectViewState.SPACE_VIEW, transitionSec);
    }

    public void SetToSpaceViewMobile(float transitionSec){
        this.targetMultiplier = 0.45f;
        this.ResetActiveState(LevelSelectViewState.TRANSITIONING_TO_SPACE_VIEW);
        this.ResetActiveStateAfter(LevelSelectViewState.SPACE_VIEW, transitionSec); 
    }

    public override void ReactStateless(float delta){
        var mult = this.ongoingMultipler;
        this.Scale = new Vector2(mult, mult);
        this.Position = new Vector2((1 - mult) * 1920, (1 - mult) * 1080f);
    }

    public override void UpdateState(float delta){
        
    }


    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case LevelSelectViewState.EARTH_VIEW:
                this.ongoingMultipler = this.targetMultiplier;
                break;
            case LevelSelectViewState.SPACE_VIEW:
                this.ongoingMultipler = this.targetMultiplier;
                break;
            case LevelSelectViewState.TRANSITIONING_TO_EARTH_VIEW:
                this.ongoingMultipler += 0.5f*delta;
                if(this.ongoingMultipler >= this.targetMultiplier){
                    this.ongoingMultipler = this.targetMultiplier;
                }
                break;
            case LevelSelectViewState.TRANSITIONING_TO_SPACE_VIEW:
                this.ongoingMultipler -= 0.5f*delta;
                if(this.ongoingMultipler <= this.targetMultiplier){
                    this.ongoingMultipler = this.targetMultiplier;
                } 
                break;
        }
    }


}
