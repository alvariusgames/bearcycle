using Godot;
using System;

public class MakeMyParentRotate : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private int rotationDirection = 1;
    public override void _Ready()
    {
     if(this.Name.ToLower().Contains("backward")){
         this.rotationDirection = -1;
     }   
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta){
      var rotationSpeed = 0.01f;
      var parent = (Node2D)this.GetParent();
      parent.Rotate(this.rotationDirection * main.DELTA_NORMALIZER * delta * rotationSpeed);
  }
//  {
//      
//  }
}
