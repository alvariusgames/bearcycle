using Godot;
using System;

public enum BearState {ON_ATV, HIT, RECOVERING_ATV};

public class Bear : FSMKinematicBody2D<BearState>{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    private const float GRAVITY  = 600.0f;
    private const float MAX_GRAVITY_SPEED = 300f;
    private const float FRICTION_EFFECT = 0.9f;

    public Vector2 velocity = new Vector2(0,0);
    public Sprite Sprite;
    private ATV ATV;
    private int recoveryTimer = 0;

    public override void _Ready()
    {
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.Sprite = (Sprite)child;
            }
        }
        this.ATV = (ATV)this.GetParent();
    }

    public override void UpdateState(float delta){
    
    }

    public override void ReactToState(float delta){
        KinematicCollision2D collision;
        switch(this.ActiveState){
            case BearState.HIT:
                this.applyGravity(delta);
                this.MoveAndSlide(this.velocity);
                for(var i=0; i<this.GetSlideCount(); i++){
                    this.velocity.x *= 0.8f;
                    this.velocity.y *= 0.8f;
                    collision = this.GetSlideCollision(i);
                    if(this.velocity.Length() <= 50 && collision.Normal.y < 0){
                        this.SetActiveState(BearState.RECOVERING_ATV, 100);
                        recoveryTimer = 0;
                    }
                }
                break;
            case BearState.ON_ATV:
                this.velocity.x = 0;
                this.velocity.y = 0;
                collision = this.MoveAndCollide(this.velocity);
                if(collision != null){
                    this.SetActiveState(BearState.HIT, 100);
                    this.ATV.ThrowBearOffATV();
                }
                break;
            case BearState.RECOVERING_ATV:
                recoveryTimer++;
                if(recoveryTimer > 60){
                    this.applyGravity(delta);
                    var angleToATV = (this.ATV.GetGlobalCenterOfTwoWheels() - this.GetGlobalPosition()).Normalized();
                    var advanceSpeed = 0f;
                    if(advanceSpeed < 30){
                        advanceSpeed++;
                    }

                    this.MoveAndSlide(this.velocity);
                    this.velocity += angleToATV * advanceSpeed;
                    for(var i=0; i< this.GetSlideCount(); i++){
                        collision = this.GetSlideCollision(i);
                        var forwardOnCurve = collision.Normal.Rotated(0.5f * (float)Math.PI);
                        var onCurveTowardsATV = forwardOnCurve.AngleTo(angleToATV);
                        if(Math.Abs(onCurveTowardsATV) < Math.PI / 2f){
                            var angleAbove = 0.2f * forwardOnCurve.AngleTo(collision.Normal);
                            var angleToApply = forwardOnCurve.Rotated(angleAbove);
                            this.velocity = this.velocity.Length() * angleToApply;}
                        else {
                            var angleAbove = 0.2f * (-forwardOnCurve).AngleTo(collision.Normal);
                            var angleToApply = (-forwardOnCurve).Rotated(angleAbove);
                            this.velocity = this.velocity.Length() * angleToApply;}
                    }
                }
                break;
            default:
                throw new Exception("Bear must have a valid state");
        }
    }

    public override void ReactStateless(float delta){
   }

    private void applyGravity(float delta){
        if(this.velocity.y < MAX_GRAVITY_SPEED){
            this.velocity.y += delta * GRAVITY;
        }
    }
}