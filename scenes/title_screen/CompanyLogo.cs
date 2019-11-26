using Godot;
using System;

public class CompanyLogo : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
     public override void _Process(float delta){
        SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                                     ToSceneStr: "res://scenes/title_screen/title_screen_press_start.tscn",
                                     effect: SceneTransitionEffect.FADE_BLACK,
                                     numSeconds: 2f);
 
  }
//  {
//      
//  }
}
