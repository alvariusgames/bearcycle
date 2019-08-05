using Godot;
using System;

public enum BearState {ON_ATV, TRIGGER_HIT_SEQUENCE, HIT_SEQ_FALL_OFF,
                      HIT_SEQ_INVINC, HIT_SEQ_TRIGGER_TEMP_INVINC};

public class Bear : FSMKinematicBody2D<BearState>{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override BearState InitialState {get { return BearState.ON_ATV; }}
    private const float GRAVITY  = 2000.0f;
    private const float MAX_GRAVITY_SPEED = 2000f;
    private const float FRICTION_EFFECT = 0.9f;

    public Vector2 velocity = new Vector2(0,0);
    public AnimationPlayer CutOutAnimationPlayer;
    public Node2D CutOut;
    public ATV ATV;
    public CollisionShape2D CollisionShape2D;

    public override void _Ready()
    {
        this.ResetActiveState(this.InitialState);
        foreach(var child in this.GetChildren()){
            if(((Node2D)child).Name.ToLower().Contains("cutout")){
                this.CutOut = (Node2D)child;
                foreach(var child2 in this.CutOut.GetChildren()){
                    if(child2 is AnimationPlayer){
                        this.CutOutAnimationPlayer = (AnimationPlayer)child2;}
                }
            }
            else if (child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
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
                this.SetActiveStateAfter(BearState.HIT_SEQ_TRIGGER_TEMP_INVINC, 200, 2.5f);
                this.ResetActiveStateAfter(BearState.ON_ATV, 5.5f);
                this.ATV.Player.playBearAnimation("die1");
                this.ATV.Player.ResetActiveState(PlayerState.NORMAL);
                this.ATV.Player.ForceClearAllTimers();
                break;
            case BearState.HIT_SEQ_FALL_OFF:
                this.applyGravity(delta);
                this.MoveAndSlide(this.velocity);
                this.velocity.x *= 0.9f;
                this.velocity.y *= 0.9f;
                var collAggreg = 0f;
                for(int i=0;i<this.GetSlideCount();i++){
                        collision = this.GetSlideCollision(i);
                        collAggreg += collision.Normal.Angle();}
                if(collAggreg != 0f){
                    this.SetGlobalRotation((this.GetGlobalRotation() + (
                        collAggreg / this.GetSlideCount()) + ((float)Math.PI / 2f)) / 2f);}
                break;
            case BearState.HIT_SEQ_TRIGGER_TEMP_INVINC:
                this.ATV.ReattachBear();
                this.SetActiveState(BearState.HIT_SEQ_INVINC, 300);
                this.ATV.Player.UpdateHealth(Player.DEFAULT_HIT_UNIT);
                this.ATV.Player.stopAllBearAnimation();
                this.ATV.Player.playBearAnimation("idleBounce1");                
                break;
            case BearState.HIT_SEQ_INVINC:
                this.velocity.x = 0f;
                this.velocity.y = 0f;
                break;
            case BearState.ON_ATV:
                this.velocity.x = 0;
                this.velocity.y = 0;
                collision = this.MoveAndCollide(this.velocity);
                if(collision != null){
                    if(collision.Collider is Platforms){
                        this.commonHitFunctionality();}
                    if(collision.Collider is NPC){
                        this.commonHitFunctionality();
                        var npc = ((NPC)collision.Collider);
                        npc.GetHitBy(this);
                        this.ATV.Player.GetHitBy(npc);}
                    if(collision.Collider is IConsumeable){
                        ((IConsumeable)collision.Collider).consume(this);}}
                break;
            default:
                throw new Exception("Bear must have a valid state");
        }
    }

    public void TriggerHitSequence(float throwSpeed = 300){
        this.SetActiveState(BearState.TRIGGER_HIT_SEQUENCE, 100);
        this.ATV.ThrowBearOffATV(throwSpeed);
    }

    private void commonHitFunctionality(){
        this.TriggerHitSequence();
    }

    public override void ReactStateless(float delta){
        drawBear(delta);
    }

    private void drawBear(float delta){
        var cutoutScale = this.CutOut.GetScale();
        if(this.ATV.Direction == ATVDirection.FORWARD && !this.ATV.IsInAir()){
            this.CutOut.SetScale(new Vector2(Math.Abs(cutoutScale[0]),
                                             cutoutScale[1]));
        } else if(this.ATV.Direction == ATVDirection.BACKWARD && !this.ATV.IsInAir()){
            this.CutOut.SetScale(new Vector2(-Math.Abs(cutoutScale[0]),
                                             cutoutScale[1]));}}

    private void applyGravity(float delta){
        if(this.velocity.y < MAX_GRAVITY_SPEED){
            this.velocity.y += delta * GRAVITY;
        }
    }
}