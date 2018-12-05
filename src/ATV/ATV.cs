using Godot;
using System;

public class ATV : Node2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public Player FrontWheel;
    public Player BackWheel;

    public override void _Ready()
    {
        foreach(Node2D child in this.GetChildren()){
            if(child.Name.Equals("FrontWheel")){
                this.FrontWheel = (Player)child;}
            else if(child.Name.Equals("BackWheel")){
                this.BackWheel = (Player)child;
            }
        }
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void _Process(float delta)
    {
        //Do physics for a joint between frontwheel and backwheel
        GD.Print(this.FrontWheel.Position);
    }
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
