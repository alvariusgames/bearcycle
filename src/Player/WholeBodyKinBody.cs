using Godot;
using System;

public enum WholeBodyState { NORMAL, 
                            INTERACTING_WITH_INTERACTABLE, 
                            TRIGGER_HOLDABLE_ACTION_PRESS, 
                            TRIGGER_HOLDABLE_ACTION_HOLD};

public class WholeBodyKinBody : FSMKinematicBody2D<WholeBodyState>{
    public Player Player;
    public bool IsOverInteractable = false;

    public override WholeBodyState InitialState { get { return WholeBodyState.NORMAL;}}

    public override void _Ready(){
        this.Player = (Player)this.GetParent();}

    public override void UpdateState(float delta){
        this.reactToInput(delta);
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case WholeBodyState.NORMAL:
                //this.AttackWindow.ResetActiveState(AttackWindowState.NOT_ATTACKING);
                break;
            case WholeBodyState.INTERACTING_WITH_INTERACTABLE:
                this.ResetActiveState(WholeBodyState.NORMAL);
                break;
        }
    }

    private void reactToInput(float delta){
        if(Input.IsActionJustPressed("ui_use_item") &&
           this.Player.ATV.ActiveState == ATVState.WITH_BEAR &&
           this.Player.ActiveHoldable != null){
                this.Player.ActiveHoldable.ReactToActionPress(delta);}
        else if(Input.IsActionPressed("ui_use_item") &&
                this.Player.ATV.ActiveState == ATVState.WITH_BEAR &&
                this.Player.ActiveHoldable != null){
                    this.Player.ActiveHoldable.ReactToActionHold(delta);}
        if(Input.IsActionJustPressed("ui_attack") &&
           this.Player.ATV.ActiveState == ATVState.WITH_BEAR){
                this.Player.ClawAttack.ReactToActionPress(delta);}}

    public override void ReactStateless(float delta){
        this.SetGlobalPosition(this.Player.ATV.GetDeFactoGlobalPosition());
        this.SetGlobalRotation(this.Player.ATV.GetDeFactorGlobalRotation());
        if(this.Player.ATV.Direction == ATVDirection.FORWARD){
            this.SetScale(new Vector2(1,1));}
        else if (this.Player.ATV.Direction == ATVDirection.BACKWARD){
           this.SetScale(new Vector2(-1,1));}
        if(this.Player.ATV.ActiveState == ATVState.WITH_BEAR){
            this.reactToSlideCollisions(delta);}}    

    private void reactToSlideCollisions(float delta){
        if(!this.Player.ActiveState.Equals(PlayerState.ALIVE)){
            return;}
        ZoneCollider highestPriorityZoneCollider = null;
        IInteractable highestPriorityInteractable = null;
        this.IsOverInteractable = false;
        this.MoveAndSlide(new Vector2(0,0));
        for(int i = 0; i < this.GetSlideCount(); i++){
            var collision = this.GetSlideCollision(i);

            //ZoneColliderArea
            //----------------
            if(collision.Collider is ZoneCollider){
                var zoneCollider = (ZoneCollider)(collision.Collider); //TODO: find more elegant solution
                if(highestPriorityZoneCollider == null){
                    highestPriorityZoneCollider = zoneCollider;
                } else if(highestPriorityZoneCollider.priority <= zoneCollider.priority){
                    highestPriorityZoneCollider = zoneCollider;}}

            //IInteractable Area
            //------------------
            else if(collision.Collider is IInteractable){
                this.IsOverInteractable = true;
                this.SetActiveState(WholeBodyState.INTERACTING_WITH_INTERACTABLE, 200);
                var interactable = (IInteractable)(collision.Collider);
                if(highestPriorityInteractable == null){
                    highestPriorityInteractable = interactable;
                } else if(highestPriorityInteractable.InteractPriority <= interactable.InteractPriority){
                    highestPriorityInteractable = interactable;
                }
            }

            //NPC Area
            //---------
            else if(collision.Collider is INPC && this.Player.ATV.Bear.ActiveState != BearState.HIT_SEQ_INVINC){
                var npc = (INPC)collision.Collider;
                npc.GetHitBy(this);
                this.Player.GetHitBy(npc);}

            //IConsumable Area
            //---------------
            else if(collision.Collider is IConsumeable){
                ((IConsumeable)collision.Collider).consume(this);}
            
            //EndLevel Area
            //-------------

            else if(collision.Collider is EndLevel){
                ((EndLevel)collision.Collider).EndLevel_(this.Player);
            }
            }

        //INTERACTABLE LOGIC TO APPLY AFTER ALL COLLISIONS
        if(highestPriorityInteractable != null && Input.IsActionJustPressed("ui_forage")){
            highestPriorityInteractable.InteractWith(this.Player);}
        /*
        if(highestPriorityInteractable is SpeedBoost && Input.IsActionJustPressed("ui_forage")){
                this.Player.ATV.FrontWheel.PlayEngineRevSound();
                var speedBoost = (SpeedBoost)highestPriorityInteractable;
                var accellMagnitude = Wheel.MAX_FORWARD_ACCEL * 2;
                if(this.Player.ATV.Direction == ATVDirection.FORWARD){
                    this.Player.ATV.SetAccellOfTwoWheels(accellMagnitude);
                    this.Player.ATV.SetVelocityOfTwoWheels(speedBoost.VelocityToApply * 2);}
                else if(this.Player.ATV.Direction == ATVDirection.BACKWARD){
                    this.Player.ATV.SetAccellOfTwoWheels(-accellMagnitude);
                    this.Player.ATV.SetVelocityOfTwoWheels(-speedBoost.VelocityToApply * 2);}
                speedBoost.GetHitBy(this);}
            else if(highestPriorityInteractable is IHoldable && Input.IsActionJustPressed("ui_forage")){
                var Holdable = (IHoldable)highestPriorityInteractable;
                this.Player.PickupHoldable(Holdable);}
            else if(highestPriorityInteractable is InfiniteFoodRegion && Input.IsActionJustPressed("ui_forage")){
                var infiniteFoodRegion = (InfiniteFoodRegion)highestPriorityInteractable;
                infiniteFoodRegion.InteractWith(this.Player);}
            else if(highestPriorityInteractable is StageSpring && Input.IsActionJustPressed("ui_forage")){
                var stageSpring = ((StageSpring)highestPriorityInteractable);
                stageSpring.TriggerHit();
                SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.Player.ATV.Bear,
                    new string[] {Spring.BOING_SAMPLE});
                var springNormalRadians = stageSpring.Rotation + (1.5f * Math.PI);
                var springNormalVec2 = new Vector2((float)Math.Cos(springNormalRadians), (float)Math.Sin(springNormalRadians));
                this.Player.ATV.SetVelocityOfTwoWheels((springNormalVec2 * stageSpring.VelocityToApply) + this.Player.ATV.GetVelocityOfTwoWheels());
            }
            */

        //ZONE COLLIDER LOGIC
        if(highestPriorityZoneCollider != null &&
           this.Player.ATV.FallThroughManager.ActiveState.Equals(FallThroughManagerState.NOT_FALLING_THROUGH)){
            foreach(var bit in ZoneCollider.AllCollisionZones){
                this.Player.ATV.Bear.SetCollisionMaskBit(bit, false);
                this.Player.ATV.BackWheel.SetCollisionMaskBit(bit, false);
                this.Player.ATV.FrontWheel.SetCollisionMaskBit(bit, false);}
            foreach(var bit in highestPriorityZoneCollider.ZoneCollisionMaskBits){
                this.Player.ATV.Bear.SetCollisionMaskBit(bit, true);
                this.Player.ATV.FrontWheel.SetCollisionMaskBit(bit, true);
                this.Player.ATV.BackWheel.SetCollisionMaskBit(bit, true);}}}}
