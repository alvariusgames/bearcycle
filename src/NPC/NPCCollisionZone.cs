using Godot;
using System;
using System.Collections.Generic;
using System.Linq;    

public abstract class NPCCollisionZone : KinematicBody2D{
    [Export]
    public float ReportCollisionMaxPeriodSec {get; set;} = 2f;
    private Dictionary<INPC, float> colliderTimers = 
        new Dictionary<INPC, float>();
    private Vector2 initialGlobalPos = new Vector2(0,0);

    public override void _Ready(){
        this.initialGlobalPos = this.GlobalPosition;}

    public override void _Process(float delta){
        base._Process(delta);
        this.timerBookkeeping(delta);
        this.collisionChecking(delta);
        this.GlobalPosition = this.initialGlobalPos;
    }
    private void timerBookkeeping(float delta){
        foreach(var key in this.colliderTimers.Keys.ToArray()){
            this.colliderTimers[key] += delta;
            if(this.colliderTimers[key] > this.ReportCollisionMaxPeriodSec){
                this.colliderTimers.Remove(key);}}
    }

    private void collisionChecking(float delta){
        this.MoveAndSlide(new Vector2(0,0));
        for(int i=0; i<this.GetSlideCount(); i++){
            var collision = this.GetSlideCollision(i);
            INPC npc = null;
            if(collision.Collider is INPC){
                npc = (INPC)collision.Collider;}
            if(collision.Collider is NPCNonPhysicsCollider){
                var npcNonPhysCol = (NPCNonPhysicsCollider)collision.Collider;
                npc = npcNonPhysCol.INPCParent;}
            if(!(npc is null)){
                if(!this.colliderTimers.ContainsKey(npc)){
                   this.OnCollisionWith(npc);
                   this.colliderTimers[npc] = 0f;}}}
    }

    public abstract void OnCollisionWith(INPC npc);

}
