using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum ATVState {WITH_BEAR, WITHOUT_BEAR}

public enum ATVDirection{FORWARD, BACKWARD}

public class ATV : FSMNode2D<ATVState> {
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override ATVState InitialState { get { return ATVState.WITH_BEAR; }}
    public ATVDirection Direction { get; private set;}
    public Wheel FrontWheel;
    public Wheel BackWheel;
    private Vector2 bodyCenter;
    private Vector2 BackToFrontVector;
    public Bear Bear;
    public Player Player;
    public float BodyLength;
    private int LastInAirsLength = 15;
    private Queue<Boolean> OngoingIsInAirs = new Queue<Boolean>();
       public override void _Ready(){
        this.ResetActiveState(this.InitialState);
        foreach(Node2D child in this.GetChildren()){
            if(child.Name.Equals("FrontWheel")){
                this.FrontWheel = (Wheel)child;}
            else if(child.Name.Equals("BackWheel")){
                this.BackWheel = (Wheel)child;}
            else if(child.Name.Equals("Bear")){
                this.Bear = (Bear)child;}}
        this.Player = (Player)this.GetParent();
        this.BodyLength = this.FrontWheel.Position.DistanceTo(
            this.BackWheel.Position);
        for(int i=0; i<this.LastInAirsLength; i++){
            this.OngoingIsInAirs.Enqueue(false);
        }}

    public Vector2 GetGlobalCenterOfTwoWheels(){
        return (this.FrontWheel.GetGlobalPosition() + this.BackWheel.GetGlobalPosition()) / 2f;}
    
    public Vector2 GetNormalizedBackToFront(){
        return (this.FrontWheel.GetGlobalPosition() - this.BackWheel.GetGlobalPosition()).Normalized();
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

    public void AdjustVelocityAndAccelOfTwoWheels(
        float velocityMultiplier, float accellMultiplier){
            this.FrontWheel.AdjustVelocityAndAccell(velocityMultiplier, accellMultiplier);
            this.BackWheel.AdjustVelocityAndAccell(velocityMultiplier, accellMultiplier);
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

    public Boolean IsInAirNormalized(){
       return this.OngoingIsInAirs.ToArray().All(x => x == true);}

    public void RotateTwoWheels(float phi){
        phi = -phi; //My math was backwards, too lazy to fix...
        var center = this.GetGlobalCenterOfTwoWheels();
        var front = this.FrontWheel.GetGlobalPosition();
        var back = this.BackWheel.GetGlobalPosition();

        var centerToFront = (front - center);
        var centerToBack = (back - center);

        var rotatedCenterToFront = centerToFront.Rotated(phi);
        var rotatedCenterToBack = centerToBack.Rotated(phi);

        var newFront = center + rotatedCenterToFront;
        this.FrontWheel.SetGlobalPosition(newFront);
        GD.Print("Front from " + front + " to " + newFront);
        var newBack = center + rotatedCenterToBack;
        GD.Print("Back from " + back + " to " + newBack);
        this.BackWheel.SetGlobalPosition(newBack);

        this.moveBearToCenter(-1);

    }

    public void ReattachBear(){
        this.Bear.SetActiveState(BearState.ON_ATV, 100);
        this.moveBearToCenter(-1);
        if(this.Bear.MoveAndCollide(new Vector2(0,0)) != null){
            //if ATV appears to be flipped over, flip it over
            var swizzle = this.FrontWheel.GetGlobalPosition();
            this.FrontWheel.SetGlobalPosition(this.BackWheel.GetGlobalPosition());
            this.BackWheel.SetGlobalPosition(swizzle);
            this.moveBearToCenter(-1);}
        if(this.Bear.MoveAndCollide(new Vector2(0,0)) != null){
            //ATV attempted to be flipped, but failed
            GD.Print("Failed to flip!");
            this.Player.GoToMostRecentSafetyCheckPoint();
        }
            this.SetActiveState(ATVState.WITH_BEAR, 100);
            this.FrontWheel.ResetActiveState(WheelState.IDLING);
            this.BackWheel.ResetActiveState(WheelState.IDLING);}

    public void ThrowBearOffATV(float throwSpeed = 100){
        this.SetActiveState(ATVState.WITHOUT_BEAR, 100);
        if(this.FrontWheel.velocity.x >= 0f){
            this.Bear.velocity += (new Vector2(throwSpeed,0)).Rotated(1.25f * (float)Math.PI);
        }
        else{
            this.Bear.velocity -= (new Vector2(throwSpeed, 0)).Rotated(0.75f * (float)Math.PI);
        }
    }

    public override void UpdateState(float delta){
        if(this.FrontWheel.ActiveState == WheelState.ACCELERATING){
            this.Direction = ATVDirection.FORWARD;
        }
        else if(this.FrontWheel.ActiveState == WheelState.DECELERATING){
            this.Direction = ATVDirection.BACKWARD;
        }
    }

    public override void ReactStateless(float delta){
        this.holdWheelsTogether(delta);
        this.updateLastInAirs(delta);
    }

    private void updateLastInAirs(float delta){
        this.OngoingIsInAirs.Dequeue();
        var inAir = this.IsInAir();
        this.OngoingIsInAirs.Enqueue(inAir);
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
        var fcenter = this.FrontWheel.GetGlobalPosition();
        var bcenter = this.BackWheel.GetGlobalPosition();
        var actualBodyLength =  this.FrontWheel.GetGlobalPosition().DistanceTo(this.BackWheel.GetGlobalPosition());

        var fBodyEndCoord = fcenter - ((fcenter - bcenter).Normalized()) * (this.BodyLength + actualBodyLength) / 2f;
        var bBodyEndCoor = bcenter - ((bcenter - fcenter).Normalized()) * (this.BodyLength + actualBodyLength) / 2f;
        this.FrontWheel.SetGlobalPosition(bBodyEndCoor);
        this.BackWheel.SetGlobalPosition(fBodyEndCoord);
    }
    private void moveBearToCenter(float delta){
        var fcenter = this.FrontWheel.GetGlobalPosition();
        var bcenter = this.BackWheel.GetGlobalPosition();
        var center = (bcenter + fcenter) / 2f;
        var angleUpCenter = (fcenter - bcenter).Rotated(3f * (float)Math.PI / 2f).Normalized();
        var distAbove = 20f;
        var bearCenter = center + angleUpCenter * distAbove;
        
        //this.Bear.SetGlobalPosition(bearCenter / 90);
        this.Bear.Sprite.SetGlobalRotation((fcenter - bcenter).Angle());
        this.Bear.SetGlobalPosition(bearCenter);
    }
}


