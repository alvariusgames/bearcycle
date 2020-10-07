using Godot;
using System;

public class Ranger : PursuingHuman, INPC, IFood, IVisibilityTrackable {
    [Export]
    public NodePath PepperSpraySprite {get; set;}
    public Sprite PepperSpraySpriteInst;
    [Export]
    public NodePath PepperSprayParticles {get; set;}
    public Particles2D PepperSprayParticlesInst;
    [Export]
    public Boolean PepperSprayParticlesLocal {get; set;} = false;

    [Export]
    public NodePath VisibilityNotifierPath {get; set;}
    private VisibilityNotifier2D visibilityNotifier2D;
    public VisibilityNotifier2D VisibilityNotifier2D { get { 
        if(this.visibilityNotifier2D is null){
            this.visibilityNotifier2D = this.GetNode<VisibilityNotifier2D>(this.VisibilityNotifierPath);}
        return this.visibilityNotifier2D;}}

    private Boolean wasPreviouslyVisible = true;
    public Boolean IsOnScreen(){
        var currVisible = this.VisibilityNotifier2D.IsOnScreen();
        var out_ = this.wasPreviouslyVisible || currVisible; //bias towards visible
        this.wasPreviouslyVisible = currVisible;
        return out_; }

    public override void _Ready(){
        base._Ready();
        this.PepperSpraySpriteInst = (Sprite)this.GetNode(this.PepperSpraySprite);
        this.PepperSprayParticlesInst = (Particles2D)this.GetNode(this.PepperSprayParticles);
        this.PepperSprayParticlesInst.LocalCoords = this.PepperSprayParticlesLocal;
    }

    public override void PursueIdle(float delta){
        this.NPCAttackWindow.MakeCollideable(false);
        this.NPCAttackWindow.Visible = false;
        base.PursueIdle(delta);
        this.PepperSpraySpriteInst.Visible = false;
        this.PepperSprayParticlesInst.Emitting = false;}

    public override void PursueMovingAwayPlayerGround(float delta){
        this.NPCAttackWindow.MakeCollideable(false);
        this.NPCAttackWindow.Visible = false;
        base.PursueMovingAwayPlayerGround(delta);
        this.PepperSpraySpriteInst.Visible = false;
        this.PepperSprayParticlesInst.Emitting = false;}

    public override void PursueMovingTowardPlayerGround(float delta){
        this.NPCAttackWindow.MakeCollideable(false);
        this.NPCAttackWindow.Visible = false;
        base.PursueMovingTowardPlayerGround(delta);
        this.PepperSpraySpriteInst.Visible = false;
        this.PepperSprayParticlesInst.Emitting = false;}

    public override String GetDisplayableName(){
        this.Name = "ranger";
        return this.getDisplayableName();
    }

    public override void PursueAttacking(float delta){
        this.forwardAccell *= 0.9f;
        this.PepperSpraySpriteInst.Visible = true;
        this.ActiveAnimationString = "attack_stationary_pepper_spray";
        if(this.prevAnimStr != this.currentAnimStr){
            // We are newly attacking -- do a workaroudn so attack windows werk correctly
            return;}
        var percentTimeAnimationPlaying = this.AnimationPlayer.CurrentAnimationPosition / 
                                          this.AnimationPlayer.CurrentAnimationLength;

        const float PERCENT_TIME_OF_ANIMATION_TO_EMIT = 0.3f;
        if(percentTimeAnimationPlaying < PERCENT_TIME_OF_ANIMATION_TO_EMIT){
            this.PepperSprayParticlesInst.Emitting = true;}
        else{
            this.PepperSprayParticlesInst.Emitting = false;}

        const float PERC_ATTACK_OFFSET = 0.2f;
        const float PERC_ATTACK_LENGTH = 0.3f;
        if(percentTimeAnimationPlaying > PERC_ATTACK_OFFSET &&
           percentTimeAnimationPlaying < PERC_ATTACK_OFFSET + PERC_ATTACK_LENGTH){
            this.NPCAttackWindow.Visible = true;
            this.NPCAttackWindow.MakeCollideable(true);}
        else{
            this.NPCAttackWindow.Visible = false;
            this.NPCAttackWindow.MakeCollideable(false);}

    }
}