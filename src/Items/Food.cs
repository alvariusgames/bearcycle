using Godot;
using System;
using System.Text.RegularExpressions;

public interface IFood{
    Sprite FoodDisplaySprite {get; set;}
    int Calories {get; set;}
    String GetDisplayableName();
    bool isConsumed{get;set;}

}

public class NonNodeFood : IFood{
    public Sprite FoodDisplaySprite{ get; set;}
    public int Calories{ get; set;}
    private String Name;
    public String GetDisplayableName(){
        return this.Name;
    }
    private bool _isConsumed = false;
    public bool isConsumed{ get{return this._isConsumed;} set{this._isConsumed = value;}}
    public NonNodeFood(Sprite FoodDisplaySprite, int Calories, String Name){
        this.FoodDisplaySprite = FoodDisplaySprite;
        this.Calories = Calories;
        this.Name = Name;
    }
}

public class Food : KinematicBody2D, IConsumeable, IFood{
    public const int FALLBACK_CALORIES = 500;
    public Sprite FoodDisplaySprite{get;set;}
    [Export]
    public int Calories{get;set;} = FALLBACK_CALORIES;
    public bool isConsumed{get;set;} = false;
    private CollisionShape2D CollisionShape2D;

    public String GetDisplayableName(){
        return this.RemoveNumbersAndTranslateNodeName();
    }

    public override void _Ready(){
        foreach(var child in this.GetChildren()){
            if(child is Sprite){
                this.FoodDisplaySprite = (Sprite)child;
            }
            else if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;}}}

    public void consume(Node2D collider){
        if(collider is WholeBodyKinBody){
            var player = ((WholeBodyKinBody)collider).Player;
            player.EatFood(this);
            this.CollisionShape2D.Disabled = true;
            this.FoodDisplaySprite.Visible = false;}}}
