using Godot;
using System;

public class TriggerBossFight : KinematicBody2D{
    public Boolean IsCurrentlyCollidingWithPlayer = false;
  public override void _Process(float delta){
      var coll = this.MoveAndCollide(new Vector2(0,0));
      if(coll != null && coll.Collider is WholeBodyKinBody){
          this.IsCurrentlyCollidingWithPlayer = true;
      } else {
          this.IsCurrentlyCollidingWithPlayer = false;}}
}
