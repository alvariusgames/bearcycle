using Godot;
using System;

public class Bear : KinematicBody2D
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    private const float GRAVITY  = 600.0f;
    private const float MAX_GRAVITY_SPEED = 300f;
    private const float FRICTION_EFFECT = 0.8f;

    public Vector2 velocity = new Vector2(0,0);
    public Sprite Sprite;

    public override void _Ready()
    {
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.Sprite = (Sprite)child;
            }
        }

    }
public override void _PhysicsProcess(float delta){
        this.processPhysics(delta);
        this.applyPhysics(delta);
    }

private void processPhysics(float delta){
        if(this.velocity.y <= MAX_GRAVITY_SPEED){        
            this.velocity.y += delta * GRAVITY;
        }
        this.velocity *= FRICTION_EFFECT;
    }

private void applyPhysics(float delta){
    this.MoveAndSlide(linearVelocity: this.velocity);
}
//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}

