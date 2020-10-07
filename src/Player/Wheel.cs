using Godot;
using System;

public enum WheelState {
    ACCELERATING,
    DECELERATING,
    IDLING,
    LOCKED}

public class Wheel : FSMKinematicBody2DBasicPhysics<WheelState>{
    public override WheelState InitialState { get { return WheelState.IDLING;}set{}}
    public Vector2 userControlledVelocity = new Vector2(0,0);
    public Sprite sprite;
    public CollisionShape2D collisionShape2D;
    public ATV ATV;
    public const float ONE_WAY_VERT_VELOCITY_THRESH = 0f;
    public const float MAX_FORWARD_ACCEL = 130f;
    public override float MaxForwardAccel {get; set;} = MAX_FORWARD_ACCEL;
    private const float DECELL_EFFECT = 0.90f;
    private const float LOCKING_EFFECT = 0.90f;
    ///when speedbosting, what slowdown effect to go back down to normal speed
    private const float SPEED_BOOST_SLOWDOWN_EFFECT = 0.995f;
    private const float BOUNCE_UP_EFFECT_VELOCITY = MAX_SPEED / 4; 
    private Boolean stopAllMovement = false;
    private Boolean stopAllMovementExceptGravity = false;
    private Boolean stopAllEngineSounds = false;
    private Wheel otherWheel;
    private Wheel OtherWheel {get {
        if(this.otherWheel == null){
            if(this.Name.ToLower().Contains("front")){
                this.otherWheel = this.ATV.BackWheel;
            } else {
                this.otherWheel = this.ATV.FrontWheel;}}
        return this.otherWheel;}}

    public override void _Ready(){
        base._Ready();
        this.ResetActiveState(this.InitialState);
        for(int i = 0; i<this.GetChildCount(); i++){
            var child = this.GetChild(i);
            if(child is Sprite && child.Name.ToLower().Contains("sprite")){
                this.sprite = (Sprite)child;}
            if(child is CollisionShape2D){
                this.collisionShape2D = (CollisionShape2D)child;}}
        this.ATV = (ATV)this.GetParent();
        this.SetActiveState(WheelState.IDLING, 100);}

    public override void UpdateState(float delta){
        this.reactToInput(delta);}

    const string ENGINE_GO_NORMAL_SAMPLE = "res://media/samples/atv/engine_go_normal1.wav";
    const string ENGINE_REV_SAMPLE = "res://media/samples/atv/engine_rev1.wav";
    const string WHEEL_THUD1_SAMPLE = "res://media/samples/atv/wheel_thud1.wav";

    public void playThudSound(){
        var rnd = new Random();
        var pitchScale = 0.75f + (float)(rnd.NextDouble() * 0.5f);
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
            new string[] {WHEEL_THUD1_SAMPLE},
            PitchScale: pitchScale,
            VolumeMultiplier: 0.5f,
            SkipIfAlreadyPlaying: true,
            MaxNumberTimesPlayPerSecond: 1);}

    private void stopAllEngineSound(){
        if(this.Name.ToLower().Equals("frontwheel")){
           SoundHandler.StopSample(this, ENGINE_GO_NORMAL_SAMPLE);
           SoundHandler.StopSample(this, ENGINE_REV_SAMPLE);}}

    public void PlayNormalEngineSound(){
        if(this.Name.ToLower().Equals("frontwheel")){
            var pitchScale = 1f + Math.Abs(forwardAccell / (MaxForwardAccel * 2.5f));
            var volumeMultipler = 0.3f + Math.Abs(forwardAccell / (MaxForwardAccel * 2));
            SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
                    new string[] {ENGINE_GO_NORMAL_SAMPLE},
                    PitchScale: pitchScale,
                    VolumeMultiplier: volumeMultipler,
                    Loop: true,
                    SkipIfAlreadyPlaying: true);}}

    public void StopAllEngineSounds(){
        this.stopAllEngineSounds = true;}

    public void PlayIdleEngineSound(){
        if(this.Name.ToLower().Equals("frontwheel")){
            SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
                    new string[] {ENGINE_GO_NORMAL_SAMPLE},
                    VolumeMultiplier: 0.5f,
                    PitchScale: 1f,
                    Loop: true,
                    SkipIfAlreadyPlaying: true);}}

    public void PlayEngineRevSound(){
        if(this.Name.ToLower().Equals("frontwheel")){
            SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, 
                    new string[] {ENGINE_REV_SAMPLE},
                    VolumeMultiplier: 0.65f);}}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case WheelState.ACCELERATING:
                if(this.forwardAccell <= MaxForwardAccel){  
                    this.forwardAccell += ForwardAccelUnit * delta * main.DELTA_NORMALIZER;
                } else {
                    //we're in 'speed boost mode', slow us down a bit
                    this.forwardAccell *= SPEED_BOOST_SLOWDOWN_EFFECT;}
 
                //this.forwardAccell *= 0.95f; 
                reactToSlideCollision(delta);
                break;
            case WheelState.DECELERATING:
                if(this.forwardAccell >= MaxBackwardAccel){
                    this.forwardAccell -= ForwardAccelUnit * delta * main.DELTA_NORMALIZER;
                } else {
                    //we're in 'speed boost mode', slow us down a bit
                    this.forwardAccell *= SPEED_BOOST_SLOWDOWN_EFFECT;}
                //this.forwardAccell *= 0.95f;
                reactToSlideCollision(delta);
                break;
            case WheelState.IDLING:
                this.forwardAccell *= DECELL_EFFECT;
                reactToSlideCollision(delta);
                break;
            case WheelState.LOCKED:
                this.forwardAccell *= LOCKING_EFFECT;
                reactToSlideCollision(delta, 0.4f);
                break;
            default:
                throw new Exception("Wheel invalid state.");}}

    public override void OnCollisionWith(Godot.Object collider){
        if(collider is IPlatform){
            if(this.ATV.IsInAirNormalized()){
                this.playThudSound();}}}

    public override void ReactStateless(float delta){
        this.applyGravity(delta);
        this.checkAndUpdateOneWayPlatformCollisions(delta);
        if(this.stopAllMovement){
            this.velocity = new Vector2(0,0);}
        if(this.stopAllMovementExceptGravity){
            this.velocity = new Vector2(0,this.velocity.y);}
        this.MoveAndSlide(linearVelocity: this.velocity, this.currentNormal);
        this.updateSprite(delta);
        this.applySound(delta);
    }

    private void checkAndUpdateOneWayPlatformCollisions(float delta){
        if(!this.ATV.Player.ActiveState.Equals(PlayerState.ALIVE) ||
           !this.ATV.FallThroughManager.ActiveState.Equals(FallThroughManagerState.NOT_FALLING_THROUGH)){
            return;}
       if(this.ATV.IsInAirNormalized(numSecondsToCheckInAir: 0.1f)){
            if(this.velocity.y > Wheel.ONE_WAY_VERT_VELOCITY_THRESH){
                this.updateOneWayCollisionMasks(true);
            } else {
                this.updateOneWayCollisionMasks(false);
            }
        } else {
            this.updateOneWayCollisionMasks(true);
        }
    }

    private void updateOneWayCollisionMasks(Boolean collide){
        this.SetCollisionMaskBit(Consts.L_PlatformOneWay, collide);
    }

    private void applySound(float delta){
        if(this.stopAllEngineSounds){
            return;}
        float AccellThreshForGoAudio = 5f;
        if(Input.IsActionPressed("ui_right") || 
           Input.IsActionPressed("ui_left") ||
           Math.Abs(this.forwardAccell) > AccellThreshForGoAudio){
               this.PlayNormalEngineSound();
        } else {
            this.PlayIdleEngineSound();}}

    private void reactToInput(float delta){
        if(!this.ATV.IsInAirNormalized()){
            ///Only change wheel state from input if on ground
            if (Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_right")){
                this.SetActiveState(WheelState.IDLING, 100);}
            else if (Input.IsActionPressed("ui_left")){
                this.SetActiveState(WheelState.DECELERATING, 100);}
            else if (Input.IsActionPressed("ui_right")){
                this.SetActiveState(WheelState.ACCELERATING, 100);}
            else{
                this.SetActiveState(WheelState.IDLING, 100);
            }
        }
    }

    public void tempStopAllMovement(Boolean exceptGravity = false){
        if(exceptGravity){
            this.stopAllMovementExceptGravity = true;}
        else{
            this.stopAllMovement = true;}}


    public void resumeMovement(){
        this.stopAllMovement = false;
        this.stopAllMovementExceptGravity = false;}


        public void PhonyBounceUp(float magnitude){
            this.velocity = new Vector2(this.currentNormal.x * magnitude * BOUNCE_UP_EFFECT_VELOCITY,
                                        this.currentNormal.y * magnitude * BOUNCE_UP_EFFECT_VELOCITY); 
        }


    public void AdjustVelocityAndAccell(float velocityMultiplier, float accellVelocityMultiplier){
        ///args: 1 means no change, 0.7f means 30% less, 2f means twice as fast, etc.
        this.velocity = this.velocity * velocityMultiplier;
        this.forwardAccell = this.forwardAccell * accellVelocityMultiplier;
    }


    private void updateSprite(float delta){
        float randJiggleEffect = 0f;
        if(this.Name == "FrontWheel" && this.forwardAccell != 0f){
            randJiggleEffect = (float)((new Random().NextDouble() - 0.45f) * 0.1);
        } else if(this.Name == "BackWheel" && this.forwardAccell != 0f){
            randJiggleEffect = (float)((new Random().NextDouble() - 0.55f) * 0.1);
        }
        const float arbitraryConstant = 400f;
        if(Math.Abs(this.forwardAccell) > 5f){
            this.sprite.Rotate((this.forwardAccell / arbitraryConstant) + randJiggleEffect);
        } else {
            this.sprite.Rotate(this.forwardAccell / arbitraryConstant);   
        }
    }

}
