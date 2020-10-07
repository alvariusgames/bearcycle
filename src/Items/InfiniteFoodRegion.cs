using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class InfiniteFoodRegion : KinematicBody2D, IInteractable
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public int InteractPriority { get { return 100;}}

    private List<Node2D> Bundles = new List<Node2D>();

    private Sprite FoodIconSprite;
    private string FoodDisplayName;
    [Export]
    public int lootableCalories {get; set;} = Food.FALLBACK_CALORIES;
    private float lootedCalories = 0f;
    private const int NUM_BITES = 3;

    // Called when the node enters the scene tree for the first time.

    public String GetDisplayableName(){
        return this.RemoveNumbersAndTranslateNodeName();
    }


    public override void _Ready(){
        this.FoodDisplayName = this.GetDisplayableName();
        foreach(Node2D child in this.GetChildren()){
            if(child is CollisionShape2D){
                foreach(Node2D child2 in child.GetChildren() ){
                    this.Bundles.Add(child2);}}
            if(child is Sprite){
                if(child.Name.ToLower().Contains("foodicon")){
                    this.FoodIconSprite = (Sprite)child;}}}
        this.scrambleBundles();}

    private void scrambleBundles(){
        for(int i = 0; i<this.Bundles.Count; i++){
            var bundle = this.Bundles[i];
            if(bundle is Sprite){
                var sprite = (Sprite)bundle;
                var rand = new Random();
                if( rand.Next(1) == 0){
                    var randSprite = this.Bundles[rand.Next(this.Bundles.Count)];
                    var randSpritePos = randSprite.GetGlobalPosition();
                    randSprite.SetGlobalPosition(sprite.GetGlobalPosition());
                    sprite.SetGlobalPosition(randSpritePos);}}}}

    public void InteractWith(Player Player){
        var caloriesInThisBite = this.lootableCalories / NUM_BITES;
        var food = new NonNodeFood((Sprite)this.FoodIconSprite.Duplicate(),
                                    caloriesInThisBite, this.FoodDisplayName);
        this.lootedCalories += caloriesInThisBite;
        if(this.lootedCalories <= this.lootableCalories){
            Player.EatFood(food);}
        else {
            Player.EatFood(food, updateTotalCalories: false);}}}
