using Godot;
using System;

public class Player : KinematicBody2D
{
    private const float GRAVITY  = 600.0f;
    private const float MAX_GRAVITY_SPEED = 300f;
    private const float MAX_FORWARD_ACCEL = 40f;
    private const float MAX_BACKWARD_ACCEL = - MAX_FORWARD_ACCEL;
    private Vector2 velocity = new Vector2(0,0);
    private const float MAX_SPEED = 300f;
    private float forwardAccell = 0f;
    private const float FORWARD_ACCEL_UNIT = 1.5f;
    private const float DECELL_EFFECT = 0.9f;
    private const float FRICTION_EFFECT = 0.90f;

    public override void _PhysicsProcess(float delta)
    {
        this.reactToInput(delta);
        this.processPhysics(delta);
        this.applyPhysics(delta);
    }

    private void reactToInput(float delta){
        if (Input.IsActionPressed("ui_left") &&
           (this.forwardAccell >= MAX_BACKWARD_ACCEL))
        {
            this.forwardAccell -= FORWARD_ACCEL_UNIT;
        }
        else if (Input.IsActionPressed("ui_right") &&
                (this.forwardAccell <= MAX_FORWARD_ACCEL)) 
        {       
            this.forwardAccell += FORWARD_ACCEL_UNIT;
        }
        else
        {
            this.forwardAccell *= DECELL_EFFECT;
        }
    }

    private void processPhysics(float delta){
        //GD.Print(this.velocity);
        //Apply the gravity effect
        if(this.velocity.y <= MAX_GRAVITY_SPEED){        
            this.velocity.y += delta * GRAVITY;
        }

        for(int i = 0; i < this.GetSlideCount(); i++){
            //Apply forward speed ONLY during collisions
           var forwardAngle = this.calculateForwardAngle(GetSlideCollision(i));
           if (Math.Abs(this.velocity.Length()) <= MAX_SPEED){
                this.velocity.x += forwardAngle.x*forwardAccell;
                this.velocity.y += forwardAngle.y*forwardAccell;
            }
            this.velocity *= FRICTION_EFFECT;
        }
    }

    /// "Normal" is defined as the direction "up" away from the platform.
    ///     - this is calculated automatically for us for each kinematic collision
    /// "Forward" is defined as the direction to continue following the curve of the platform
    ///     - We must calculate this
    /// Purely mathematically, Forward = Normal + (PI / 2 radians)
    /// In this Godot project, this works for Convex curves, but fails for Concave curves
    ///     - The Player will follow the curve when it should fly away from it
    /// The solution to this is to make the "Forward" angle a bit "above" forward.
    /// "above" is defined as the direction towards "Normal" (away from the platform)
    /// "above" is dependant upon what direction the player is currently moving, AKA the angle of velocity
    private Vector2 calculateForwardAngle(KinematicCollision2D collision){
        //Calculate the percent offset angle above
        const float angleAbovePercent = 0.20f;
        var normalAngle = collision.Normal.Normalized();
        var currentVelocityAngle = collision.Travel.Normalized();
        var angleAbove = angleAbovePercent * currentVelocityAngle.AngleTo(normalAngle);
        //Apply that angle to the forward angle 
        var unadjustedForwardAngle = normalAngle.Rotated((float)Math.PI / 2f).Normalized();
        var adjustedForwardAngle = unadjustedForwardAngle.Rotated(angleAbove);
        GD.Print(angleAbove);
        return adjustedForwardAngle;
   }
    private void applyPhysics(float delta){
        MoveAndSlide(linearVelocity: this.velocity);
    }
            
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


