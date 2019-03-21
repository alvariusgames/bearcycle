using Godot;
using System;

public enum SpeedBoostState { READY_TO_BE_HIT, NOT_READY_TO_BE_HIT, TRIGGER_HIT_OF_BOOST}
public class SpeedBoost : FSMKinematicBody2D<SpeedBoostState>
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override SpeedBoostState InitialState { get { return SpeedBoostState.READY_TO_BE_HIT;}}
    public Sprite Sprite;
    public Vector2 ForwardDirection;
    public float ForwardMagnitude;
    public Vector2 ForwardVelocityToApply;
    public CollisionShape2D CollisionShape2D;
    const float RESET_TIME_SECONDS = 2f;

    public override void _Ready(){
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.Sprite = (Sprite)child;}
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
            }
        }
        this.ForwardDirection = new Vector2(1,0).Rotated(this.Rotation);
        this.ForwardMagnitude = (float)Convert.ToDouble(this.Sprite.Name);
        this.ForwardVelocityToApply = this.ForwardMagnitude * this.ForwardDirection;}

    public void GetHitBy(Node node){
        this.SetActiveState(SpeedBoostState.TRIGGER_HIT_OF_BOOST, 100);
        this.CollisionShape2D.Disabled = true;

    }

    public override void UpdateState(float delta){}
    public override void ReactStateless(float delta){}
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case SpeedBoostState.READY_TO_BE_HIT:
               this.CollisionShape2D.Disabled = false;
                break;
            case SpeedBoostState.TRIGGER_HIT_OF_BOOST:
               this.ResetActiveStateAfter(SpeedBoostState.READY_TO_BE_HIT, RESET_TIME_SECONDS);
               this.SetActiveState(SpeedBoostState.NOT_READY_TO_BE_HIT, 200);
               break;
            case SpeedBoostState.NOT_READY_TO_BE_HIT:
               break;
            default:
                throw new Exception("Invalid Speed Boost State");
        }
    }

 }
