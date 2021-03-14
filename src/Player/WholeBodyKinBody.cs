using Godot;
using System.Collections.Generic;

public enum WholeBodyState { NORMAL, 
                            INTERACTING_WITH_INTERACTABLE, 
                            TRIGGER_HOLDABLE_ACTION_PRESS, 
                            TRIGGER_HOLDABLE_ACTION_HOLD};

public class WholeBodyKinBody : FSMKinematicBody2D<WholeBodyState>{
    public Player Player;
    public bool IsOverInteractable = false;

    public override WholeBodyState InitialState { get { return WholeBodyState.NORMAL;}set{}}

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
        if(this.Player.ATV.ActiveState == ATVState.WITH_BEAR &&
            this.Player.ActiveHoldable != null){
            if(Input.IsActionJustPressed("ui_use_item") && 
               this.timeElapsedSinceLastActionPress > MIN_ACTION_PRESS_WAITING_PERIOD_SEC){
                this.timeElapsedSinceLastActionPress = 0f;
                this.Player.ActiveHoldable.ReactToActionPress(delta);}
            else if(Input.IsActionPressed("ui_use_item")){
                this.Player.ActiveHoldable.ReactToActionHold(delta);}
            if(Input.IsActionJustReleased("ui_use_item")){
                this.Player.ActiveHoldable.ReactToActionRelease(delta);
            }}
        if(Input.IsActionJustPressed("ui_attack") &&
           this.Player.ATV.ActiveState == ATVState.WITH_BEAR){
                this.Player.ClawAttack.ReactToActionPress(delta);}}

    private float timeElapsedSinceLastActionPress = 0f;

    private const float MIN_ACTION_PRESS_WAITING_PERIOD_SEC = 1f;

    public override void ReactStateless(float delta){
        this.timeElapsedSinceLastActionPress += delta;
        this.SetGlobalPosition(this.Player.ATV.GetDeFactoGlobalPosition());
        this.SetGlobalRotation(this.Player.ATV.GetDeFactorGlobalRotation());
        if(this.Player.ATV.Direction == ATVDirection.FORWARD){
            this.SetScale(new Vector2(1,1));}
        else if (this.Player.ATV.Direction == ATVDirection.BACKWARD){
           this.SetScale(new Vector2(-1,1));}
        if(this.Player.ATV.ActiveState == ATVState.WITH_BEAR){
            this.reactToCollisions(delta);}}    

    private Queue<Godot.Object> externallyReportedColliders = new Queue<Godot.Object>();
    public void reportExternalCollider(Godot.Object collider){
        ///Used mainly by RigidBody2D for reporting on these collisions
        ///that take place outside of `getSlideCollision()`
        this.externallyReportedColliders.Enqueue(collider);}

    private void reactToCollisions(float delta){
        if(!this.Player.ActiveState.Equals(PlayerState.ALIVE)){
            return;}
        ZoneCollider highestPriorityZoneCollider = null;
        IInteractable highestPriorityInteractable = null;
        this.IsOverInteractable = false;
        this.MoveAndSlide(new Vector2(0,0));
        var colliders = new List<Godot.Object>();
        for(int i=0; i< this.GetSlideCount(); i++){
            colliders.Add(this.GetSlideCollision(i).Collider);}
        while(this.externallyReportedColliders.Count > 0){
            colliders.Add(this.externallyReportedColliders.Dequeue());}

        foreach(Node collider in colliders){
            //ZoneColliderArea
            //----------------
            if(collider is ZoneCollider){
                var zoneCollider = (ZoneCollider)(collider); //TODO: find more elegant solution
                if(highestPriorityZoneCollider == null){
                    highestPriorityZoneCollider = zoneCollider;
                } else if(highestPriorityZoneCollider.priority <= zoneCollider.priority){
                    highestPriorityZoneCollider = zoneCollider;}}

            //IInteractable Area
            //------------------
            else if(collider is IInteractable){
                this.IsOverInteractable = true;
                this.SetActiveState(WholeBodyState.INTERACTING_WITH_INTERACTABLE, 200);
                var interactable = (IInteractable)(collider);
                if(highestPriorityInteractable == null){
                    highestPriorityInteractable = interactable;
                } else if(highestPriorityInteractable.InteractPriority <= interactable.InteractPriority){
                    highestPriorityInteractable = interactable;
                }
            }

            //NPC Area
            //---------
            else if(collider is INPC){
                var npc = (INPC)collider;
                if(this.Player.ATV.Bear.ActiveState != BearState.HIT_SEQ_INVINC){
                    npc.GetHitBy(this);
                    this.Player.GetHitBy(npc);}}

            //NPC Attack Area
            //---------------
            else if(collider is NPCAttackWindow){
                if(this.Player.ATV.Bear.ActiveState != BearState.HIT_SEQ_INVINC){
                    var npcAttackWindow = (NPCAttackWindow)collider;
                    if(this.Player.ATV.Bear.ActiveState != BearState.HIT_SEQ_INVINC){
                        npcAttackWindow.GetHitBy(this);
                        this.Player.GetHitBy(npcAttackWindow);}}}

            //IConsumable Area
            //---------------
            else if(collider is IConsumeable){
                ((IConsumeable)collider).Consume(this.Player);}}
            
        //INTERACTABLE LOGIC TO APPLY AFTER ALL COLLISIONS
        if(highestPriorityInteractable != null &&
           Input.IsActionJustPressed("ui_forage") &&
           this.timeElapsedSinceLastActionPress > MIN_ACTION_PRESS_WAITING_PERIOD_SEC){
            highestPriorityInteractable.InteractWith(this.Player, delta);}

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
