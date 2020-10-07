using Godot;
using System;

public class NPCNonPhysicsCollider : KinematicBody2D, INPC{
    public bool ResetPlayerAttackWindowAfterGettingHit { 
        get {
            return this.INPCParent.ResetPlayerAttackWindowAfterGettingHit; } 
        set {
            this.INPCParent.ResetPlayerAttackWindowAfterGettingHit = value;}}

    public void GetHitBy(Node node){
        this.INPCParent.GetHitBy(node);}

    public INPC INPCParent {
        get {
            return (INPC)this.GetParent();}}
}
