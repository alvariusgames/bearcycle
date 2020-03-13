using Godot;
using System;
using System.Collections.Generic;

public enum BearState {ON_ATV, TRIGGER_HIT_SEQUENCE, HIT_SEQ_FALL_OFF,
                      HIT_SEQ_INVINC, HIT_SEQ_TRIGGER_TEMP_INVINC,
                      TRIGGER_END_HIT_SEQ};

public class Bear : FSMKinematicBody2D<BearState>{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override BearState InitialState {get { return BearState.ON_ATV; }}
    private const float GRAVITY  = 1400.0f;
    private const float MAX_GRAVITY_SPEED = 2200f;
    private const float FRICTION_EFFECT = 0.95f;

    public Vector2 velocity = new Vector2(0,0);
    public BearAnimationPlayer AnimationPlayer;
    public Node2D CutOut;
    public ATV ATV;
    public CollisionShape2D CollisionShape2D;
    public CameraManager CameraManager;
    private const String NOM2_SAMPLE = "res://media/samples/bear/nom2.wav";
    private const String WHIMPER1_SAMPLE = "res://media/samples/bear/whimper1.wav";
    private const String THUD1_SAMPLE = "res://media/samples/atv/wheel_thud1.wav";
    public override void _Ready(){
        this.ResetActiveState(this.InitialState);
        foreach(var child in this.GetChildren()){
            if(((Node2D)child).Name.ToLower().Contains("cutout")){
                this.CutOut = (Node2D)child;
                foreach(var child2 in this.CutOut.GetChildren()){
                    if(child2 is BearAnimationPlayer){
                        this.AnimationPlayer = (BearAnimationPlayer)child2;}
                }
            }
            else if (child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
            }
            else if (child is CameraManager){
                this.CameraManager = (CameraManager)child;
            }
        }
        this.ATV = (ATV)this.GetParent();
    }
    public override void UpdateState(float delta){}

    private Boolean isInAir = false;
    public Boolean IsInAir(){ 
        if(this.ActiveState == BearState.ON_ATV){
            return this.ATV.IsInAir();}
        else { return this.isInAir;}
    }

    public override void ReactToState(float delta){
        KinematicCollision2D collision;
        float numSecondsToWait;
        switch(this.ActiveState){
            case BearState.TRIGGER_HIT_SEQUENCE:
                var hitTimeSec = 2.5f;
                var invincTimeSec = 3f;
                this.SetActiveState(BearState.HIT_SEQ_FALL_OFF, 100);
                this.SetActiveStateAfter(BearState.HIT_SEQ_TRIGGER_TEMP_INVINC, 200, hitTimeSec);
                this.SetActiveStateAfter(BearState.TRIGGER_END_HIT_SEQ, 300, hitTimeSec + invincTimeSec);
                this.AnimationPlayer.AdvancedPlay("die1");
                SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
                    new string[] {WHIMPER1_SAMPLE});
                this.ATV.Player.DropActiveHoldable(delta);
                break;
            case BearState.HIT_SEQ_FALL_OFF:
                this.AnimationPlayer.AdvancedPlay("die1");
                this.applyGravity(delta);
                this.MoveAndSlide(this.velocity);
                this.velocity.x *= 0.95f;
                var collAggreg = 0f;
                var slideCount = this.GetSlideCount();
                if(slideCount == 0){
                    this.isInAir = true;}
                for(int i=0;i<slideCount; i++){
                        collision = this.GetSlideCollision(i);
                        collAggreg += collision.Normal.Angle();
                        if(collision.Collider is Platforms){
                            if(this.isInAir){
                                this.PlayThudSound();}
                            this.isInAir = false;
                            this.velocity.x *= 0.9f;
                            this.velocity.y *= 0.9f;
                            }}
                if(collAggreg != 0f){
                    this.SetGlobalRotation((this.GetGlobalRotation() + (
                        collAggreg / this.GetSlideCount()) + ((float)Math.PI / 2f)) / 2f);}
                else if(this.ATV.Direction == ATVDirection.FORWARD){
                    this.SetGlobalRotation(this.GetGlobalRotation() - 0.05f);}
                else {
                    this.SetGlobalRotation(this.GetGlobalRotation() + 0.05f);
                }
                break;
            case BearState.HIT_SEQ_TRIGGER_TEMP_INVINC:
                this.ATV.ReattachBear();
                this.SetActiveState(BearState.HIT_SEQ_INVINC, 300);
                this.ATV.Player.UpdateHealth(Player.DEFAULT_HIT_UNIT);
                this.AnimationPlayer.Stop();
                this.AnimationPlayer.AdvancedPlay("invinc2");                
                break;
            case BearState.HIT_SEQ_INVINC:
                this.velocity.x = 0f;
                this.velocity.y = 0f;
                if(this.ATV.IsBearSmushedUnderATV()){
                    this.ATV.FlipUpsideDown();
                }
                break;
            case BearState.TRIGGER_END_HIT_SEQ:
                this.AnimationPlayer.Stop();
                this.AnimationPlayer.AdvancedPlay("idleBounce1");
                this.ResetActiveState(BearState.ON_ATV);
                break;
            case BearState.ON_ATV:
                this.velocity.x = 0;
                this.velocity.y = 0;
                collision = this.MoveAndCollide(this.velocity);
                if(collision != null){
                    if(collision.Collider is Platforms){
                        this.commonHitFunctionality();}
                    if(collision.Collider is IConsumeable){
                        ((IConsumeable)collision.Collider).consume(this);}}
                break;
            default:
                throw new Exception("Bear must have a valid state");
        }
    }

    public void TriggerHitSequence(float throwSpeed = 500){
        if(this.ActiveState == BearState.ON_ATV){
            this.SetActiveState(BearState.TRIGGER_HIT_SEQUENCE, 100);
            this.ATV.ThrowBearOffATV(throwSpeed);}
    }

    private void commonHitFunctionality(){
        this.TriggerHitSequence();
    }

    public override void ReactStateless(float delta){
        drawBear(delta);
    }

    private void drawBear(float delta){
        var cutoutScale = this.CutOut.GetScale();
        if(this.ATV.Direction == ATVDirection.FORWARD && !this.ATV.IsInAir()){
            this.CutOut.SetScale(new Vector2(Math.Abs(cutoutScale[0]),
                                             cutoutScale[1]));
        } else if(this.ATV.Direction == ATVDirection.BACKWARD && !this.ATV.IsInAir()){
            this.CutOut.SetScale(new Vector2(-Math.Abs(cutoutScale[0]),
                                             cutoutScale[1]));}}

    private void applyGravity(float delta){
        if(this.velocity.y < MAX_GRAVITY_SPEED){
            this.velocity.y += delta * GRAVITY;
        }
    }

    public void PlayRandomNomSound(){
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
            new string[] { NOM2_SAMPLE },
            VolumeMultiplier: 0.66f);}

    public void PlayThudSound(){
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this,
        new string[] {THUD1_SAMPLE},
        SkipIfAlreadyPlaying: true);
    }
}