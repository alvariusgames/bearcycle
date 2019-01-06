using Godot;
using System;

public class SafetyCheckpoint : CollisionShape2D
{
    public Vector2 GlobalPositionToResetTo;
    public override void _Ready(){
        this.GlobalPositionToResetTo = this.GetGlobalPosition();
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
