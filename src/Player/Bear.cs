using Godot;
using System;

public enum BearState {ON_ATV, TRIGGER_HIT_SEQUENCE, HIT_SEQ_FALL_OFF,
                      HIT_SEQ_INVINC, HIT_SEQ_TRIGGER_TEMP_INVINC};

public class Bear : FSMKinematicBody2D<BearState>{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override BearState InitialState {get { return BearState.ON_ATV; }}
    private const float GRAVITY  = 600.0f;
    private const float MAX_GRAVITY_SPEED = 300f;
    private const float FRICTION_EFFECT = 0.9f;

    public Vector2 velocity = new Vector2(0,0);
    public AnimatedSprite Sprite;
    public ATV ATV;

    public override void _Ready()
    {
        this.ResetActiveState(this.InitialState);
        foreach(var child in this.GetChildren()){
            if(child is AnimatedSprite){
                this.Sprite = (AnimatedSprite)child;
            }
        }
        this.ATV = (ATV)this.GetParent();
    }

    public override void UpdateState(float delta){
    
    }

    public override void ReactToState(float delta){
        KinematicCollision2D collision;
        float numSecondsToWait;
        switch(this.ActiveState){
            case BearState.TRIGGER_HIT_SEQUENCE:
                this.SetActiveState(BearState.HIT_SEQ_FALL_OFF, 100);
                this.SetActiveStateAfter(BearState.HIT_SEQ_TRIGGER_TEMP_INVINC, 200, 1.5f);
                this.ResetActiveStateAfter(BearState.ON_ATV, 4.5f);
                this.Sprite.Play("hit");
                break;
            case BearState.HIT_SEQ_FALL_OFF:
                this.applyGravity(delta);
                this.MoveAndSlide(this.velocity);
                for(var i=0; i<this.GetSlideCount(); i++){
                    this.velocity.x *= 0.8f;
                    this.velocity.y *= 0.8f;
                    collision = this.GetSlideCollision(i);}
                break;
            case BearState.HIT_SEQ_TRIGGER_TEMP_INVINC:
                this.ATV.ReattachBear();
                this.SetActiveState(BearState.HIT_SEQ_INVINC, 300);
                this.ATV.Player.UpdateHealth(Player.DEFAULT_HIT_UNIT);
                break;
            case BearState.HIT_SEQ_INVINC:
                this.velocity.x = 0f;
                this.velocity.y = 0f;
                this.Sprite.Play("invinc");
                break;
            case BearState.ON_ATV:
                this.velocity.x = 0;
                this.velocity.y = 0;
                collision = this.MoveAndCollide(this.velocity);
                if(collision != null){
                    if(collision.Collider is Platforms){
                        this.commonHitFunctionality();}
                    if((collision.Collider is NPC)){
                        this.commonHitFunctionality();
                        GD.Print(collision.Collider);
                        ((NPC)collision.Collider).GetHitBy(this);}}
                this.Sprite.Play("default");
                break;
            default:
                throw new Exception("Bear must have a valid state");
        }
    }

    private void commonHitFunctionality(){
        this.SetActiveState(BearState.TRIGGER_HIT_SEQUENCE, 100);
        this.ATV.ThrowBearOffATV();}

    public override void ReactStateless(float delta){}

    private void applyGravity(float delta){
        if(this.velocity.y < MAX_GRAVITY_SPEED){
            this.velocity.y += delta * GRAVITY;
        }
    }
}