using Godot;
using System;

public class PlayArbitraryLevel : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        SceneTransitioner.TransitionToLevel(FromScene: this.GetTree().Root.GetChild(0),
                                            ToLevelStr: "res://scenes/levels/level1z2.tscn",
                                            LevelTitle: "",
                                            LevelZone: 0);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
