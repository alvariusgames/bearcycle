using Godot;
using System;
using System.Text.RegularExpressions;

public enum LeapingEnemyState { LEAPING_FORWARD, LEAPING_BACKWARD, TRIGGER_GRAZING, GRAZING, UNTRIGGER_GRAZING, STANDING_STILL}

public class LeapingEnemy : NPC<LeapingEnemyState>, INPC, IFood
{
    public override LeapingEnemyState InitialState { get { return LeapingEnemyState.LEAPING_FORWARD;}set{}}

    [Export]
    public override int Calories { get; set; } = Food.FALLBACK_CALORIES;

    public override bool isConsumed { get; set; }

    private String cutOutScenePath = "res://scenes/npc/deer1/deer1_cutout.tscn";

    public override Sprite FoodDisplaySprite { get { 
        //TODO: implement logic from Ranger FoodDisplaySprite class so this is less hacky and works with other scenes etc
        var copyOfCutOut = ((PackedScene)GD.Load(this.cutOutScenePath)).Instance();
        foreach(Node child in copyOfCutOut.GetChildren()){
            if(child.Name.ToLower().Contains("root") && child is Sprite){
                var copyOfRootSprite = (Sprite)child;
                return copyOfRootSprite;
            }
        }
        return null;
    } set {}}

    private int leapSpeed = 250;
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
        this.CutOutAnimationPlayer.Play("leap1");
        this.initialCutOutScale = this.CutOut.GetScale();}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case LeapingEnemyState.LEAPING_FORWARD:
                this.PathFollow2D.SetOffset(this.PathFollow2D.GetOffset() + delta * leapSpeed);;
                this.CutOut.SetScale(this.initialCutOutScale);
                break;
            case LeapingEnemyState.LEAPING_BACKWARD:
                this.CutOut.SetScale(new Vector2(-this.initialCutOutScale.x,
                                                 this.initialCutOutScale.y));
                this.PathFollow2D.SetOffset(this.PathFollow2D.GetOffset() - delta * leapSpeed);;
                break;
            case LeapingEnemyState.STANDING_STILL:
                break;
        }
    }

    public override void GetHitBy(Node node){
        if(node is PlayerAttackWindow){
            this.CollisionShape2D.Disabled = true;
            this.CutOut.Visible = false;
            this.ResetActiveState(LeapingEnemyState.STANDING_STILL);
            this.DisplayExplosion();
            this.PlayGetEatenSound();
            }}
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

