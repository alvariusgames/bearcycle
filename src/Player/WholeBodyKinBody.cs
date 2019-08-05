using Godot;
using System;

public enum WholeBodyState { DEFAULT };

public class WholeBodyKinBody : FSMKinematicBody2D<WholeBodyState>{
    public Player Player;

    public override WholeBodyState InitialState { get { return WholeBodyState.DEFAULT;}}

    public override void _Ready(){
        this.Player = (Player)this.GetParent();}

    public override void UpdateState(float delta){}

    public override void ReactToState(float delta){}

    public override void ReactStateless(float delta){
        this.SetGlobalPosition(this.Player.ATV.GetDeFactoGlobalPosition());
        this.SetGlobalRotation(this.Player.ATV.GetDeFactorGlobalRotation());
        if(this.Player.ATV.Direction == ATVDirection.FORWARD){
            this.SetScale(new Vector2(1,1));}
        else if (this.Player.ATV.Direction == ATVDirection.BACKWARD){
           this.SetScale(new Vector2(-1,1));}
        this.reactToSlideCollisions(delta);}    

    private void reactToSlideCollisions(float delta){
        ZoneCollider highestPriorityZoneCollider = null;
        this.MoveAndSlide(new Vector2(0,0));
        for(int i = 0; i < this.GetSlideCount(); i++){
            var collision = this.GetSlideCollision(i);
            if(collision.Collider is SpeedBoost && Input.IsActionPressed("ui_accept")){
                var speedBoost = (SpeedBoost)collision.Collider;
                var accellMagnitude = Wheel.MAX_FORWARD_ACCEL * 2;
                if(this.Player.ATV.Direction == ATVDirection.FORWARD){
                    this.Player.ATV.SetAccellOfTwoWheels(accellMagnitude);
                    this.Player.ATV.SetVelocityOfTwoWheels(speedBoost.VelocityToApply * 2);
                } else if(this.Player.ATV.Direction == ATVDirection.BACKWARD){
                    this.Player.ATV.SetAccellOfTwoWheels(-accellMagnitude);
                    this.Player.ATV.SetVelocityOfTwoWheels(-speedBoost.VelocityToApply * 2);}
                speedBoost.GetHitBy(this);
            } else if(collision.Collider is ZoneCollider){
                var zoneCollider = (ZoneCollider)(collision.Collider);
                if(highestPriorityZoneCollider == null){
                    highestPriorityZoneCollider = zoneCollider;
                } else if(highestPriorityZoneCollider.priority <= zoneCollider.priority){
                    highestPriorityZoneCollider = zoneCollider;}}}
        if(highestPriorityZoneCollider != null){
            foreach(var bit in highestPriorityZoneCollider.allCollisionZones){
                this.Player.ATV.Bear.SetCollisionMaskBit(bit, false);
                this.Player.ATV.BackWheel.SetCollisionMaskBit(bit, false);
                this.Player.ATV.FrontWheel.SetCollisionMaskBit(bit, false);}
            foreach(var bit in highestPriorityZoneCollider.ZoneCollisionMaskBits){
                this.Player.ATV.Bear.SetCollisionMaskBit(bit, true);
                this.Player.ATV.FrontWheel.SetCollisionMaskBit(bit, true);
                this.Player.ATV.BackWheel.SetCollisionMaskBit(bit, true);}}}}
