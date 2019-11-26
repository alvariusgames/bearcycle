using Godot;
using System;
using System.Text.RegularExpressions;

public enum LeapingEnemyState { LEAPING_FORWARD, LEAPING_BACKWARD, TRIGGER_GRAZING, GRAZING, UNTRIGGER_GRAZING}

public class LeapingEnemy : FSMKinematicBody2D<LeapingEnemyState>, INPC, IFood
{
    public override LeapingEnemyState InitialState { get { return LeapingEnemyState.LEAPING_FORWARD;}}

    public float Calories { get; set; }
    public String GetDisplayableName(){
        var name = this.GetName();
        string pattern = @"\d+$"; //find numbers at end of string
        string replacement = "";
        Regex rgx = new Regex(pattern);
        return rgx.Replace(name, replacement);
    }

    public bool isConsumed { get; set; }

    private String cutOutScenePath = "res://scenes/npc/deer1/deer1_cutout.tscn";

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

    private int leepSpeed = 250;
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
                            this.Calories = 1400; //TODO: make me dynamic
                        } catch(Exception e){
                            this.Calories = Food.FALLBACK_CALORIES;}}}}}
        this.CutOutAnimationPlayer.Play("leap1");
        this.initialCutOutScale = this.CutOut.GetScale();}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case LeapingEnemyState.LEAPING_FORWARD:
                this.PathFollow2D.SetOffset(this.PathFollow2D.GetOffset() + delta * leepSpeed);;
                this.CutOut.SetScale(this.initialCutOutScale);
                break;
            case LeapingEnemyState.LEAPING_BACKWARD:
                this.CutOut.SetScale(new Vector2(-this.initialCutOutScale.x,
                                                 this.initialCutOutScale.y));
                this.PathFollow2D.SetOffset(this.PathFollow2D.GetOffset() - delta * leepSpeed);;
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
        if(this.ActiveState.Equals(LeapingEnemyState.LEAPING_FORWARD) ||
           this.ActiveState.Equals(LeapingEnemyState.LEAPING_BACKWARD)){
            if(this.PathFollow2D.UnitOffset == 1){
                this.ResetActiveState(LeapingEnemyState.LEAPING_BACKWARD);}
            if(this.PathFollow2D.UnitOffset == 0){
                this.ResetActiveState(LeapingEnemyState.LEAPING_FORWARD);}}
        
    }



}

