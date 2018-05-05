using Godot;
using System;

public class Player : KinematicBody2D
{

    public override void _PhysicsProcess(float delta)
    {
        // Move down 1 pixel per physics frame
        MoveAndCollide(new Vector2 (0,1));
    }
}