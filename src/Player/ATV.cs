using Godot;
using System;

public enum ATVState {WITH_BEAR, WITHOUT_BEAR}

public class ATV : FSMNode2D<ATVState> {
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public Wheel FrontWheel;
    public Wheel BackWheel;
    private Vector2 bodyCenter;
    private Vector2 BackToFrontVector;
    public Bear Bear;
    public float BodyLength;

    public Vector2 GetGlobalCenterOfTwoWheels(){
        return (this.FrontWheel.GetGlobalPosition() + this.BackWheel.GetGlobalPosition()) / 2f;
    }

    public void ReattachBear(){
        this.Bear.SetActiveState(BearState.ON_ATV, 100);
        this.moveBearToCenter(-1);
        if(this.Bear.MoveAndCollide(new Vector2(0,0)) != null){
            var swizzle = this.FrontWheel.GetGlobalPosition();
            this.FrontWheel.SetGlobalPosition(this.BackWheel.GetGlobalPosition());
            this.BackWheel.SetGlobalPosition(swizzle);
        }
        this.SetActiveState(ATVState.WITH_BEAR, 100);
        this.FrontWheel.ResetActiveState(WheelState.IDLING);
        this.BackWheel.ResetActiveState(WheelState.IDLING);

    }

    public override void _Ready()
    {
        foreach(Node2D child in this.GetChildren()){
            if(child.Name.Equals("FrontWheel")){
                this.FrontWheel = (Wheel)child;}
            else if(child.Name.Equals("BackWheel")){
                this.BackWheel = (Wheel)child;}
            else if(child.Name.Equals("Bear")){
                this.Bear = (Bear)child;}
        }
        this.BodyLength = this.FrontWheel.Position.DistanceTo(
            this.BackWheel.Position);
    }

    public void ThrowBearOffATV(float throwSpeed = 100){
        this.SetActiveState(ATVState.WITHOUT_BEAR, 100);
        if(this.FrontWheel.velocity.x >= 0f){
            this.Bear.velocity += (new Vector2(throwSpeed,0)).Rotated(1.25f * (float)Math.PI);
        }
        else{
            this.Bear.velocity -= (new Vector2(throwSpeed, 0)).Rotated(0.75f * (float)Math.PI);
        }
        //this.Bear.velocity += throwDirection * throwSpeed;
    }

    public override void UpdateState(float delta){
        
    }

    public override void ReactStateless(float delta){
        this.holdWheelsTogether(delta);
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
        var fBodyEndCoord = fcenter - ((fcenter - bcenter).Normalized()) * this.BodyLength;
        var bBodyEndCoor = bcenter - ((bcenter - fcenter).Normalized()) * this.BodyLength;
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


