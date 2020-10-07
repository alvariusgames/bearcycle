using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class PursuingHuman : PursuingEnemy, ISpawnable{
    public CollisionShape2D CollisionShape2D;
    private Vector2 OriginalCollisionShape2DScale = new Vector2(1f,1f);
    public Node2D BodyTiles;
    private Vector2 OriginalBodyTilesScale = new Vector2(1f,1f);
    [Export]
    public Godot.Collections.Array<NodePath> SkinSprites {get; set;}
    private List<Node2D> skinSpritesInst = new List<Node2D>();
    [Export]
    public Godot.Collections.Array<NodePath> HairSprites {get; set;}
    private List<Node2D> hairSpritesInst = new List<Node2D>();
    [Export]
    public Godot.Collections.Array<NodePath> MaleSprites {get; set;}
    private List<Node2D> maleSpritesInst = new List<Node2D>();
    [Export]
    public Godot.Collections.Array<NodePath> FemaleSprites {get; set;}
    private List<Node2D> femaleSpritesInst = new List<Node2D>();
    [Export]
    public bool Diversify {get; set;} = true;
    public AnimationPlayer AnimationPlayer;

    [Export]
    public NodePath NPCAttackWindowPath;
    public NPCAttackWindow NPCAttackWindow;
    public bool IsDestroyed {get; set;} = false;

    public override float MaxForwardAccel {get; set;} = 100f;
    protected override float MaxBackwardAccel {get; set;} = -50f;
    protected override float ForwardAccelUnit {get; set;} = 3f;
    protected float numSecOfInAir = 0f;
    private const int NUM_LAST_ACTUAL_STEPS = 5;
    private Queue<Vector2> lastActualSteps = new Queue<Vector2>();
    private Vector2 normalizedLastActualSteps { get {
        Vector2 aggr = new Vector2(0,0);
        foreach(var step in this.lastActualSteps){
            aggr += step;}
        return aggr / NUM_LAST_ACTUAL_STEPS;
    }}
    protected String prevAnimStr = "idle";
    protected String currentAnimStr = "idle";

    public String ActiveAnimationString { 
        get { return this.currentAnimStr;}
        set { this.prevAnimStr = this.currentAnimStr;
              this.currentAnimStr = value;}}

    [Export]
    public NodePath FoodDisplayNodePath { get; set; }  

    public Sprite FoodDisplaySprite {set{} get {
        var copyOfSelf = (Ranger)this.Duplicate();
        return (Sprite)copyOfSelf.GetNode(copyOfSelf.FoodDisplayNodePath);}}

    public int Calories {get; set;} = 1000;

    public virtual string GetDisplayableName(){
        return this.getDisplayableName();}

    public bool isConsumed {get; set;} = false;

    private Vector2 towardPlayer { get {
        try{
            return (this.PlayerToPursue.ATV.GetGlobalCenterOfTwoWheels() - this.GlobalPosition).Normalized(); }
        catch{
            return new Vector2(0,0);}}}

    private Boolean playerIsForward { get {
        return this.towardPlayer.x > 0;}}

    public void PostDuplicate(Node2D masterTemplate){
        var template = (PursuingHuman)masterTemplate;
        this.NPCAttackWindow.InitialCollisionLayer = 
            template.NPCAttackWindow.InitialCollisionLayer;
        this.NPCAttackWindow.InitialCollisionMask = 
            template.NPCAttackWindow.InitialCollisionMask;
    }
    public override void _Ready(){
        base._Ready();
        foreach(Node child in this.GetChildren()){
            if(child.Name.ToLower().Contains("tiles")){
                this.BodyTiles = (Node2D)child;
                this.OriginalBodyTilesScale = this.BodyTiles.Scale;}
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
                this.OriginalCollisionShape2DScale = this.CollisionShape2D.Scale;}
            if(child is AnimationPlayer){
                this.AnimationPlayer = (AnimationPlayer)child;}}
        foreach(NodePath skinSpritePath in this.SkinSprites){
            this.skinSpritesInst.Add((Node2D)this.GetNode(skinSpritePath));}
        foreach(NodePath hairSpritePath in this.HairSprites){
            this.hairSpritesInst.Add((Node2D)this.GetNode(hairSpritePath));}
        foreach(NodePath maleSpritePath in this.MaleSprites){
            this.maleSpritesInst.Add((Node2D)this.GetNode(maleSpritePath));}
        foreach(NodePath femaleSpritePath in this.FemaleSprites){
            this.femaleSpritesInst.Add((Node2D)this.GetNode(femaleSpritePath));}
        this.AnimationPlayer.Play("idle");
        this.NPCAttackWindow = (NPCAttackWindow)this.GetNode(this.NPCAttackWindowPath);
        for(int i=0;i<NUM_LAST_ACTUAL_STEPS; i++){
            this.lastActualSteps.Enqueue(new Vector2(0,0));}
        if(this.Diversify){
            var result = PCComplianceFactory.FillDiversityQuota(this.BodyTiles, this.skinSpritesInst,
                                                                this.hairSpritesInst, this.maleSpritesInst, 
                                                                this.femaleSpritesInst);
            if(result.IsMale){
                this.MakeMaleSize();}
            else{
                this.MakeFemaleSize();}}}

    public void MakeMaleSize(){
        this.BodyTiles.Scale = this.OriginalBodyTilesScale;
        this.CollisionShape2D.Scale = this.OriginalCollisionShape2DScale;
    }

    public void MakeFemaleSize(){
        this.BodyTiles.Scale = new Vector2(0.9f * this.OriginalBodyTilesScale.x,
                                           0.9f * this.OriginalBodyTilesScale.y);
        this.CollisionShape2D.Scale = new Vector2(0.9f * this.OriginalCollisionShape2DScale.x,
                                                  0.9f * this.OriginalCollisionShape2DScale.y);
    }

    public override void PursueIdle(float delta){
        this.forwardAccell *= 0.9f;
        this.ActiveAnimationString = "idle";}

    private void accellerateForward(float delta){
        if(this.forwardAccell <= this.MaxForwardAccel){  
            this.forwardAccell += this.ForwardAccelUnit * delta * main.DELTA_NORMALIZER;}}

    private void accellerateBackward(float delta){
        if(this.forwardAccell >= this.MaxBackwardAccel){
            this.forwardAccell -= this.ForwardAccelUnit * delta * main.DELTA_NORMALIZER;}}

    public override void PursueMovingTowardPlayerGround(float delta){
        this.ActiveAnimationString = "idle";
        if(this.playerIsForward){
            this.accellerateForward(delta);}
        else{
            this.accellerateBackward(delta);}}

    public override void PursueMovingAwayPlayerGround(float delta){
        this.ActiveAnimationString = "idle";
        if(this.playerIsForward){
            this.accellerateBackward(delta);}
        else{
            this.accellerateForward(delta);}}

    public override void GetHitBy(Node node){
        if(node is PursuingHuman){
            var h = (PursuingHuman)node;
            if(this.ActiveState.Equals(PursuingEnemyState.ATTACKING)){
                // Don't do anything if we're attacking, this is a higher priority
            }
            else if(h.ActiveState.Equals(PursuingEnemyState.ATTACKING)){
                // If who you're running into is attacking, joing in on the fun
                this.SetActiveState(PursuingEnemyState.ATTACKING, 150);
            }
            else {
                // If we're running toward or away, pick an arbitrary node and take their state
                PursuingHuman higherPriorityNode;
                if(String.Compare(this.Name, h.Name) < 0){
                    higherPriorityNode = this;}
                 else{
                     higherPriorityNode = h;}
                this.SetActiveState(higherPriorityNode.ActiveState, 150);
            }
        }
        if(node is PlayerAttackWindow){
            this.DisplayExplosion(offset: new Vector2(0, -40f));
            this.CollisionShape2D.Disabled = true;
            this.BodyTiles.Visible = false;
            this.ResetActiveState(PursuingEnemyState.IDLE);
            this.PlayGetEatenSound();
            this.IsDestroyed = true;}}
    private void updateLastInAirs(float delta){
        var inAir = this.IsInAir();
        if(inAir){
            this.numSecOfInAir += delta; }
        else {
            this.numSecOfInAir = 0f;}}
    public Boolean IsInAirNormalized(float numSecondsToCheckInAir = -1){
        if(numSecondsToCheckInAir == -1){
            numSecondsToCheckInAir = 0.25f;}
        return this.numSecOfInAir >= numSecondsToCheckInAir;}

    private float randomTriggerCounter = 0f;
    private float randomEventAwayOrTowardsTimeSec = 5f;
    public override void UpdateState(float delta){
        base.UpdateState(delta);
        switch(this.PursuitPattern){
            case EnemyPursuePattern.AUTO_1:
                    this.randomTriggerCounter += delta;
                    if(this.randomTriggerCounter > this.randomEventAwayOrTowardsTimeSec){
                        this.randomTriggerCounter = 0f;
                        var n = (float)(new Random()).Next(0,10);
                        if(n<8){
                            this.SetActiveState(PursuingEnemyState.MOVING_TOWARD_PLAYER_GROUND, 100);}
                        else{
                            this.SetActiveState(PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND, 100);}
                    }
                    if(Input.IsActionJustPressed("ui_attack") && this.ActiveStatePriority < 700){
                        var prevState = this.ActiveState;
                        this.SetActiveStateAfter(PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND, 150, 0.5f);
                        this.ResetActiveStateAfter(prevState, 2.5f);}
                break;
            case EnemyPursuePattern.STATIONARY_ATACK_1:
                this.ResetActiveState(PursuingEnemyState.ATTACKING);
                this.velocity.x = 0f;
                break;
        }
    }

    public override void ReactStateless(float delta){
        this.applyGravity(delta);
        this.reactToSlideCollision(delta);
        var lastStep = this.GlobalPosition;
        this.MoveAndSlide(this.velocity, this.currentNormal);
        lastStep -= this.GlobalPosition;
        this.trackLastLastSteps(lastStep);
        this.updateScaleFromDirectionToPlayer(delta);
        this.updateAnimation(delta);
        this.updateLastInAirs(delta);
    }

    private void trackLastLastSteps(Vector2 lastStep){
        this.lastActualSteps.Dequeue();
        this.lastActualSteps.Enqueue(lastStep);}

    private void updateScaleFromDirectionToPlayer(float delta){
        float directionMultiplier = 1;
        if(this.ActiveState.Equals(PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND)){
            directionMultiplier = -1;}
        if(this.towardPlayer.x > 0){
            //Player is to the right of the current enemy
            this.BodyTiles.Scale = new Vector2(directionMultiplier * Math.Abs(this.BodyTiles.Scale.x),
                                               this.BodyTiles.Scale.y);
            this.NPCAttackWindow.Scale = new Vector2(directionMultiplier * Math.Abs(this.NPCAttackWindow.Scale.x),
                                                  this.BodyTiles.Scale.y);
        } else {
            //Player is to the left of the current enemy
            this.BodyTiles.Scale = new Vector2(-directionMultiplier * Math.Abs(this.BodyTiles.Scale.x),
                                               this.BodyTiles.Scale.y);
            this.NPCAttackWindow.Scale = new Vector2(-directionMultiplier * Math.Abs(this.NPCAttackWindow.Scale.x),
                                                  this.NPCAttackWindow.Scale.y);}}

    private void updateAnimation(float delta){
        if(this.IsInAirNormalized()){
            this.ActiveAnimationString = "jump_down";}
        if(Math.Abs(this.normalizedLastActualSteps.x) > 1){
            this.ActiveAnimationString = "run";}
        if(this.prevAnimStr != this.currentAnimStr){
            //Recently changed -- start playing the new animation
            this.AnimationPlayer.Play(this.ActiveAnimationString);}
    }


}
