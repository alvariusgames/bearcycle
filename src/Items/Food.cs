using Godot;
using System;
using System.Text.RegularExpressions;

public interface IFood{
    Sprite FoodDisplaySprite {get; set;}
    float Calories {get; set;}
    String GetDisplayableName();
    bool isConsumed{get;set;}

}

public class NonNodeFood : IFood{
    public Sprite FoodDisplaySprite{ get; set;}
    public float Calories{ get; set;}
    private String Name;
    public String GetDisplayableName(){
        return this.Name;
    }
    private bool _isConsumed = false;
    public bool isConsumed{ get{return this._isConsumed;} set{this._isConsumed = value;}}
    public NonNodeFood(Sprite FoodDisplaySprite, float Calories, String Name){
        this.FoodDisplaySprite = FoodDisplaySprite;
        this.Calories = Calories;
        this.Name = Name;
    }
}

public class Food : KinematicBody2D, IConsumeable, IFood{
    public const float FALLBACK_CALORIES = 500f;
    public Sprite FoodDisplaySprite{get;set;}
    public float Calories{get;set;}
    public bool isConsumed{get;set;} = false;
    private CollisionShape2D CollisionShape2D;

    public String GetDisplayableName(){
        var name = this.GetName();
        string pattern = @"\d+$"; //find numbers at end of string
        string replacement = "";
        Regex rgx = new Regex(pattern);
        return rgx.Replace(name, replacement);
    }

    public override void _Ready(){
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.FoodDisplaySprite = (Sprite)child;
                try{
                    this.Calories = (float)Convert.ToDouble(this.FoodDisplaySprite.Name);
                } catch(Exception e){
                    this.Calories = FALLBACK_CALORIES;
                }
            }
            else if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;}}}

    public void consume(Node2D collider){
        if(collider is WholeBodyKinBody){
            var player = ((WholeBodyKinBody)collider).Player;
            player.EatFood(this);
            this.CollisionShape2D.Disabled = true;
            this.FoodDisplaySprite.Visible = false;}}}
