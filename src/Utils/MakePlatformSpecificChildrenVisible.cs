using Godot;
using System;

public class MakePlatformSpecificChildrenVisible : PlatformSpecificChildren
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        foreach(Node2D child in this.GetChildren()){
            child.Visible = false;}
        this.GetPlatformSpecificChildren(); // This makes the children visible (hacky)
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
