using Godot;
using System;

public class RocketBooster : MonitorHoldable{
    private Vector2 StartingPosition;
    [Export]
    public override int NumActionCallsToDepleted { get; set; } = 300;
    public override Texture UIDisplayIcon { get { 
        return (Texture)GD.Load("res://media/sprites/items/holdables/icons/rocket_booster_icon.png");} set {}}
    [Export]
    public float VelocityStep = 50;
    [Export]
    public float PseudoImpulseEffectSec = 1f;
    [Export]
    public float MaxSpeedToApply = 1200;
    private Node OriginalParent;
    public float numSecondsBeingHeld = 0f;
    public override Boolean DisplayInProgressBar { get { return true;}}
    private AnimatedSprite MonitorAnimSprite;
    private AnimatedSprite CloudAnimSprite;
    public Boolean IsEmitting{ get {
        // Reactive, but also normalized to 'linger' a bit if pressed for longer
        return Input.IsActionPressed("ui_use_item") /*|| 
               this.numSecondsBeingHeld - (this.PseudoImpulseEffectSec * 0.66f)  > 0;*/;
    }}
    public override void ReactToActionHold(float delta){
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this, "res://media/samples/items/rocketboost1.wav",
                                                       SkipIfAlreadyPlaying: true, Loop: true);
        this.numSecondsBeingHeld += 2 * delta; //Double it since we'll be subracting it always in ReactStateless()
        if(this.numSecondsBeingHeld > this.PseudoImpulseEffectSec){
             this.numSecondsBeingHeld = this.PseudoImpulseEffectSec;}
        var forwardVec = this.Player.ATV.GetNormalizedForwardDirection();//this.Player.ATV.CurrentNormal.Rotated((float)Math.PI * 3f / 2f);
        var speedStep = (this.numSecondsBeingHeld / this.PseudoImpulseEffectSec) * this.VelocityStep;
        var velStep = forwardVec * speedStep;
        var newVel = this.Player.ATV.GetVelocityOfTwoWheels() + velStep;
        if(newVel.Length() > this.MaxSpeedToApply){
            newVel = this.MaxSpeedToApply * newVel.Normalized();}
        this.Player.ATV.SetVelocityOfTwoWheels(newVel);
        this.CurrentNumActionCalls++;
    }
    public override void ReactToActionRelease(float delta){
         SoundHandler.StopSample(this, "res://media/samples/items/rocketboost1.wav");       
    }

    public override void PostDepletionAction(){
        SoundHandler.StopSample(this, "res://media/samples/items/rocketboost1.wav");
        base.PostDepletionAction();
    }

    public override void ReactToActionPress(float delta){}
    public override void ReactStateless(float delta){
        this.numSecondsBeingHeld -= delta; 
        if(this.numSecondsBeingHeld < 0){
            this.numSecondsBeingHeld = 0f;}
        base.ReactStateless(delta);
    }

}
