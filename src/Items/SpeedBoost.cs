using Godot;
using System;

public enum SpeedBoostState { READY_TO_BE_HIT, NOT_READY_TO_BE_HIT, TRIGGER_HIT_OF_BOOST}
public class SpeedBoost : FSMKinematicBody2D<SpeedBoostState>, IInteractable
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override SpeedBoostState InitialState { get { return SpeedBoostState.READY_TO_BE_HIT;}}
    public int InteractPriority { get { return 500;}}
    public Sprite Sprite;
    public Vector2 Direction;
    public float Magnitude;
    public Vector2 VelocityToApply;
    public CollisionShape2D CollisionShape2D;
    public Boolean IsForward = false;
    public Boolean IsBackward = false;
    const float RESET_TIME_SECONDS = 0.2f;

    public AnimationPlayer AnimationPlayer;

    public override void _Ready(){
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.Sprite = (Sprite)child;
                foreach(var child2 in this.Sprite.GetChildren()){
                    if(child2 is AnimationPlayer){
                        this.AnimationPlayer = (AnimationPlayer)child2;}}}
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
            }
        }
        this.IsForward = this.CollisionShape2D.Name.ToLower().Contains("forward");
        this.IsBackward = this.CollisionShape2D.Name.ToLower().Contains("backward");
        this.Direction = new Vector2(1,0).Rotated(this.Rotation);
        this.Magnitude = (float)Convert.ToDouble(this.Sprite.Name);
        this.VelocityToApply = new Vector2(this.Direction.x * this.Magnitude,
                                                this.Direction.y * this.Magnitude);
        this.AnimationPlayer.Play("default");}

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
