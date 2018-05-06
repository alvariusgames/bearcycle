using Godot;
using System;

public class Player : KinematicBody2D
{
    const float gravity = 200.0f;
    const int walk_speed = 200;

    Vector2 velocity = new Vector2(0,0);

    private float forwardSpeed = 0f;
    private Vector2 prevPosition;
    private float angleOfMovement = 0f;
    private Vector2 normalAngle;

    public override void _Ready(){
        prevPosition = this.Position;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("ui_left"))
        {
            this.forwardSpeed = -walk_speed;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            this.forwardSpeed = walk_speed;
        }
        else
        {
            this.forwardSpeed = 0;
        }

        this.velocity.y += 10;
        MoveAndSlide(this.velocity);

        for(int i = 0; i < this.GetSlideCount(); i++){
            this.normalAngle = GetSlideCollision(i).Normal;
            var forwardAngle = normalAngle.Angle() + ((float)Math.PI / 2f);
            GD.Print(forwardAngle);
            this.velocity = new Vector2((float)Math.Cos(forwardAngle)*forwardSpeed,
                                       (float) Math.Sin(forwardAngle)*forwardSpeed);
        }

        MoveAndSlide(linearVelocity: this.velocity,
                     floorNormal: this.normalAngle,
                     floorMaxAngle: (float)(2 * Math.PI));
        
            
            //col.Colli((CollisionPolygon2D)col.ColliderShape).Polygon[col.ColliderShapeIndex];
        /*
        var remainingVelocity = MoveAndSlide(new Vector2(0, 10 * delta * gravity), new Vector2(0,-1), 19, 19, 2f * (float)Math.PI);
        var floorAngle = (remainingVelocity - velocity).Normalized();
        var forwardAngle = floorAngle.Angle() + ((float)Math.PI / 2f);
        var lostVelocity = new Vector2((float)Math.Cos(forwardAngle)*forwardSpeed,
                                       (float) Math.Sin(forwardAngle)*forwardSpeed);
        var leftover = MoveAndSlide(lostVelocity, floorAngle,19, 19, 2f*(float)Math.PI);
        GD.Print(floorAngle);
        */
    }

}
