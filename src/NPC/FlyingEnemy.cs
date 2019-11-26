using Godot;
using System;
using System.Text.RegularExpressions;

public enum FlyingEnemyState { FLYING_DEFAULT }

public class FlyingEnemy : FSMKinematicBody2D<FlyingEnemyState>, INPC, IFood
{
    public override FlyingEnemyState InitialState { get { return FlyingEnemyState.FLYING_DEFAULT;}}

    public float Calories { get; set; }
    public String GetDisplayableName(){
        var name = this.GetName();
        string pattern = @"\d+$"; //find numbers at end of string
        string replacement = "";
        Regex rgx = new Regex(pattern);
        return rgx.Replace(name, replacement);
    }

    public bool isConsumed { get; set; }

    private String cutOutScenePath = "res://scenes/npc/bird1/bird1_cutout.tscn";

    public Sprite FoodDisplaySprite { get { 
        var copyOfCutOut = ((PackedScene)GD.Load(this.cutOutScenePath)).Instance();
        foreach(Node child in copyOfCutOut.GetChildren()){
            if(child.Name.ToLower().Contains("root") && child is Sprite){
                var copyOfRootSprite = (Sprite)child;
                //copyOfRootSprite.SetScale(new Vector2(0.00075f, 0.00075f));
                return copyOfRootSprite;
            }
        }
        return null;
    } set {}}

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
                        this.RootSprite = (Sprite)child2;
                        try{
                            this.Calories = 700; //TODO: make me dynamic
                        } catch(Exception e){
                            this.Calories = Food.FALLBACK_CALORIES;}}}}}
        this.CutOutAnimationPlayer.Play("flap1");
        this.initialCutOutScale = this.CutOut.GetScale();}

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
        }
    }

    public void GetHitBy(object node){
        if(node is AttackWindow){
            GD.Print("Hit an attack window");
            this.CollisionShape2D.Disabled = true;
            this.CutOut.Visible = false;}}
    public override void ReactStateless(float delta){

    }

    public override void UpdateState(float delta){

    }



}
