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
	protected float numSecOfInAir = 0f;
	private const int NUM_LAST_ACTUAL_STEPS = 5;
	private Queue<Vector2> lastActualSteps = new Queue<Vector2>();
	private Vector2 normalizedLastActualSteps { get {
		Vector2 aggr = new Vector2(0,0);
		foreach(var step in this.lastActualSteps){
			aggr += step;}
		return aggr / NUM_LAST_ACTUAL_STEPS;
	}}
	[Export]
	public NodePath FoodDisplayNodePath { get; set; }  

	public Sprite FoodDisplaySprite {set{} get {
		var copyOfSelf = (Ranger)this.Duplicate();
		return (Sprite)copyOfSelf.GetNode(copyOfSelf.FoodDisplayNodePath);}}

	public int Calories {get; set;} = 500;

	public virtual string GetDisplayableName(){
		return this.getDisplayableName();}

	public bool isConsumed {get; set;} = false;

	public bool IsMale = true;

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
			this.IsMale = result.IsMale;
			if(this.IsMale){
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

   public override void GetHitBy(Node node){
		/*
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
		}*/
		if(node is PlayerAttackWindow){
			this.DisplayExplosion(offset: new Vector2(0, -40f));
			this.CollisionShape2D.Disabled = true;
			this.BodyTiles.Visible = false;
			this.ResetActiveState(PursuingEnemyState.IDLE);
			this.PlayGetEatenSound();
			this.IsDestroyed = true;}}

	private float randomTriggerCounter = 0f;
	private float randomEventAwayOrTowardsTimeSec = 5f;
	public override void UpdateState(float delta){
		base.UpdateState(delta);
		switch(this.PursuitEnemyPattern){
			case PursuitEnemyPattern.FOLLOWING_PATH2D:
					this.randomTriggerCounter += delta;
					if(this.randomTriggerCounter > this.randomEventAwayOrTowardsTimeSec){
						//Generic 'freshness' -- move towards player randomly, sometimes away
						this.randomTriggerCounter = 0f;
						var n = (float)(new Random()).Next(0,10);
						if(n<8){
							this.SetActiveState(PursuingEnemyState.MOVING_TOWARD_PLAYER_GROUND, 100);}
						else{
							this.SetActiveState(PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND, 100);}}
					else if(Input.IsActionJustPressed("ui_attack") && this.ActiveStatePriority < 700){
						//if the player is attacking, give a 20% of running away
						var n = (float)(new Random()).Next(0,10);
						if(n>7){
							var prevState = this.ActiveState;
							this.SetActiveStateAfter(PursuingEnemyState.MOVING_AWAY_FROM_PLAYER_GROUND, 150, 0.5f);
							this.ResetActiveStateAfter(prevState, 2.5f);}}
				break;
			case PursuitEnemyPattern.STATIONARY_ATTACK_1:
			case PursuitEnemyPattern.STATIONARY_ATTACK_2:
				break;

		}
	}

	public override void ReactStateless(float delta){
		this.updateScaleFromDirectionToPlayer(delta);
		this.updateAnimation(delta);
		base.ReactStateless(delta);
	}
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

	public override void _Process(float delta){
		if(!this.IsDestroyed){
			base._Process(delta);}
	}

	public override void _PhysicsProcess(float delta)
	{
		if(!this.IsDestroyed){
			base._PhysicsProcess(delta);}
	}

	private void updateAnimation(float delta){
		if(Math.Abs(this.normalizedLastActualSteps.x) > 1){
			this.ActiveAnimationString = "run";}
		if(this.prevAnimStr != this.currentAnimStr){
			//Recently changed -- start playing the new animation
			this.AnimationPlayer.Play(this.ActiveAnimationString);}
	}


}
