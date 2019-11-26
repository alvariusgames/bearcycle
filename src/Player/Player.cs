using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState {NORMAL, END_ANIMATION_CHANGE_TIMER}
public class Player : FSMNode2D<PlayerState>{
    public ILevel ActiveLevel;
    public override PlayerState InitialState {get { return PlayerState.NORMAL;}}
    public AttackWindow AttackWindow;

    public ClawAttack ClawAttack;
    public List<IHoldable> AllHoldibles = new List<IHoldable>(); //TODO: Make me a LIFO Stack
    public IHoldable ActiveHoldable;

    public ATV ATV;
    private const float MAX_HEALTH = 100;
    private const float HEALTH_TO_CALORIES_RATIO = 1f/100f;
    public const float DEFAULT_HIT_UNIT = -6f;
    public const int DEFAULT_NUM_LIVES = 5;
    public float CurrentHealth {get; private set;} = MAX_HEALTH;
    private const float HEALTH_DANGER_PERC = 0.25f;
    public Boolean IsInHealthDanger{ get { return this.CurrentHealth <= MAX_HEALTH * HEALTH_DANGER_PERC;}}
    public float TotalCalories = 0;
    private List<IFood> foodEaten = new List<IFood>();
    public IFood lastFoodEaten;
    private SafetyCheckPoint LastSafetyCheckPoint;
    public WholeBodyKinBody WholeBodyKinBody;
    private string animationToPlayAfterTimer;

    public override void _Ready(){
       this.ResetActiveState(this.InitialState);
       this.setInitialSafetyCheckPoint();
       foreach(var child in this.GetChildren()){
            if(child is ATV){
                this.ATV = (ATV)child;}
            else if(child is AttackWindow){
                this.AttackWindow = (AttackWindow)child;}
            else if(child is WholeBodyKinBody){
                this.WholeBodyKinBody = (WholeBodyKinBody)child;}
            else if(child is ClawAttack){
                this.ClawAttack = (ClawAttack)child;
                this.PickupHoldable(this.ClawAttack);
            }}}

    private void setInitialSafetyCheckPoint(){
       this.LastSafetyCheckPoint = new SafetyCheckPoint();
       this.LastSafetyCheckPoint.GlobalPositionToResetTo = this.GetGlobalPosition();
       this.LastSafetyCheckPoint.BeenActivated = true;
    }

    public override void ReactStateless(float delta){
        const float HEALTH_LOSS_DIVISOR = 1.5f;
        this.CurrentHealth -= delta / HEALTH_LOSS_DIVISOR;
        if(this.ActiveHoldable != null && this.ActiveHoldable.IsDepleted){
            this.DropActiveHoldable(delta);
        }
        if(this.CurrentHealth <= 5f){
            //TODO: MAKE ME MORE ELEGANT AND SHOW THE BEAR DIE ANIMATION
            //TODO: FIND OUT WHY BAR IS EMPTY at 5 HEALTH AND NOT 0
            SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                ToSceneStr: "res://scenes/level_select/level_select.tscn",
                effect: SceneTransitionEffect.FADE_BLACK,
                numSeconds: 2f,
                FadeOutAudio: true);
 
        }
    }
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case PlayerState.NORMAL:
                break;
            case PlayerState.END_ANIMATION_CHANGE_TIMER:
                this.playBearAnimation(this.animationToPlayAfterTimer);
                this.ResetActiveState(PlayerState.NORMAL);
                break;
            default:
                throw new Exception("Invalid Player State");
        }

    }

    public void stopAllBearAnimation(){
        this.ATV.Bear.CutOutAnimationPlayer.Stop();
    }

    public string currentBearAnimation{ get {
        return this.ATV.Bear.CutOutAnimationPlayer.CurrentAnimation;}}

    public void playBearAnimation(params string[] animationNames){
        Random random = new Random();
        var animationName = animationNames[random.Next(0, 
                                           animationNames.Length)];
        this.ATV.Bear.CutOutAnimationPlayer.Play(animationName);
    }

    public void playBearAnimationAfter(string animationName, float numSeconds){
        this.animationToPlayAfterTimer = animationName;
        this.ResetActiveStateAfter(PlayerState.END_ANIMATION_CHANGE_TIMER, numSeconds);}



    public override void UpdateState(float delta){}

    public void SetMostRecentSafetyCheckPoint(SafetyCheckPoint safetyCheckPoint){
        this.LastSafetyCheckPoint = safetyCheckPoint;
    }

    public void GoToMostRecentSafetyCheckPoint(){
        this.ATV.SetGlobalCenterOfTwoWheels(this.LastSafetyCheckPoint.GlobalPositionToResetTo);
    }

    public void EatFood(IFood food, bool playAnimation = true, bool updateTotalCalories = true){
        if(!food.isConsumed){
            if(!this.currentBearAnimation.ToLower().Contains("eat")  && 
               !this.currentBearAnimation.ToLower().Contains("attack") && 
               playAnimation){
                this.playBearAnimation("eat1");
                this.playBearAnimationAfter("idleBounce1", 1f);}
            this.ATV.Bear.PlayRandomNomSound();
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
        if(node is INPC){
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

    public void MoveCameraTo(Node2D node, float numSecondsToTransition=1f){
        this.ATV.Bear.CameraManager.MoveCameraToArbitraryNode(node, numSecondsToTransition);
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
