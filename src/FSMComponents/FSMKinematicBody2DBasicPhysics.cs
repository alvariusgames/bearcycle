using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class FSMKinematicBody2DBasicPhysics<T> : FSMKinematicBody2D<T>{
    public Vector2 velocity = new Vector2(0,0);
    public float forwardAccell = 0f;
    protected int platformCollisions = 0;
    //The below 2 vars auto update, & can be used to calculate all info about "forward"
    // See `this.calculateForwardAngle()` for more information
    protected Vector2 currentTravel = new Vector2(0,0);
    protected Vector2 currentNormal = new Vector2(0,-1);
    public virtual float MaxForwardAccel {get; set;} = 130f;
    protected virtual float MaxBackwardAccel {get; set;} = -130f;
    protected virtual float ForwardAccelUnit {get; set;} = 6f;
    public const float GRAVITY  = 1400.0f;
    public const float MAX_GRAVITY_SPEED = 2200f;
    protected const float MAX_SPEED = 1600f;
    protected const float DEFAULT_FRICTION_EFFECT = 0.90f;
    private const int NUM_GLOBAL_ROTATIONS_TO_STORE = 15;
    public Queue<float> OngoingGlobalRotations = new Queue<float>();
    public float GlobalRotationNormalized { get {
        return this.OngoingGlobalRotations.ToArray().Average();}}

    public override void _Ready(){
        this.OngoingGlobalRotations = new Queue<float>();
        for(int i=0; i<NUM_GLOBAL_ROTATIONS_TO_STORE; i++){
            this.OngoingGlobalRotations.Enqueue(this.GlobalRotation);}
        base._Ready();}

    public override void _Process(float delta){
        base._Process(delta);
        this.manageOngoingGlobalRotations(delta);}

    public Boolean IsInAir(){
        return this.platformCollisions <= 0;}

    protected void reactToSlideCollision(float delta,
                                         float frictionEffect = DEFAULT_FRICTION_EFFECT){
        //Process Collision with platforms
        this.platformCollisions = 0;
        var collAggreg = 0f;
        var collNumber = 0;
        for(int i = 0; i < this.GetSlideCount(); i++){
           var collision = this.GetSlideCollision(i);
           this.OnCollisionWith(collision.Collider);
           if(collision.Collider is IPlatform){
                collAggreg += collision.Normal.Angle();
                collNumber++;
                this.platformCollisions++;
                //Save relevant collision info to this
                this.currentTravel = collision.Travel;
                this.currentNormal = collision.Normal;
                //Calculate the Forward movement
                var forwardAngle = this.calculateForwardAngle();
                if (Math.Abs(this.velocity.Length()) <= MAX_SPEED){
                        this.velocity.x += forwardAngle.x*forwardAccell;
                        this.velocity.y += forwardAngle.y*forwardAccell;}
                if(collision.Collider is KinematicPlatform){
                    var kinematicPlatform = (KinematicPlatform)collision.Collider;
                    if(kinematicPlatform.TransferMovementToPlayer){
                        //TODO: remove this transferMovement property since it always transfers movement
                        var y = kinematicPlatform.velocity.y;
                        if(y > 0){
                             this.applyGravity(delta);
                        }
                   }}
                this.velocity *= frictionEffect;}
            else if(this is INPC && !(collision.Collider is PlayerAttackWindow)){
                ((INPC)this).GetHitBy((Node2D)collision.Collider);}
        }
        if(collAggreg != 0f){
            this.GlobalRotation = (collAggreg / collNumber) + ((float)Math.PI / 2f);
            this.manageOngoingGlobalRotations(delta);
            this.GlobalRotation = this.GlobalRotationNormalized;}}

    private void manageOngoingGlobalRotations(float delta){
        this.OngoingGlobalRotations.Dequeue();
        this.OngoingGlobalRotations.Enqueue(this.GlobalRotation);}

    public virtual void OnCollisionWith(Godot.Object collider){}

    private Vector2 calculateForwardAngle(){
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
    protected void applyGravity(float delta){
        if(this.velocity.y < MAX_GRAVITY_SPEED){
            this.velocity.y += delta * GRAVITY;
        }
    }
}