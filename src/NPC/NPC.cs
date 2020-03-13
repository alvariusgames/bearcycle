using Godot;
using System;
using System.Text.RegularExpressions;

public abstract class NPC<T> : FSMKinematicBody2D<T>, INPC{
    public abstract void GetHitBy(object node);
    public void DisplayExplosion(){
        var explosion = (Particles2D)(GD.Load("res://scenes/misc/effects/explosion1.tscn") as PackedScene).Instance();
        explosion.Emitting = true;
        explosion.OneShot = true;
        explosion.Position = this.Position;
        this.AddChild(explosion);
    }
    public String GetDisplayableName(){
        return this.RemoveNumbersAndTranslateNodeName();
    }

    public void PlayGetEatenSound(){
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this,
        new string[]{"res://media/samples/npc/meaty_enemy/geteaten1.wav",
                     "res://media/samples/npc/meaty_enemy/geteaten2.wav"});
    }

}