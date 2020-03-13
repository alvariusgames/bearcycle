using Godot;
using System;

public class RemoveWallWhenHealthIs100 : KinematicBody2D{
    public CollisionPolygon2D CollisionPolygon2D;
    public Vector2 InitialPos;
    public StaticBody2D StaticBody2D;

    public override void _Ready(){
        this.StaticBody2D = (StaticBody2D)this.GetParent();
        this.InitialPos = this.GlobalPosition;
        foreach(Node child in this.GetChildren()){
            if(child is CollisionPolygon2D){
                this.CollisionPolygon2D = (CollisionPolygon2D)child;}}}

  public override void _Process(float delta){
      var col = this.MoveAndCollide(new Vector2(0,0));
      this.GlobalPosition = this.InitialPos;
      if(col != null && col.Collider is WholeBodyKinBody){
          var player = ((WholeBodyKinBody)col.Collider).Player;
          if(player.CurrentHealth >= 0.93f * Player.MAX_HEALTH){
              if(this.StaticBody2D != null){
                this.StaticBody2D.GetParent().RemoveChild(this.StaticBody2D);
                this.StaticBody2D = null;}
              //this.CollisionPolygon2D.Disabled = true;
          }
      }

  }
}
