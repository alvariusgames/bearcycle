using Godot;
using System;

public class ToggleSpaceViewCollider : KinematicBody2D{

    public LevelSelectView LevelSelectView;

    public override void _Ready(){
        this.LevelSelectView = (LevelSelectView)this.GetParent();}

  public override void _Process(float delta){
        var prevPos = this.GlobalPosition;
        var col = this.MoveAndCollide(new Vector2(0,0));
        this.GlobalPosition = prevPos;
        if(col != null && col.Collider is LevelSelectPlayer){
            this.LevelSelectView.TryToggleSpaceView();}}
}
