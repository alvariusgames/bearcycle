using Godot;
using System;

public class ATV : Node2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public Player FrontWheel;
    public Player BackWheel;
    public float BodyLength;

    public override void _Ready()
    {
        foreach(Node2D child in this.GetChildren()){
            if(child.Name.Equals("FrontWheel")){
                this.FrontWheel = (Player)child;}
            else if(child.Name.Equals("BackWheel")){
                this.BackWheel = (Player)child;
            }
        }
        this.BodyLength = this.FrontWheel.Position.DistanceTo(
            this.BackWheel.Position);
        //GD.Print(this.BodyLength);
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void _Process(float delta)
    {
        //Do physics for a joint between frontwheel and backwheel
        var fcenter = this.FrontWheel.getCenter();
        var bcenter = this.BackWheel.getCenter();
        var fBodyEndCoord = fcenter - ((fcenter - bcenter).Normalized()) * this.BodyLength;
        var bBodyEndCoor = bcenter - ((bcenter - fcenter).Normalized()) * this.BodyLength;
        GD.Print(this.FrontWheel.GetGlobalPosition());
        GD.Print(fcenter);
        GD.Print(bBodyEndCoor);
        GD.Print("----");
        this.FrontWheel.SetGlobalPosition(bBodyEndCoor);
        this.BackWheel.SetGlobalPosition(fBodyEndCoord);
    }
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
