using Godot;
using System;

public enum WheelState {
    ACCELERATING,
    DECELERATING,
    IDLING,
    LOCKED}

public class Wheel : FSMKinematicBody2D<WheelState>{
    public override WheelState InitialState { get { return WheelState.IDLING;}}
    public Vector2 userControlledVelocity = new Vector2(0,0);
    public Vector2 velocity = new Vector2(0,0);
    public Sprite sprite;
    public CollisionShape2D collisionShape2D;
    public ATV ATV;
    public float forwardAccell = 0f;
    private int platformCollisions = 0;
    //The below 2 vars auto update, & can be used to calculate all info about "forward"
    // See `this.calculateForwardAngle()` for more information
    private Vector2 currentTravel = new Vector2(0,0);
    private Vector2 currentNormal = new Vector2(0,-1);
    private const float GRAVITY  = 1400.0f;
    private const float MAX_GRAVITY_SPEED = 2200f;
    private const float MAX_FORWARD_ACCEL = 130f;
    private const float MAX_BACKWARD_ACCEL = - MAX_FORWARD_ACCEL;
    private const float MAX_SPEED = 1600f;
    private const float FORWARD_ACCEL_UNIT = 6f;
    private const float DECELL_EFFECT = 0.90f;
    private const float LOCKING_EFFECT = 0.90f;
    private const float DEFAULT_FRICTION_EFFECT = 0.90f;
    ///when speedbosting, what slowdown effect to go back down to normal speed
    private const float SPEED_BOOST_SLOWDOWN_EFFECT = 0.995f;
    private const float BOUNCE_UP_EFFECT_VELOCITY = MAX_SPEED / 4; 

    public override void _Ready(){
        this.ResetActiveState(this.InitialState);
        for(int i = 0; i<this.GetChildCount(); i++){
            var child = this.GetChild(i);
            if(child is Sprite){
                this.sprite = (Sprite)child;
            }
            if(child is CollisionShape2D){
                this.collisionShape2D = (CollisionShape2D)child;
            }
        }
        this.ATV = (ATV)this.GetParent();
        this.SetActiveState(WheelState.IDLING, 100);
    }

    public override void UpdateState(float delta){
        this.reactToInput(delta);
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case WheelState.ACCELERATING:
                if(this.forwardAccell <= MAX_FORWARD_ACCEL){  
                    this.forwardAccell += FORWARD_ACCEL_UNIT;
                } else {
                    //we're in 'speed boost mode', slow us down a bit
                    this.forwardAccell *= SPEED_BOOST_SLOWDOWN_EFFECT;}
 
                //this.forwardAccell *= 0.95f; 
                reactToSlideCollision(delta);

                break;
            case WheelState.DECELERATING:
                if(this.forwardAccell >= MAX_BACKWARD_ACCEL){
                    this.forwardAccell -= FORWARD_ACCEL_UNIT;
                } else {
                    //we're in 'speed boost mode', slow us down a bit
                    this.forwardAccell *= SPEED_BOOST_SLOWDOWN_EFFECT;}
                //this.forwardAccell *= 0.95f;
                reactToSlideCollision(delta);
                break;
            case WheelState.IDLING:
                this.forwardAccell *= DECELL_EFFECT;
                reactToSlideCollision(delta);
                break;
            case WheelState.LOCKED:
                this.forwardAccell *= LOCKING_EFFECT;
                reactToSlideCollision(delta, 0.4f);
                break;
            default:
                throw new Exception("Wheel invalid state.");
        }
    }
    public override void ReactStateless(float delta){
        this.applyGravity(delta);
        MoveAndSlide(linearVelocity: this.velocity);
        this.updateSprite(delta);
    }

    private void reactToInput(float delta){
        if(!this.ATV.IsInAirNormalized()){
            ///Only change wheel state from input if on ground
            if (Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_right")){
                this.SetActiveState(WheelState.IDLING, 100);}
            else if (Input.IsActionPressed("ui_left")){
                this.SetActiveState(WheelState.DECELERATING, 100);}
            else if (Input.IsActionPressed("ui_right")){
                this.SetActiveState(WheelState.ACCELERATING, 100);}
            else{
                this.SetActiveState(WheelState.IDLING, 100);
            }
        }
    }

    private void applyGravity(float delta){
        if(this.velocity.y < MAX_GRAVITY_SPEED){
            this.velocity.y += delta * GRAVITY;
        }
    }

    private void reactToSlideCollision(float delta,
                                       float frictionEffect = DEFAULT_FRICTION_EFFECT){
        //Process Collision with platforms
        var numCollisions = this.GetSlideCount();
        this.platformCollisions = 0;
        for(int i = 0; i < this.GetSlideCount(); i++){
           var collision = this.GetSlideCollision(i);
           if(collision.Collider is Platforms){
                this.platformCollisions++;
                //Save relevant collision info to this
                this.currentTravel = collision.Travel;
                this.currentNormal = collision.Normal;
                //Calculate the Forward movement
                var forwardAngle = this.calculateForwardAngle();
                if (Math.Abs(this.velocity.Length()) <= MAX_SPEED){
                        this.velocity.x += forwardAngle.x*forwardAccell;
                        this.velocity.y += forwardAngle.y*forwardAccell;
                        }
                    //Apply the friction effect
                    this.velocity *= frictionEffect;}
            if(collision.Collider is IConsumeable){
                ((IConsumeable)collision.Collider).consume(this);}
            if(collision.Collider is NPC){
                var npc = (NPC)collision.Collider;
                npc.GetHitBy(this);
                this.ATV.Player.GetHitBy(npc);
                this.ATV.ApplyPhonyRunOverEffect(this);}
            if(collision.Collider is SpeedBoost){
                var speedBoost = (SpeedBoost)collision.Collider;
                this.ATV.SetVelocityOfTwoWheels(speedBoost.VelocityToApply);
                var accellMagnitude = MAX_FORWARD_ACCEL * 2;
                if(speedBoost.IsForward){
                    this.ATV.SetAccellOfTwoWheels(accellMagnitude);
                } else if(speedBoost.IsBackward){
                    this.ATV.SetAccellOfTwoWheels(-accellMagnitude);
                }
                //this.ATV.AdjustVelocityAndAccelOfTwoWheels(1f, 3f);
                speedBoost.GetHitBy(this);
            }}}

        public void PhonyBounceUp(float magnitude){
            this.velocity = new Vector2(this.currentNormal.x * magnitude * BOUNCE_UP_EFFECT_VELOCITY,
                                        this.currentNormal.y * magnitude * BOUNCE_UP_EFFECT_VELOCITY); 
        }

    /// "Normal" is defined as the direction "up" away from the platform.
    ///     - this is calculated automatically for us for each kinematic collision
    /// "Forward" is defined as the direction to continue following the curve of the platform
    ///     - We must calculate this
    /// Purely mathematically, Forward = Normal + (PI / 2 radians)
    /// In this Godot project, this works for Convex curves, but fails for Concave curves
    ///     - The Player will follow the curve when it should "fly away" from it
    /// The solution to this is to make the angle a bit "above" the traditional "forward".
    ///     - "above" is defined as the direction towards "Normal" (away from the platform)
    ///     - "above" is dependant the direction player is moving (AKA the angle of velocity)
    private Vector2 calculateForwardAngle(){
        //Calculate the angle above foreward
        const float angleAbovePercent = 0.20f;
        var normalAngle = this.currentNormal.Normalized();
        var currentVelocityAngle = this.currentTravel.Normalized();
        var angleAbove = angleAbovePercent * currentVelocityAngle.AngleTo(normalAngle);
        //Apply the previously calculate "angleAbove" to the forward angle 
        var unadjustedForwardAngle = normalAngle.Rotated((float)Math.PI / 2f).Normalized();
        var adjustedForwardAngle = unadjustedForwardAngle.Rotated(angleAbove);
        return adjustedForwardAngle;
    }

    public void AdjustVelocityAndAccell(float velocityMultiplier, float accellVelocityMultiplier){
        ///args: 1 means no change, 0.7f means 30% less, 2f means twice as fast, etc.
        this.velocity = this.velocity * velocityMultiplier;
        this.forwardAccell = this.forwardAccell * accellVelocityMultiplier;
    }

    public Boolean IsInAir(){
        return this.platformCollisions <= 0;
    }

    private void updateSprite(float delta){
        float randJiggleEffect = 0f;
        if(this.Name == "FrontWheel" && this.forwardAccell != 0f){
            randJiggleEffect = (float)((new Random().NextDouble() - 0.45f) * 0.1);
        } else if(this.Name == "BackWheel" && this.forwardAccell != 0f){
            randJiggleEffect = (float)((new Random().NextDouble() - 0.55f) * 0.1);
        }
        const float arbitraryConstant = 400f;
        if(Math.Abs(this.forwardAccell) > 5f){
            this.sprite.Rotate((this.forwardAccell / arbitraryConstant) + randJiggleEffect);
        } else {
            this.sprite.Rotate(this.forwardAccell / arbitraryConstant);   
        }
    }
}
