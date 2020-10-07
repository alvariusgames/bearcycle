using Godot;
using System;

public class TurnaroundZone : NPCCollisionZone{
    public override void OnCollisionWith(INPC npc){
        if(npc is PursuingEnemy){
            var pursEnemy = (PursuingEnemy)npc;
            pursEnemy.TurnAround();}
    }

}
