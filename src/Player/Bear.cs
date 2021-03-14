using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum BearState {ON_ATV, TRIGGER_HIT_SEQUENCE, HIT_SEQ_FALL_OFF,
                      HIT_SEQ_INVINC, HIT_SEQ_TRIGGER_TEMP_INVINC,
                      TRIGGER_END_HIT_SEQ};

public enum BearHitSequence{SPIN_THEN_SPLAT, SWIPED_OFF, STAY_ON_ATV_1}

public class Bear : FSMKinematicBody2D<BearState>{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override BearState InitialState {get { return BearState.ON_ATV;}set{}}
    private const float GRAVITY  = 1400.0f;
    private const float MAX_GRAVITY_SPEED = 2200f;
    private const float FRICTION_EFFECT = 0.95f;

    public Vector2 velocity = new Vector2(0,0);
    public BearAnimationPlayer AnimationPlayer;
    public CutOut CutOut;
    public ATV ATV;
    public CollisionShape2D CollisionShape2D;
    public CameraManager CameraManager;
    private BearHitSequence ActiveHitSequence = BearHitSequence.SWIPED_OFF;
    private const String NOM2_SAMPLE = "res://media/samples/bear/nom2.wav";
    private const String WHIMPER1_SAMPLE = "res://media/samples/bear/whimper1.wav";
    private const String THUD1_SAMPLE = "res://media/samples/atv/wheel_thud1.wav";
    private const float MAX_THUD_FREQ_PER_SEC = 0.5f;
    private const int NUM_GLOBAL_ROTATIONS_TO_STORE = 15;
    private Queue<float> OngoingGlobalRotations = new Queue<float>();
    public override void _Ready(){
        base._Ready();
        this.ResetActiveState(this.InitialState);
        foreach(var child in this.GetChildren()){
            if(child is CutOut){
                this.CutOut = (CutOut)child;
                foreach(var child2 in this.CutOut.GetChildren()){
                    if(child2 is BearAnimationPlayer){
                        GD.Print("here");
                        this.AnimationPlayer = (BearAnimationPlayer)child2;}}}
            else if (child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;}
            else if (child is CameraManager){
                this.CameraManager = (CameraManager)child;}}
        for(int i=0; i<NUM_GLOBAL_ROTATIONS_TO_STORE; i++){
            this.OngoingGlobalRotations.Enqueue(this.GlobalRotation);}
        this.ATV = (ATV)this.GetParent();}

    public float GlobalRotationNormalized { get {
        return this.OngoingGlobalRotations.ToArray().Average();}}

    private Boolean isInAir = false;
    public Boolean IsInAir(){ 
        if(this.ActiveState == BearState.ON_ATV){
            return this.ATV.IsInAir();}
        else { return this.isInAir;}
    }

    public String InAirHitSequenceAnimString{ get { 
        if(this.ActiveHitSequence == BearHitSequence.SWIPED_OFF){
            return "die1_in_air";}
        if(this.ActiveHitSequence == BearHitSequence.SPIN_THEN_SPLAT){
            return "die2_in_air";}
        else{
            return "die1_in_air";}}}
    public String OnGroundHitSequenceAnimString{ get { 
        if(this.ActiveHitSequence == BearHitSequence.SWIPED_OFF){
            return "die1_on_ground";}
        if(this.ActiveHitSequence == BearHitSequence.SPIN_THEN_SPLAT){
            return "die2_on_ground";}
        else{
            return "die1_on_ground";}}}


    private int numFlipsWhileInvincible = 0;

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
                //this.AnimationPlayer.AdvancedPlay(this.InAirHitSequenceAnimString);
                SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
                    new string[] {WHIMPER1_SAMPLE});
                foreach(var holdable in this.ATV.Player.AllHoldibles.ToArray()){
                    this.ATV.Player.DropHoldable(holdable);}
                break;
            case BearState.HIT_SEQ_FALL_OFF:
                this.applyGravity(delta);
                this.MoveAndSlide(this.velocity);
                this.velocity.x *= 0.95f;
                var collAggreg = 0f;
                var slideCount = this.GetSlideCount();
                if(slideCount == 0){
                    this.isInAir = true;
                    if(!this.AnimationPlayer.CurrentAnimation.Equals(this.OnGroundHitSequenceAnimString)){
                        this.AnimationPlayer.AdvancedPlay(this.InAirHitSequenceAnimString, skipIfAlreadyPlaying: true);}}
                else{
                    this.AnimationPlayer.AdvancedPlay(this.OnGroundHitSequenceAnimString, skipIfAlreadyPlaying: true);}
                for(int i=0;i<slideCount; i++){
                        collision = this.GetSlideCollision(i);
                        collAggreg += collision.Normal.Angle();
                        if(collision.Collider is IPlatform){
                            if(this.isInAir){
                                this.PlayThudSound();}
                            this.isInAir = false;
                            this.velocity.x *= 0.9f;
                            this.velocity.y *= 0.9f;}}
                if(collAggreg != 0f){
                    this.GlobalRotation = (collAggreg / this.GetSlideCount()) + ((float)Math.PI / 2f);
                    this.manageOngoingGlobalRotations(delta);
                    this.GlobalRotation = this.GlobalRotationNormalized;}
                break;
            case BearState.HIT_SEQ_TRIGGER_TEMP_INVINC:
                this.ATV.ReattachBear();
                this.SetActiveState(BearState.HIT_SEQ_INVINC, 300);
                this.AnimationPlayer.Stop();
                this.AnimationPlayer.AdvancedPlay("invinc2");
                this.numFlipsWhileInvincible = 0;            
                break;
            case BearState.HIT_SEQ_INVINC:
                this.velocity.x = 0f;
                this.velocity.y = 0f;
                if(this.ATV.IsBearSmushedUnderATV()){
                    this.numFlipsWhileInvincible++;
                    if(this.numFlipsWhileInvincible > 2){
                        this.TriggerHitSequence();
                    } else {
                        this.ATV.FlipUpsideDown();
                    }
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
                if(collision != null && (collision.Collider is IPlatform)) {
                    this.TriggerHitSequence(BearHitSequence.SPIN_THEN_SPLAT);
                    ((LevelNode2D)this.ATV.Player.ActiveLevel).TriggerHitFlash();}
                break;
            default:
                throw new Exception("Bear must have a valid state");
        }
    }

    public Boolean HitSeqTriggeredThisFrame = false;
    private Boolean oneFrameDelayHitSeq = false;
    public void TriggerHitSequence(BearHitSequence hitSequence = BearHitSequence.SPIN_THEN_SPLAT,
                                   float throwSpeed = 500, 
                                   float hitUnits = Player.DEFAULT_HIT_UNIT){
        if(this.ActiveState == BearState.ON_ATV){
            this.ActiveHitSequence = hitSequence;
            this.ATV.Player.Health -= hitUnits;
            this.HitSeqTriggeredThisFrame = true;
            this.oneFrameDelayHitSeq = true;

            if(hitSequence.Equals(BearHitSequence.STAY_ON_ATV_1)){
                SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
                    new string[] {WHIMPER1_SAMPLE}); 
               this.AnimationPlayer.AdvancedPlay("get_hit1");
               this.ATV.ApplyPhonyRunOverEffect();
               this.numFlipsWhileInvincible = 0;
               this.SetActiveState(BearState.HIT_SEQ_INVINC, 100);
               this.SetActiveStateAfter(BearState.TRIGGER_END_HIT_SEQ, 100, 1f);
               foreach(var holdable in this.ATV.Player.AllHoldibles.ToArray()){
                    this.ATV.Player.DropHoldable(holdable);}
 
            } else {
                this.SetActiveState(BearState.TRIGGER_HIT_SEQUENCE, 100);
                this.ATV.ThrowBearOffATV(throwSpeed);}
        }
    }

    public override void ReactStateless(float delta){
        this.checkAndUpdateOneWayPlatformCollisions(delta);
        this.drawBear(delta);
        this.updateHitSeqTriggeredPublicFlag(delta);

    }

    private void updateHitSeqTriggeredPublicFlag(float delta){
        // I am a horrible programmer and I hate this hack
        if(this.HitSeqTriggeredThisFrame){
            if(this.oneFrameDelayHitSeq){
                this.oneFrameDelayHitSeq = false;
                return;}
            this.HitSeqTriggeredThisFrame = false;
        }

    }

    public override void UpdateState(float delta){}

    private void manageOngoingGlobalRotations(float delta){
        this.OngoingGlobalRotations.Dequeue();
        this.OngoingGlobalRotations.Enqueue(this.GlobalRotation);}

    private void checkAndUpdateOneWayPlatformCollisions(float delta){
        if(!this.ATV.Player.ActiveState.Equals(PlayerState.ALIVE)){
            return;}
        if(this.ATV.IsInAirNormalized() || this.ATV.IsInAir()){
            this.SetCollisionMaskBit(12, false);}
        else{
            this.SetCollisionMaskBit(12, this.ATV.FrontWheel.GetCollisionMaskBit(12));}
    }

    private void updateOneWayCollisionMasks(Boolean collide){
        this.SetCollisionMaskBit(12, collide);
    }


    private void drawBear(float delta){
        return;
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
        THUD1_SAMPLE,
        SkipIfAlreadyPlaying: true,
        MaxNumberTimesPlayPerSecond: MAX_THUD_FREQ_PER_SEC);
    }
}