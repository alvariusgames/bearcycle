using Godot;
using System;

public enum LevelSelectPlayerState { WAITING_FOR_INPUT, MOVING}

public class LevelSelectPlayer : FSMKinematicBody2D<LevelSelectPlayerState>
{
    public override LevelSelectPlayerState InitialState { get { return LevelSelectPlayerState.WAITING_FOR_INPUT;}}

    private LevelPortal MovingTarget;
    public LevelSelect LevelSelect;
    public LevelPortal LevelPortalCurrentlyCollidingWith;

    public override void _Ready(){
        if(this.GetParent() is LevelSelect){
            this.LevelSelect = (LevelSelect)this.GetParent();
        }
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case LevelSelectPlayerState.WAITING_FOR_INPUT:
            if(this.LevelPortalCurrentlyCollidingWith != null){
                var levelPortal = this.LevelPortalCurrentlyCollidingWith;
                   if((this.LevelSelect.AdvanceButton.UserHasJustSelected())
                       && this.LevelSelect.ActiveState == LevelSelectState.WAITING_FOR_PLAYER_INPUT){
                        SceneTransitioner.TransitionToLevel(FromScene: this.GetTree().Root.GetChild(0),
                                                            ToLevelStr: "res://scenes/levels/level1.tscn",
                                                            effect: SceneTransitionEffect.FADE_BLACK,
                                                            numSeconds: 2f,
                                                            FadeOutAudio: true);
                    }
                }
                break;
            case LevelSelectPlayerState.MOVING:
                GD.Print("I would be moving to this target");
                break;}}

    public override void ReactStateless(float delta){
        var prevPos = this.GlobalPosition;
        var col = this.MoveAndCollide(new Vector2(0,0));
        this.GlobalPosition = prevPos;
        if(col != null && col.Collider is LevelPortal){
            this.LevelPortalCurrentlyCollidingWith = (LevelPortal)col.Collider;}
        else{
            this.LevelPortalCurrentlyCollidingWith = LevelPortal.None;
        }}

    public override void UpdateState(float delta){}

    public void MoveTo(LevelPortal levelSelectPortal){
        this.ResetActiveState(LevelSelectPlayerState.MOVING);
        this.MovingTarget = levelSelectPortal;}

}
