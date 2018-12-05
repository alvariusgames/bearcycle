using Godot;
using System;

public class ATV : Node2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public Player frontWheel;
    public Player backWheel;
    public Body body;

    public override void _Ready()
    {
        foreach(Node child in this.GetChildren()){
            if(child is Player && child.Name.Contains("Front")){
                this.frontWheel = (Player)child;}
            else if(child is Player && child.Name.Contains("Back")){
                this.backWheel = (Player)child;}
            else if (child.Name.Contains("Body")){
                this.body = (Body)child;
            }
        }
    }

    public override void _Process(float delta){
        //GD.Print(this.Position);
    }
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
