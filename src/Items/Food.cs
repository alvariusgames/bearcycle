using Godot;
using System;

public interface IFood{
    Sprite Sprite {get; set;}
    float Calories {get; set;}
    String Name{get;set;}
    bool isConsumed{get;set;}

}

public class Food : KinematicBody2D, IConsumeable, IFood{
    private const float FALLBACK_CALORIES = 500f;
    public Sprite Sprite{get;set;}
    public float Calories{get;set;}
    public bool isConsumed{get;set;} = false;
    private CollisionShape2D CollisionShape2D;

    public override void _Ready(){
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.Sprite = (Sprite)child;
                try{
                    this.Calories = (float)Convert.ToDouble(this.Sprite.Name);
                } catch(Exception e){
                    this.Calories = FALLBACK_CALORIES;
                }
            }
            else if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;}}}

    public void consume(Node2D collider){
        Player player = null;
        if(collider is Bear){
            player = ((Bear)collider).ATV.Player;}
        if(collider is Wheel){
            player = ((Wheel)collider).ATV.Player;}
        if(player != null){
            player.EatFood(this);
            this.CollisionShape2D.Disabled = true;
            this.Sprite.Visible = false;
            this.isConsumed = true;}}}
