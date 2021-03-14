using Godot;
using System;

public class SpaceRock : KinematicBody2D, IConsumeable, IFood
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private CollisionShape2D CollisionShape2D;
    private AnimatedSprite AnimatedSprite;
    public const int NUM_SPACE_ROCKS_IN_WHOLE_GAME = 12;
    [Export]
    public int RockNum = 0; 

    public override void _Ready(){
        foreach(Node2D child in this.GetChildren()){
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;}
            if(child is AnimatedSprite){
                this.AnimatedSprite = (AnimatedSprite)child;}}
        this.setToTransparentIfAlreadyCollected();}

    private void setToTransparentIfAlreadyCollected(){
        var rootLevel = this.GetRootLevelNode2DParent();
        rootLevel.HydrateSpaceRocks();
        if((this.RockNum == 1 && rootLevel.SpaceRock1Collected) ||
           (this.RockNum == 2 && rootLevel.SpaceRock2Collected) ||
           (this.RockNum == 3 && rootLevel.SpaceRock3Collected)){
               this.AnimatedSprite.SelfModulate = new Color(1f,1f,1f, 0.33f);}}

    private LevelNode2D GetRootLevelNode2DParent(Node node = null){
        if(node == null){
            node = this;}
        if(node is LevelNode2D){
            return (LevelNode2D)node;}
        else{
            return this.GetRootLevelNode2DParent(node.GetParent());}}

    public Sprite FoodDisplaySprite { 
    get {
        var s = new Sprite();
        s.Texture = (Texture)GD.Load("res://media/sprites/items/collectables/space_rock/icon.png");
        return s;} 
    set {}}

    public int Calories { get { return 0; } set {}}

    private Boolean _isConsumed = false;
    public Boolean isConsumed { get { return this._isConsumed;} set { this._isConsumed = value;}}

    public String GetDisplayableName() {
        return "???";
    }

    public void Consume(Player player){
        player.EatFood(this, false, false, false); //TODO: make this Absorb() or something
        SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                                                     "res://media/samples/items/special_sparkle1.wav",
                                                     PauseAllOtherSoundWhilePlaying: true);
        if(this.RockNum == 1){
            player.ActiveLevel.SpaceRock1Collected = true;}
        if(this.RockNum == 2){
            player.ActiveLevel.SpaceRock2Collected = true;}
        if(this.RockNum == 3){
             player.ActiveLevel.SpaceRock3Collected = true;}
        this.CollisionShape2D.Disabled = true;
        this.Visible = false;}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta){
        var mod = this.Modulate;
        mod.s = 0.5f;
        mod.h += delta;
        if(mod.h > 1){
            mod.h = 0;}
        this.Modulate = mod;
    }
//  {
//      
//  }
}
