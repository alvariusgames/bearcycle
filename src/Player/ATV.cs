using Godot;
using System;


public class ATV : Node2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public enum State {withBear=0, withoutBear=1 };
    public Wheel FrontWheel;
    public Wheel BackWheel;
    public Bear Bear;
    public State activeState = ATV.State.withBear;
    public float BodyLength;

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
        //GD.Print(this.BodyLength);
        // Called every time the node is added to the scene.
        // Initialization here
        
    }
    public override void _Process(float delta) {
        this.holdWheelsTogether(delta);
        if(this.activeState == ATV.State.withBear){
            this.moveBearToCenter(delta);
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

        GD.Print(angleUpCenter);
        GD.Print("----");
    }
}


