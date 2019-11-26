using Godot;
using System;

public class SpaceRock : KinematicBody2D, IConsumeable, IFood
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private CollisionShape2D CollisionShape2D;

    public override void _Ready(){
        foreach(Node2D child in this.GetChildren()){
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
            }
        }
    }

    public Sprite FoodDisplaySprite { 
    get {
        var s = new Sprite();
        s.Texture = (Texture)GD.Load("res://media/sprites/items/collectables/space_rock/icon.png");
        return s;} 
    set {}}

    public float Calories { get { return 0f; } set {}}

    private Boolean _isConsumed = false;
    public Boolean isConsumed { get { return this._isConsumed;} set { this._isConsumed = value;}}

    public String GetDisplayableName() {
        return "???";
    }

    public void consume(Node2D collider){
        if(collider is WholeBodyKinBody){
            var player = ((WholeBodyKinBody)collider).Player;
            player.EatFood(this); //TODO: make this Absorb() or something
            if(this.Name.Contains("1")){
                player.ActiveLevel.SpaceRock1Collected = true;}
            if(this.Name.Contains("2")){
                player.ActiveLevel.SpaceRock2Collected = true;}
            if(this.Name.Contains("3")){
                player.ActiveLevel.SpaceRock3Collected = true;}
            this.CollisionShape2D.Disabled = true;
            this.Visible = false;}}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
