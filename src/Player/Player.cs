using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum PlayerState {ALIVE, TRIGGER_DIEING, DIEING_TEMP_FREEZE, TRIGGER_DIEING_FLIP, DIEING_FLIP_OFF_SCREEN, END_DIEING}
public class Player : FSMNode2D<PlayerState>{
    public ILevel ActiveLevel;
    public LevelFrame LevelFrame;
    public override PlayerState InitialState {get { return PlayerState.ALIVE;}}
    public AttackWindow AttackWindow;

    public ClawAttack ClawAttack;
    public List<IHoldable> AllHoldibles = new List<IHoldable>(); //TODO: Make me a LIFO Stack
    public IHoldable ActiveHoldable;

    public ATV ATV;
    public AboveHeadManager AboveHeadManager;
    public const float MAX_HEALTH = 100;
    private const float HEALTH_TO_CALORIES_RATIO = 1f/100f;
    private const float HEALTH_LOSS_DIVISOR = 1.5f;
    public const float DEFAULT_HIT_UNIT = -6f;
    public const int DEFAULT_NUM_LIVES = 2; //0 based index
    public float CurrentHealth {get; set;} = MAX_HEALTH;
    private const float HEALTH_DANGER_PERC = 0.25f;
    public Boolean IsInHealthDanger{ get { return this.CurrentHealth <= MAX_HEALTH * HEALTH_DANGER_PERC;}}
    public int TotalCalories = 0;
    private List<IFood> foodEaten = new List<IFood>();
    public IFood lastFoodEaten;
    private SafetyCheckPoint LastSafetyCheckPoint;
    public WholeBodyKinBody WholeBodyKinBody;
    private float animationTimer = 0f;
    private string animationToPlayAfterTimer;
    private float animationToPlayAfterThreshold = 0f;
    public int NumLives;
    public int NumContinues;
    private float transitionToDeathModCounter = 0f;

    public override void _Ready(){
        this.ResetActiveState(this.InitialState);
        this.setInitialSafetyCheckPoint();
        this.NumLives = DbHandler.ActiveSlot.NumLives;
        this.NumContinues = DbHandler.ActiveSlot.NumContinuesUsed;
        foreach(var child in this.GetChildren()){
            if(child is ATV){
                this.ATV = (ATV)child;}
            else if(child is AboveHeadManager){
                this.AboveHeadManager = (AboveHeadManager)child;}
            else if(child is AttackWindow){
                this.AttackWindow = (AttackWindow)child;}
            else if(child is WholeBodyKinBody){
                this.WholeBodyKinBody = (WholeBodyKinBody)child;}
            else if(child is ClawAttack){
                this.ClawAttack = (ClawAttack)child;
                this.PickupHoldable(this.ClawAttack);
            }}
        this.ATV.Bear.AnimationPlayer.AdvancedPlay(new String[]{"idleBounce1"});}

    private void setInitialSafetyCheckPoint(){
       this.LastSafetyCheckPoint = new SafetyCheckPoint();
       this.LastSafetyCheckPoint.GlobalPositionToResetTo = this.GetGlobalPosition();
       this.LastSafetyCheckPoint.BeenActivated = true;
    }

    public override void ReactStateless(float delta){
        this.checkAnimationTimer(delta);
    }

    private void checkAnimationTimer(float delta){
        if(this.animationToPlayAfterTimer != null){
            this.animationTimer += delta;
            if(this.animationTimer >= this.animationToPlayAfterThreshold){
                this.ATV.Bear.AnimationPlayer.AdvancedPlay(this.animationToPlayAfterTimer);
                this.animationTimer = 0f;
                this.animationToPlayAfterTimer = null;}}
    }

    private void updateHealthCheckDeath(float delta){
        this.CurrentHealth -= delta / HEALTH_LOSS_DIVISOR;
        if(this.ActiveHoldable != null && this.ActiveHoldable.IsDepleted){
            this.DropActiveHoldable(delta);
        }
        if(this.CurrentHealth <= 5f){
            //TODO: MAKE ME MORE ELEGANT AND SHOW THE BEAR DIE ANIMATION
            //TODO: FIND OUT WHY BAR IS EMPTY at 5 HEALTH AND NOT 0
            this.Die();
        }
    }

    public void Die(){
        this.CurrentHealth = 0f;
        this.ResetActiveState(PlayerState.TRIGGER_DIEING);
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case PlayerState.ALIVE:
                this.updateHealthCheckDeath(delta);
                break;
            case PlayerState.TRIGGER_DIEING:
                this.ATV.ReattachBear();
                this.ATV.Bear.AnimationPlayer.AdvancedPlay(new String[]{"idleBounce1"});
                this.AboveHeadManager.HackyClearAllAboveHead();
                // Make everything non-collideable

                //Tell camera to stop moving
                var throwawayNode = new Node2D();
                throwawayNode.GlobalPosition = this.ATV.GetDeFactoGlobalPosition();
                this.GetParent().AddChild(throwawayNode);
                this.MoveCameraTo(throwawayNode, new Vector2(0,0), 0.5f);

                //Stop other miscellaneous things
                this.ClawAttack.SetActiveState(ClawAttackState.LOCKED, 999);
                SoundHandler.StopAllSample();
                SoundHandler.StopAllStream();
                this.ATV.FrontWheel.StopAllEngineSounds();
                this.ATV.tempStopAllMovement();
                this.ATV.FrontWheel.ResetActiveState(WheelState.LOCKED);
                this.ATV.BackWheel.ResetActiveState(WheelState.LOCKED);
                this.ATV.SetAccellOfTwoWheels(0f);
                this.ATV.SetVelocityOfTwoWheels(new Vector2(0,0));
                this.ATV.RotationManager.SetActiveState(RotationManagerState.NOT_ROTATING, 999);
                this.LevelFrame = ((LevelNode2D)this.ActiveLevel).TryGetLevelFrame();
                this.LevelFrame.PlayerStatsDisplayHandler.LevelFrameBannerBase.PlaySoundWhenBlinking = false;
                SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                                                            new String[]{"res://media/samples/player/die1.wav"});
                this.ResetActiveState(PlayerState.DIEING_TEMP_FREEZE);
                this.ResetActiveStateAfter(PlayerState.TRIGGER_DIEING_FLIP, 1f);
                break;
            case PlayerState.DIEING_TEMP_FREEZE:
                break;
            case PlayerState.TRIGGER_DIEING_FLIP:
                //Throw player up at initial speed and start dieing process
                this.ATV.resumeMovement();
                const int DIE_THROW_SPEED = 1000;
                this.ATV.FrontWheel.CollisionLayer = 0;
                this.ATV.FrontWheel.CollisionMask = 0;
                this.ATV.BackWheel.CollisionLayer = 0;
                this.ATV.BackWheel.CollisionMask = 0;
                this.ATV.Bear.CollisionLayer = 0;
                this.ATV.Bear.CollisionMask = 0; 
                SoundHandler.PlayStream<MyAudioStreamPlayer>(this,
                                                             new String[]{"res://media/music/misc/die1.ogg"});
                SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this,
                                                             new string[]{"res://media/samples/bear/whimper1.wav"},
                                                             VolumeMultiplier: 0.01f);
                this.ATV.SetVelocityOfTwoWheels(new Vector2(0,-DIE_THROW_SPEED));
                this.transitionToDeathModCounter = 0f;
                this.ResetActiveState(PlayerState.DIEING_FLIP_OFF_SCREEN);
                this.ResetActiveStateAfter(PlayerState.END_DIEING, 5f);
                break;
            case PlayerState.DIEING_FLIP_OFF_SCREEN:
                this.ATV.RotateTwoWheels(0.1f, delta);
                this.transitionToDeathModCounter += delta / 10f;
                ((LevelNode2D)this.ActiveLevel).ChangeLevelModulate(PlayerModulate: new Color(1f,1f,1f),
                                                                    PlayerModTransThresh: 1,
                                                                    BackgroundModulate: new Color(0f,0f,0f),
                                                                    BgModTransThresh: this.transitionToDeathModCounter,
                                                                    EverythingElseModulate: new Color(0f,0f,0f),
                                                                    EvElseTransThresh: this.transitionToDeathModCounter / 5f);
                this.LevelFrame.PlayerStatsDisplayHandler.Modulate = 
                    this.LevelFrame.PlayerStatsDisplayHandler.Modulate * (1-this.transitionToDeathModCounter) + 
                    ((new Color(0f,0f,0f) * this.transitionToDeathModCounter));
                break;
            case PlayerState.END_DIEING:
                if(this.NumLives <= 0){
                    //TODO: have a 'continue' screen or something
                    DbHandler.ClearAllNonRefreshables();
                    var slot = DbHandler.ActiveSlot;
                    slot.NumLives = Player.DEFAULT_NUM_LIVES;
                    slot.NumContinuesUsed += 1;
                    DbHandler.ActiveSlot = slot;
                    SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                        ToSceneStr: "res://scenes/level_select/level_select.tscn",
                        effect: SceneTransitionEffect.FADE_BLACK,
                        numSeconds: 2f,
                        FadeOutAudio: true); 
                } else {
                    var slot = DbHandler.ActiveSlot;
                    slot.NumLives -= 1;
                    DbHandler.ActiveSlot = slot;
                    SceneTransitioner.RestartLevel(FromScene: this.GetTree().GetRoot().GetChild(0),
                                                   Level: this.ActiveLevel,
                                                   onLoadPlayerCalories: ((LevelNode2D)this.ActiveLevel.NodeInst).onLoadPlayerCalories,
                                                   SafetyCheckPoint: this.LastSafetyCheckPoint,
                                                   effect: SceneTransitionEffect.FADE_BLACK,
                                                   numSeconds: 2f,
                                                   FadeOutAudio: true);}
                break;
            default:
                throw new Exception("Invalid Player State");
        }

    }



    public override void UpdateState(float delta){}

    public void SetMostRecentSafetyCheckPoint(SafetyCheckPoint safetyCheckPoint){
        this.LastSafetyCheckPoint = safetyCheckPoint;
    }

    public void GoToMostRecentSafetyCheckPoint(){
        this.ATV.SetGlobalCenterOfTwoWheels(this.LastSafetyCheckPoint.GlobalPositionToResetTo);
    }

    public void EatFood(IFood food, bool playAnimation = true, bool playNomSound = true, bool updateTotalCalories = true){
        if(!food.isConsumed){
            if(!this.ATV.Bear.AnimationPlayer.CurrentAnimation.ToLower().Contains("eat")  && 
               !this.ATV.Bear.AnimationPlayer.CurrentAnimation.ToLower().Contains("attack") && 
               playAnimation){
                this.ATV.Bear.AnimationPlayer.AdvancedPlay("eat1");}
            if(playNomSound){
                this.ATV.Bear.PlayRandomNomSound();}
            this.UpdateHealth(food.Calories * HEALTH_TO_CALORIES_RATIO);
            if(updateTotalCalories){
                this.TotalCalories += food.Calories;}
            this.foodEaten.Add(food);
            this.lastFoodEaten = food;
            food.isConsumed = true;}}
    public void UpdateHealth(float signedHealthUnits){
        var tentativeHealth = this.CurrentHealth + signedHealthUnits;
        if(tentativeHealth >= MAX_HEALTH){
            this.CurrentHealth = MAX_HEALTH;}
        else {
            this.CurrentHealth = tentativeHealth;}}

    public void GetHitBy(object node){
        if(node is INPC && this.ActiveState.Equals(PlayerState.ALIVE)){
            this.ATV.Bear.TriggerHitSequence();
            //TODO: evaluate if this is needed or not
    }}

    public void PickupHoldable(IHoldable Holdable){
        Holdable.Player = this;
        Holdable.IsBeingHeld = true;
        this.ActiveHoldable = Holdable;
        this.AllHoldibles.Add(Holdable);
        this.WholeBodyKinBody.SetActiveState(WholeBodyState.INTERACTING_WITH_INTERACTABLE, 200);
        if(Holdable != ClawAttack){
            Holdable.PickupPreAction();
            SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this.ATV.Bear,
                new string[] {"res://media/samples/items/equip1.wav",});}
    }

    public void DropActiveHoldable(float delta){
        if(this.ActiveHoldable != ClawAttack){
            // Don't ever drop the ClawAttack, our default attack
            this.ActiveHoldable.PostDepletionAction(delta);
            this.AllHoldibles.Remove(this.ActiveHoldable);
            if(this.AllHoldibles.Count>=0){
                this.ActiveHoldable = this.AllHoldibles[this.AllHoldibles.Count-1];}}
    }


    public void ForceUnfreezePlayerInput(){
        this.ATV.FrontWheel.ResetActiveState(WheelState.IDLING);
        this.ATV.BackWheel.ResetActiveState(WheelState.IDLING);
    }

    public void MoveCameraTo(Node2D node, Vector2 offset, float numSecondsToTransition = 1f){
        this.ATV.Bear.CameraManager.MoveCameraToArbitraryNode(node, offset, numSecondsToTransition);
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
