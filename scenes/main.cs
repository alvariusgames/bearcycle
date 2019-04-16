using Godot;
using System;

public class main : Node2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready(){
    }

    public override void _Process(float delta){
        if(Input.IsActionJustPressed("ui_accept")){
            GetTree().ChangeScene("res://scenes/frames/LevelFrame.tscn");
        }
    }
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
