using Godot;
using System;
using System.Text.RegularExpressions;
public static class NPCExtensions{
    //Slightly complex workaround for having common logic in FSMKinematicBody NPC and FSMRigidBody NPC
    private static Vector2 ZERO_VEC2 = new Vector2(0f,0f); 
    public static void DisplayExplosion(this INPC npcObj, Vector2? offset = null){
        if(offset is null){
            offset = new Vector2(0,0);}
        var explosion = (Particles2D)(GD.Load("res://scenes/misc/effects/explosion1.tscn") as PackedScene).Instance();
        explosion.Emitting = true;
        explosion.OneShot = true;
        npcObj.GetParent().AddChild(explosion);
        explosion.GlobalPosition = npcObj.GlobalPosition + (Vector2)offset;}

    public static void PlayGetEatenSound(this INPC npcObj){
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>((Node)npcObj,
        new string[]{"res://media/samples/npc/meaty_enemy/geteaten1.wav",
                     "res://media/samples/npc/meaty_enemy/geteaten2.wav"});}

    public static String getDisplayableName(this INPC npcObj){
        return ((Node)npcObj).RemoveNumbersAndTranslateNodeName();}
}

public abstract class NPC<T> : FSMKinematicBody2D<T>, INPC, IFood{
    [Export]
    public bool ResetPlayerAttackWindowAfterGettingHit {get; set;} = true;
    public abstract void GetHitBy(Node node);
    public abstract Sprite FoodDisplaySprite {get; set;}
    public abstract int Calories {get; set;}
    public abstract bool isConsumed {get; set;}
    public String GetDisplayableName(){
        return this.getDisplayableName();}
}