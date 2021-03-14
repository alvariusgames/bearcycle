using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum LevelSelectPlayerState { WAITING_FOR_INPUT, MOVING_FORWARD, MOVING_BACKWARD}

public class LevelSelectPlayer : FSMKinematicBody2D<LevelSelectPlayerState>
{
    public override LevelSelectPlayerState InitialState { get { return LevelSelectPlayerState.WAITING_FOR_INPUT;} set{}}

    private LevelPortal MovingTarget;
    public LevelSelect LevelSelect;
    public LevelSelectView LevelSelectView;
    public LevelPortal LevelPortalCurrentlyCollidingWith;

    public AnimatedSprite AnimatedSprite;
    public Vector2 InitialAnimatedSpriteScale;
    public Vector2 InitialOffset;
    public VisiblePath2D VisiblePath2D;
    public PathFollow2D PathFollow2D;
    private float travelSpeed = 300f;
    private Boolean isTransitioningToLevel = false;

    public override void _Ready(){
        this.PathFollow2D = (PathFollow2D)this.GetParent();
        this.VisiblePath2D = (VisiblePath2D)this.PathFollow2D.GetParent();
        this.LevelSelectView = (LevelSelectView)this.PathFollow2D.GetParent().GetParent();
        this.LevelSelect = (LevelSelect)this.LevelSelectView.GetParent();
        this.InitialOffset = this.Position;
        foreach(Node child in this.GetChildren()){
            if(child is AnimatedSprite){
                this.AnimatedSprite = (AnimatedSprite)child;
                this.InitialAnimatedSpriteScale = this.AnimatedSprite.Scale;}}
        this.isTransitioningToLevel = false;
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case LevelSelectPlayerState.WAITING_FOR_INPUT:
                this.reactToInputAndEnterLevel(delta);
                this.reactToInputAndMovePlayer(delta);
                break;
            case LevelSelectPlayerState.MOVING_FORWARD:
                this.movePlayer(delta, _Direction.forward);
                if((this.atDestination()) && this.PathFollow2D.UnitOffset > this.MovingTarget.level.UnitOffset){
                    this.PathFollow2D.UnitOffset = this.LevelPortalCurrentlyCollidingWith.level.UnitOffset;}
                break;
            case LevelSelectPlayerState.MOVING_BACKWARD:
                this.movePlayer(delta, _Direction.backward);
                if((this.atDestination()) && this.PathFollow2D.UnitOffset < this.MovingTarget.level.UnitOffset){
                    this.PathFollow2D.UnitOffset = this.LevelPortalCurrentlyCollidingWith.level.UnitOffset;}
                break;}}

    private enum _Direction {forward, backward};

    private void movePlayer(float delta, _Direction direction){

        if(direction.Equals(_Direction.forward)){
            this.AnimatedSprite.Scale = this.InitialAnimatedSpriteScale;}
        if(direction.Equals(_Direction.backward)){
            this.AnimatedSprite.Scale = new Vector2(-this.InitialAnimatedSpriteScale.x,
                                                     this.InitialAnimatedSpriteScale.y);}

        var travelSpeed = 200f;
        var unitOffset = this.PathFollow2D.UnitOffset;
        if(unitOffset > level0.ConstUnitOffset && unitOffset < level1.ConstUnitOffset){
            travelSpeed = 200f;
            this.updateRotationAlongPath(direction);
        } else if(unitOffset > level1.ConstUnitOffset && unitOffset < level2.ConstUnitOffset){
            travelSpeed = 300f;
            this.updateRotationAlongPath(direction);
        } else if(unitOffset > level2.ConstUnitOffset && unitOffset < level3.ConstUnitOffset){
            travelSpeed = 300f;
            this.updateRotationAlongPath(direction);
        } else if(unitOffset > level3.ConstUnitOffset && unitOffset < level4.ConstUnitOffset){
            var fromFront = unitOffset - level3.ConstUnitOffset;
            var fromBack =  level4.ConstUnitOffset - unitOffset;
            var arbitraryMult = Math.Abs(80f / (fromFront - fromBack));
            if(arbitraryMult > 900f){
                arbitraryMult = 900f;}
            travelSpeed = 0f + arbitraryMult; 

            var dist = level4.ConstUnitOffset - level3.ConstUnitOffset;
            float percDone = 0f;
            if(direction.Equals(_Direction.forward)){
                percDone = (unitOffset - level3.ConstUnitOffset) / dist;
            } else {
                percDone = (level4.ConstUnitOffset - unitOffset) / dist;
            }
            if(percDone < 0.8){
                this.updateRotationAlongPath(_Direction.forward, keepUpright: false);
            } else {
                this.updateRotationAlongPath(direction, keepUpright: true);
            }

        } else if(unitOffset > level4.ConstUnitOffset && unitOffset < level5.ConstUnitOffset){
            var fromFront = unitOffset - level4.ConstUnitOffset;
            var fromBack =  level5.ConstUnitOffset - unitOffset;
            var arbitraryMult = Math.Abs(80f / (fromFront - fromBack));
            if(arbitraryMult > 900f){
                arbitraryMult = 900f;}
            travelSpeed = 0f + arbitraryMult; 

            var dist = level5.ConstUnitOffset - level4.ConstUnitOffset;
            float percDone = 0f;
            if(direction.Equals(_Direction.forward)){
                percDone = (unitOffset - level4.ConstUnitOffset) / dist;
            } else {
                percDone = (level5.ConstUnitOffset - unitOffset) / dist;
            }
            this.updateRotationAlongPath(direction, keepUpright: true);

        } else if(unitOffset > level5.ConstUnitOffset && unitOffset < level6.ConstUnitOffset){
            var fromFront = unitOffset - level5.ConstUnitOffset;
            var fromBack =  level6.ConstUnitOffset - unitOffset;
            var arbitraryMult = Math.Abs(80f / (fromFront - fromBack));
            if(arbitraryMult > 900f){
                arbitraryMult = 900f;}
            travelSpeed = 0f + arbitraryMult; 

            var dist = level6.ConstUnitOffset - level5.ConstUnitOffset;
            float percDone = 0f;
            if(direction.Equals(_Direction.forward)){
                percDone = (unitOffset - level5.ConstUnitOffset) / dist;
            } else {
                percDone = (level6.ConstUnitOffset - unitOffset) / dist;
            }
            if(percDone < 0.9){
                this.updateRotationAlongPath(_Direction.backward, keepUpright: false);
            } else {
                this.updateRotationAlongPath(direction, keepUpright: true);
            }
       } else if(unitOffset > level6.ConstUnitOffset && unitOffset < level7.ConstUnitOffset){
            var fromFront = unitOffset - level6.ConstUnitOffset;
            var fromBack =  level7.ConstUnitOffset - unitOffset;
            var arbitraryMult = Math.Abs(80f / (fromFront - fromBack));
            if(arbitraryMult > 800f){
                arbitraryMult = 800f;}
            travelSpeed = 100f + arbitraryMult; 

            var dist = level7.ConstUnitOffset - level6.ConstUnitOffset;
            float percDone = 0f;
            if(direction.Equals(_Direction.forward)){
                percDone = (unitOffset - level6.ConstUnitOffset) / dist;
            } else {
                percDone = (level7.ConstUnitOffset - unitOffset) / dist;
            }
            if(direction.Equals(_Direction.forward) && percDone > 0.69 && percDone < 0.8){
                //Slow motion effect over the moon 
                travelSpeed = 250f;
            }
            if(percDone < 0.9){
                this.updateRotationAlongPath(direction, keepUpright: false);
            } else {
                this.updateRotationAlongPath(direction, keepUpright: true);}}



        if(direction.Equals(_Direction.forward)){
            this.PathFollow2D.Offset = this.PathFollow2D.Offset + delta * travelSpeed;}
        if(direction.Equals(_Direction.backward)){
            this.PathFollow2D.Offset = this.PathFollow2D.Offset - delta * travelSpeed;}
    }

    private void updateRotationAlongPath(_Direction direction, Boolean keepUpright = true){
        if(!keepUpright){
            if(direction.Equals(_Direction.forward)){
                this.AnimatedSprite.Scale = this.InitialAnimatedSpriteScale;}
            else{
                this.AnimatedSprite.Scale = new Vector2(-this.InitialAnimatedSpriteScale.x,
                                                        this.InitialAnimatedSpriteScale.y);
            }}
        else{
            this.Position = new Vector2(this.Position.x,
                                        this.InitialOffset.Rotated(this.PathFollow2D.Rotation).y);
            int directionMultiplier = 1;
            if(direction.Equals(_Direction.forward)){
                directionMultiplier = 1;}
            if(direction.Equals(_Direction.backward)){
                directionMultiplier = -1;}
            if((this.PathFollow2D.Rotation < (-Math.PI / 2f)) || 
                (this.PathFollow2D.Rotation > (Math.PI / 2f))){
                this.AnimatedSprite.Scale = new Vector2(directionMultiplier * -this.InitialAnimatedSpriteScale.x,
                                                        this.InitialAnimatedSpriteScale.y);
                this.AnimatedSprite.Rotation = (float)Math.PI;
            } else {
                this.AnimatedSprite.Scale = new Vector2(directionMultiplier * this.InitialAnimatedSpriteScale.x,
                                                        this.InitialAnimatedSpriteScale.y);
                this.AnimatedSprite.Rotation = 0f;}}
    }

    public Boolean atDestination(){
        return this.MovingTarget != null && this.MovingTarget.Equals(LevelPortalCurrentlyCollidingWith);}

    public override void ReactStateless(float delta){
        this.updateLevelPortalCurrentlyCollidingWith();
    }

    private void updateLevelPortalCurrentlyCollidingWith(){
        var prevPos = this.GlobalPosition;
        var col = this.MoveAndCollide(new Vector2(0,0));
        this.GlobalPosition = prevPos;
        if(col != null && col.Collider is LevelPortal){
            this.LevelPortalCurrentlyCollidingWith = (LevelPortal)col.Collider;}
        else{
            this.LevelPortalCurrentlyCollidingWith = LevelPortal.None;
        }}

    public override void UpdateState(float delta){
        this.updateStateFromDistanceAlongPath(delta);
    }

    private void updateStateFromDistanceAlongPath(float delta){
        if(this.MovingTarget == null){
            return;}
        if(this.PathFollow2D.UnitOffset < this.MovingTarget.level.UnitOffset){
           this.ResetActiveState(LevelSelectPlayerState.MOVING_FORWARD);}
        else if(this.PathFollow2D.UnitOffset > this.MovingTarget.level.UnitOffset){
            this.ResetActiveState(LevelSelectPlayerState.MOVING_BACKWARD);}
        else{
            this.ResetActiveState(LevelSelectPlayerState.WAITING_FOR_INPUT);
        }

    }

    private void reactToInputAndEnterLevel(float delta){
        if(this.LevelPortalCurrentlyCollidingWith != null){
            var levelPortal = this.LevelPortalCurrentlyCollidingWith;
            if((this.LevelSelect.AdvanceButton.UserHasJustSelected()) && 
                this.LevelSelect.ActiveState == LevelSelectState.WAITING_FOR_PLAYER_INPUT &&
                !this.isTransitioningToLevel){
                this.isTransitioningToLevel = true;
                var level = levelPortal.GetLevel();
                DbHandler.SetCurrentLevelHoveringOver(level.LevelNum);
                SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                    new string[]{"res://media/samples/ui/accept_2.wav"});
                SceneTransitioner.TransitionToLevel(FromScene: this.GetTree().Root.GetChild(0),
                                                    ToLevelStr: level.NodePath,
                                                    LevelTitle: level.Title,
                                                    LevelZone: level.Zone,
                                                    effect: SceneTransitionEffect.FADE_BLACK,
                                                    numSeconds: 2f,
                                                    FadeOutAudio: true);}}}

    private void reactToInputAndMovePlayer(float delta){
        var targetLevelPortal = this.LevelPortalCurrentlyCollidingWith;
        if(this.LevelPortalCurrentlyCollidingWith == null){
            return;}
        var levelPortals = this.LevelSelect.LevelPortalChain.LevelPortals;
        if(this.isJustReleasedIfValid(targetLevelPortal.forwardInputString1) ||
           this.isJustReleasedIfValid(targetLevelPortal.forwardInputString2)){
            SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                new string[]{"res://media/samples/ui/click_1.wav"});
            var indexOfLevelPortal = levelPortals.IndexOf(targetLevelPortal);
            var levelPortalToMoveTo = levelPortals[indexOfLevelPortal+1];
            this.MoveTo(levelPortalToMoveTo);}
        if(this.isJustReleasedIfValid(targetLevelPortal.backwardInputString1) ||
           this.isJustReleasedIfValid(targetLevelPortal.backwardInputString2)){
            SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                new string[]{"res://media/samples/ui/click_1.wav"});
            var indexOfLevelPortal = levelPortals.IndexOf(targetLevelPortal);
            var levelPortalToMoveTo = levelPortals[indexOfLevelPortal-1];
            this.MoveTo(levelPortalToMoveTo);}
    }

    private Boolean isJustReleasedIfValid(String inputString){
        return (!(inputString.Equals("")) && Input.IsActionJustReleased(inputString));
    }

    public void MoveTo(LevelPortal levelSelectPortal){
        //this.ResetActiveState(LevelSelectPlayerState.MOVING);
        if(this.LevelSelect.LevelPortalChain.UnlockedLevelPortals.ToList().Contains(levelSelectPortal)){
            this.MovingTarget = levelSelectPortal;}}
}
