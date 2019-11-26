using Godot;
using System;

public class EvokeMyParentNow : Node2D
{
  public override void _Process(float delta){
      //This is called AFTER our wheels have done their physics updates
      //needed to fix a bug
      var parent = (ATV)this.GetParent();
      parent._ManualProcess(delta);
  }
}
