using Godot;
using System;
using System.Text.RegularExpressions;

public enum FlyingEnemyState { FLYING_DEFAULT, HOVERING_STILL }

public class FlyingEnemy : NPC<FlyingEnemyState>, INPC, IFood
{
    public override FlyingEnemyState InitialState { get { return FlyingEnemyState.FLYING_DEFAULT;}set{}}

    [Export]
    public override int Calories { get; set; } = Food.FALLBACK_CALORIES;

    public override bool isConsumed { get; set; }
    public Sprite foodDisplaySprite;
    public void setUpFoodDisplaySprite() { 
        var cutOutScenePath = this.CutOut.Filename;      
        var copyOfCutOut = ((PackedScene)GD.Load(cutOutScenePath)).Instance();
        foreach(Node child in copyOfCutOut.GetChildren()){
            if(child.Name.ToLower().Contains("root") && child is Sprite){
                var copyOfRootSprite = (Sprite)child;
                this.foodDisplaySprite = copyOfRootSprite;
            }
        }
    }

    public override Sprite FoodDisplaySprite { get { return this.foodDisplaySprite;} set{}}

    private int flySpeed = 350;
    public PathFollow2D PathFollow2D;

    public Node2D CutOut;
    private Vector2 initialCutOutScale;

    public AnimationPlayer CutOutAnimationPlayer;

    public CollisionShape2D CollisionShape2D;
    public Sprite RootSprite;

    public override void _Ready(){
        this.isConsumed = false;
        this.PathFollow2D = (PathFollow2D)this.GetParent();
        foreach(var child in this.GetChildren()){
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
            }
            if(((Node2D)child).Name.ToLower().Contains("cutout")){
                this.CutOut = (Node2D)child;
                foreach(var child2 in this.CutOut.GetChildren()){
                    if(child2 is AnimationPlayer){
                        this.CutOutAnimationPlayer = (AnimationPlayer)child2;}
                    else if(((Node2D)child2).Name.Contains("root") && 
                             (child2 is Sprite)){
                        this.RootSprite = (Sprite)child2;}}}}
        this.CutOutAnimationPlayer.Play("flap1");
        this.initialCutOutScale = this.CutOut.GetScale();
        this.setUpFoodDisplaySprite();}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case FlyingEnemyState.FLYING_DEFAULT:
                this.PathFollow2D.SetOffset(this.PathFollow2D.GetOffset() + delta * flySpeed);;
                if((this.PathFollow2D.Rotation < (-Math.PI / 2f)) || 
                   (this.PathFollow2D.Rotation > (Math.PI / 2f))){
                    this.CutOut.SetScale(new Vector2(-this.initialCutOutScale.x,
                                                     this.initialCutOutScale.y));
                    this.CutOut.SetRotation((float)Math.PI);
                } else {
                    this.CutOut.SetScale(this.initialCutOutScale);
                    this.CutOut.SetRotation(0f);
                }
                break;
            case FlyingEnemyState.HOVERING_STILL:
                break;
        }
    }

    public override void GetHitBy(Node node){
        if(node is PlayerAttackWindow){
            GD.Print("Hit an attack window");
            this.CollisionShape2D.Disabled = true;
            this.CutOut.Visible = false;
            this.ResetActiveState(FlyingEnemyState.HOVERING_STILL);
            this.DisplayExplosion();
            this.PlayGetEatenSound();}}
    public override void ReactStateless(float delta){}

    public override void UpdateState(float delta){}

}