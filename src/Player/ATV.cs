using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum ATVState {WITH_BEAR, WITHOUT_BEAR}

public enum ATVDirection{FORWARD, BACKWARD}

public class ATV : ManualEvocableFSMNode2D<ATVState> {
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override ATVState InitialState { get { return ATVState.WITH_BEAR;}set{}}
    public ATVDirection Direction { get; private set;}
    public Wheel FrontWheel;
    public Wheel BackWheel;
    private Vector2 bodyCenter;
    private Vector2 BackToFrontVector;
    public Bear Bear;
    public FallThroughManager FallThroughManager;
    public RotationManager RotationManager;
    public Player Player;
    public float BodyLength;
    public Vector2 CurrentNormal;
	public Sprite Sprite;
    private int LastInAirsLength = 15;
    private Queue<Boolean> OngoingIsInAirs = new Queue<Boolean>();
    private int LastVelocitiesOfTwoWheelsLength = 15;
    private float numSecOfInAir = 0f;
    private Queue<Vector2> OngoingVelocitiesOfTwoWheels = new Queue<Vector2>();
    private Vector2 initialOffsetFromWheelsToBear;
    private Particles2D RocketExhaustLocal;
    private Particles2D RocketExhaustGlobal;
    private Vector2 InitialLocalRocketExhaustPosition = new Vector2(0,0);
    private Vector2 InitialGlobalRocketExhaustPosition = new Vector2(0,0);
       public override void _Ready(){
        this.ResetActiveState(this.InitialState);
        foreach(Node2D child in this.GetChildren()){
            if(child.Name.Equals("FrontWheel")){
                this.FrontWheel = (Wheel)child;}
            else if(child.Name.Equals("BackWheel")){
                this.BackWheel = (Wheel)child;}
            else if(child.Name.Equals("Bear")){
                this.Bear = (Bear)child;}
            else if(child is Sprite){
                this.Sprite = (Sprite)child;}
            else if(child is FallThroughManager){
                this.FallThroughManager = (FallThroughManager)child;}
            else if(child is RotationManager){
                this.RotationManager = (RotationManager)child;}
            else if(child is Particles2D && child.Name.ToLower().Contains("rocket")){
                if(child.Name.ToLower().Contains("local")){
                    this.RocketExhaustLocal = (Particles2D)child;
                    this.InitialLocalRocketExhaustPosition = this.RocketExhaustLocal.Position;}
                if(child.Name.ToLower().Contains("global")){
                    this.RocketExhaustGlobal = (Particles2D)child;
                    this.InitialGlobalRocketExhaustPosition = this.RocketExhaustGlobal.Position;}}}
        this.Player = (Player)this.GetParent();
        this.BodyLength = this.FrontWheel.Position.DistanceTo(
            this.BackWheel.Position);
        for(int i=0; i<this.LastInAirsLength; i++){
            this.OngoingIsInAirs.Enqueue(false);
        }
        for(int i=0; i<this.LastVelocitiesOfTwoWheelsLength; i++){
            this.OngoingVelocitiesOfTwoWheels.Enqueue(new Vector2(0,0));}
        this.initialOffsetFromWheelsToBear = this.Bear.GlobalPosition - this.GetGlobalCenterOfTwoWheels();
        }

    public Vector2 GetDeFactoGlobalPosition(){
        //TODO: Find a better way to do this besides this hack
        return this.Sprite.GetGlobalPosition();
    }

    public float GetDeFactorGlobalRotation(){
        return this.GetNormalizedBackToFront().Angle();
    }

    public Vector2 GetGlobalCenterOfTwoWheels(){
        return (this.FrontWheel.GetGlobalPosition() + this.BackWheel.GetGlobalPosition()) / 2f;}

    public Vector2 GetGlobalCenterOfATV(){
        var t = this.initialOffsetFromWheelsToBear.Rotated(this.CurrentNormal.Angle() + ((float)Math.PI / 2f));
        return this.GetGlobalCenterOfTwoWheels() + t;//this.initialOffsetFromWheelsToBear * this.CurrentNormal.Rotated((float)Math.PI);
    }
    
    public Vector2 GetNormalizedBackToFront(){
        return (this.FrontWheel.GetGlobalPosition() - this.BackWheel.GetGlobalPosition()).Normalized();
    }

    public Vector2 GetNormalizedForwardDirection(){
        if(this.Direction.Equals(ATVDirection.FORWARD)){
            return this.GetNormalizedBackToFront();}
        else if(this.Direction.Equals(ATVDirection.BACKWARD)){
            return this.GetNormalizedBackToFront().Rotated((float)Math.PI);}
        else{
            return new Vector2(0,0);}
    }

    public void SetGlobalCenterOfTwoWheels(Vector2 globalCenter, float verticalOffset=30){
        this.BackWheel.SetGlobalPosition(new Vector2(globalCenter.x, 
                                                     globalCenter.y - verticalOffset));
        this.FrontWheel.SetGlobalPosition(new Vector2(globalCenter.x + this.BodyLength,
                                                   globalCenter.y - verticalOffset));
        this.holdWheelsTogether(-1);
        this.moveBearToCenter(-1);
    }

    public Vector2 GetVelocityOfTwoWheels(){
        return (this.FrontWheel.velocity + this.BackWheel.velocity) / 2f;
    }

    public Vector2 GetRecentAverageVelocityOfTwoWheels(){
        var arr = this.OngoingVelocitiesOfTwoWheels.ToArray();
        var sum = new Vector2(0,0);
        foreach(var vec in arr){
            sum += vec;
        }
        return sum / arr.Length;
    }

    public void SetVelocityOfTwoWheels(Vector2 velocity){
        this.FrontWheel.velocity = velocity;
        this.BackWheel.velocity = velocity;
    }

    public float GetAccellOfTwoWheels() {
        return (this.FrontWheel.forwardAccell + this.BackWheel.forwardAccell) / 2f;}

    public void SetAccellOfTwoWheels(float accell){
        this.FrontWheel.forwardAccell = accell;
        this.BackWheel.forwardAccell = accell;
    }

    public Boolean IsInAir(){
        return this.FrontWheel.IsInAir() && this.BackWheel.IsInAir();
    }

    public Boolean IsInAirNormalized(float numSecondsToCheckInAir = -1){
        if(numSecondsToCheckInAir == -1){
            numSecondsToCheckInAir = (1 / main.DELTA_NORMALIZER) * (float)this.LastInAirsLength;}
        return this.numSecOfInAir >= numSecondsToCheckInAir;}

    public void RotateTwoWheels(float phi, float delta){
        phi *= delta * main.DELTA_NORMALIZER;
        var front = this.FrontWheel.GetGlobalPosition();
        var back = this.BackWheel.GetGlobalPosition();
        var center = this.GetGlobalCenterOfTwoWheels();

        var centerToFront = (front - center);
        var centerToBack = (back - center);

        var rotatedCenterToFront = centerToFront.Rotated(phi);
        var rotatedCenterToBack = centerToBack.Rotated(phi);

        var newFront = center + rotatedCenterToFront;
        this.FrontWheel.SetGlobalPosition(newFront);
        var newBack = center + rotatedCenterToBack;
        this.BackWheel.SetGlobalPosition(newBack);
    }

    public void ReattachBear(){
        this.Bear.SetActiveState(BearState.ON_ATV, 100);
        this.moveBearToCenter(-1);
        if(this.IsBearSmushedUnderATV()){
            //if ATV appears to be flipped over, flip it over
            this.FlipUpsideDown();}
        if(this.IsBearSmushedUnderATV()){
            //ATV attempted to be flipped, but failed
            GD.Print("Failed to flip!");
            this.Player.GoToMostRecentSafetyCheckPoint();}
        this.SetActiveState(ATVState.WITH_BEAR, 100);
        this.FrontWheel.ResetActiveState(WheelState.IDLING);
        this.BackWheel.ResetActiveState(WheelState.IDLING);}

    public bool IsBearSmushedUnderATV(){
        var collision = this.Bear.MoveAndCollide(new Vector2(0,0));
        return (collision != null) && (collision.Collider is IPlatform);
    }

    public void FlipUpsideDown(){
            var swizzle = this.FrontWheel.GetGlobalPosition();
            this.FrontWheel.SetGlobalPosition(this.BackWheel.GetGlobalPosition());
            this.BackWheel.SetGlobalPosition(swizzle);
            this.drawATV(-1);
            this.moveBearToCenter(-1);
    }

    public void ThrowBearOffATV(float throwSpeed = 500){
        this.SetActiveState(ATVState.WITHOUT_BEAR, 100);
        if(this.FrontWheel.velocity.x >= 0f){
            this.Bear.velocity += (new Vector2(throwSpeed,0)).Rotated(1.25f * (float)Math.PI);
        }
        else{
            this.Bear.velocity -= (new Vector2(throwSpeed, 0)).Rotated(0.75f * (float)Math.PI);
        }
    }

    public void CancelAllRotationalEnergy(){
        var avg = (this.FrontWheel.velocity + this.BackWheel.velocity) / 2f;
        this.SetVelocityOfTwoWheels(avg);
    }

    public void CancelAllForwardTwoWheelEnergy(){
        if(this.FrontWheel.forwardAccell > 0f){
            this.FrontWheel.forwardAccell = 0f;
            this.FrontWheel.ResetActiveState(WheelState.IDLING);}
        if (this.BackWheel.forwardAccell > 0f){
            this.BackWheel.forwardAccell = 0f;
            this.BackWheel.ResetActiveState(WheelState.IDLING);}}

    public void CancelAllBackwardTwoWheelEnergy(){
        if(this.FrontWheel.forwardAccell < 0f){
            this.FrontWheel.forwardAccell = 0f;
            this.FrontWheel.ResetActiveState(WheelState.IDLING);}
        if (this.BackWheel.forwardAccell < 0f){
            this.BackWheel.forwardAccell = 0f;
            this.BackWheel.ResetActiveState(WheelState.IDLING);}}

    public override void UpdateState(float delta){
        this.UpdateDirection(delta);}
    
    private void UpdateDirection(float delta){
        if((this.FrontWheel.ActiveState == WheelState.ACCELERATING || Input.IsActionPressed("ui_right"))
                && this.Player.AttackWindow.ActiveState == AttackWindowState.NOT_ATTACKING
                && !this.IsInAir()
                && this.ActiveState == ATVState.WITH_BEAR
                && this.Player.ActiveState == PlayerState.ALIVE){
            this.Direction = ATVDirection.FORWARD;
        }
        else if((this.FrontWheel.ActiveState == WheelState.DECELERATING || Input.IsActionPressed("ui_left")) 
                && this.Player.AttackWindow.ActiveState == AttackWindowState.NOT_ATTACKING
                && !this.IsInAir()
                && this.ActiveState == ATVState.WITH_BEAR
                && this.Player.ActiveState == PlayerState.ALIVE){
            this.Direction = ATVDirection.BACKWARD;}}

    public override void ReactStateless(float delta){
        this.holdWheelsTogether(delta);
        this.updateLastVelocitiesOfTwoWheels(delta);
        this.updateLastInAirs(delta);
        this.drawATV(delta);
    }

    private void updateLastVelocitiesOfTwoWheels(float delta){
        this.OngoingVelocitiesOfTwoWheels.Dequeue();
        var vel = this.GetVelocityOfTwoWheels();
        this.OngoingVelocitiesOfTwoWheels.Enqueue(vel);}

    private void updateLastInAirs(float delta){
        this.OngoingIsInAirs.Dequeue();
        var inAir = this.IsInAir();
        this.OngoingIsInAirs.Enqueue(inAir);
        if(inAir){
            this.numSecOfInAir += delta; }
        else {
            this.numSecOfInAir = 0f;}
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case ATVState.WITH_BEAR:
                moveBearToCenter(delta);
                break;
            case ATVState.WITHOUT_BEAR:
                this.FrontWheel.SetActiveState(WheelState.LOCKED, 200);
                this.BackWheel.SetActiveState(WheelState.LOCKED, 200);
                break;
            default:
                throw new Exception("ATV must have state");
        }
    }
   
    private void holdWheelsTogether(float delta){
        //Do physics for a joint between frontwheel and backwheel
        var fcenter = this.FrontWheel.GlobalPosition;
        var bcenter = this.BackWheel.GlobalPosition;
        var actualBodyLength =  this.FrontWheel.GlobalPosition.DistanceTo(this.BackWheel.GlobalPosition);

        var fBodyEndCoord = fcenter - ((fcenter - bcenter).Normalized()) * (this.BodyLength + actualBodyLength) / 2f;
        var bBodyEndCoor = bcenter - ((bcenter - fcenter).Normalized()) * (this.BodyLength + actualBodyLength) / 2f;

        this.FrontWheel.GlobalPosition = bBodyEndCoor;
        this.BackWheel.GlobalPosition = fBodyEndCoord;
}


    private void moveBearToCenter(float delta){
        var fcenter = this.FrontWheel.GetGlobalPosition();
        var bcenter = this.BackWheel.GetGlobalPosition();
        var center = (bcenter + fcenter) / 2f;
        var angleUpCenter = (fcenter - bcenter).Rotated(3f * (float)Math.PI / 2f).Normalized();
        this.CurrentNormal = angleUpCenter;
        this.Bear.GlobalPosition = this.GetGlobalCenterOfATV();
        this.Bear.SetGlobalRotation((fcenter - bcenter).Angle());
    }

    private Vector2 prevATVPosition;

    public void drawATV(float delta){
        var fwcenter = this.FrontWheel.GetGlobalPosition();
        var bwcenter = this.BackWheel.GetGlobalPosition();
        var bcenter = this.Bear.GetGlobalPosition();
        var center = (bwcenter + fwcenter) / 2f;

        var angleUpCenter = (fwcenter - bwcenter).Rotated(
            3f * (float)Math.PI / 2f).Normalized();
        var distAbove = 40f;
        var atvPos = center + distAbove * angleUpCenter;

        //linear interpolate BS here
        var fraction = Engine.GetPhysicsInterpolationFraction();
        this.Sprite.GlobalPosition = this.prevATVPosition.LinearInterpolate(atvPos, fraction);
        this.prevATVPosition = atvPos;
        //end linear interpolate 

        this.Sprite.SetGlobalRotation((fwcenter - bwcenter).Angle());

        var spriteScale = this.Sprite.GetScale();
        if(this.Direction == ATVDirection.FORWARD){
            this.Sprite.SetScale(new Vector2(Math.Abs(spriteScale[0]),
                                             spriteScale[1]));
        } else if(this.Direction == ATVDirection.BACKWARD){
            this.Sprite.SetScale(new Vector2(-Math.Abs(spriteScale[0]),
                                             spriteScale[1])) ;
        }
        this.drawRocketExhaust(atvPos);
    }

    private void drawRocketExhaust(Vector2 atvPos){
        return;
        RocketBooster rocketBooster = null;
        if(this.Player.ActiveHoldable is RocketBooster){
            rocketBooster = ((RocketBooster)this.Player.ActiveHoldable);}
        if(rocketBooster != null && rocketBooster.IsEmitting){
            this.RocketExhaustLocal.Emitting = true;
            this.RocketExhaustGlobal.Emitting = true;}
        else{
            this.RocketExhaustLocal.Emitting = false;
            this.RocketExhaustGlobal.Emitting = false;}
        if(!this.IsInAirNormalized()){
            this.RocketExhaustGlobal.Emitting = false;
        }
        var vel2WheelNorm = this.GetVelocityOfTwoWheels().Normalized();    
        var normalizedForwardDir = this.GetNormalizedForwardDirection();
        var normalizedBackwardDir = normalizedForwardDir.Rotated((float)Math.PI);
        var globalPosShift = 20f;
        if(rocketBooster != null){
            globalPosShift += (rocketBooster.numSecondsBeingHeld / rocketBooster.PseudoImpulseEffectSec) * 20;}
        this.RocketExhaustLocal.GlobalPosition = atvPos + this.InitialLocalRocketExhaustPosition.Length() * normalizedBackwardDir;
        this.RocketExhaustGlobal.GlobalPosition = atvPos + (this.InitialGlobalRocketExhaustPosition.Length() + globalPosShift) * normalizedBackwardDir ;
        var localDir = new Vector3(normalizedBackwardDir.x, normalizedBackwardDir.y, 0);
        var globalDir = new Vector3(vel2WheelNorm.x, vel2WheelNorm.y, 0);
        ((ParticlesMaterial)this.RocketExhaustLocal.ProcessMaterial).Direction = localDir;
        ((ParticlesMaterial)this.RocketExhaustGlobal.ProcessMaterial).Direction = globalDir;
        ((ParticlesMaterial)this.RocketExhaustLocal.ProcessMaterial).InitialVelocity = 0.1f * this.GetVelocityOfTwoWheels().Length();
        ((ParticlesMaterial)this.RocketExhaustGlobal.ProcessMaterial).InitialVelocity = 1f * this.GetVelocityOfTwoWheels().Length();
    }

    public void tempStopAllMovement(Boolean exceptGravity = false){
        this.FrontWheel.tempStopAllMovement(exceptGravity);
        this.BackWheel.tempStopAllMovement(exceptGravity);
    }

    public void resumeMovement(){
        this.FrontWheel.resumeMovement();
        this.BackWheel.resumeMovement();
    }

    public override void _Process(float delta){}

    public  void _ManualProcess(float delta) {
        this.UpdateState(delta);
        this.ReactStateless(delta);
        this.handleTimers(delta);
        this.ReactToState(delta);}

    public void ApplyPhonyRunOverEffect(){
        ////Simulates an ATV squishing something below it
        if(this.Direction.Equals(ATVDirection.FORWARD)){
            this.FrontWheel.PhonyBounceUp(0.5f);
            this.BackWheel.PhonyBounceUp(0.25f);
        } else {
            this.FrontWheel.PhonyBounceUp(0.25f);
            this.BackWheel.PhonyBounceUp(0.5f);
        }        
    }
}