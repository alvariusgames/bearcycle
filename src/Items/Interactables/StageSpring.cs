using Godot;
using System;

public enum StageSpringState { READY_TO_BE_HIT, NOT_READY_TO_BE_HIT, TRIGGER_HIT, HIT_STAGE_1, HIT_STAGE_2, }
public class StageSpring : FSMKinematicBody2D<StageSpringState>, IInteractable{

    public override StageSpringState InitialState{ get { return StageSpringState.READY_TO_BE_HIT;}set{}}

    public int InteractPriority { get { return 500;}}
    [Export]
    public int VelocityToApply {get; set;} = 2000;
    private Sprite WoundUp;
    private Sprite Extended;
    private CollisionShape2D CollisionShape2D;
    private float timer = 0f;
    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child is Sprite && child.Name.ToLower().Contains("wound")){
                this.WoundUp = (Sprite)child;
                this.WoundUp.Visible = true;}
            if(child is Sprite && child.Name.ToLower().Contains("extend")){
                this.Extended = (Sprite)child;
                this.Extended.Visible = false;}
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;}}}

    public override void ReactStateless(float delta){}
    public override void UpdateState(float delta){}

    public void InteractWith(Player player, float delta){
        this.ResetActiveState(StageSpringState.TRIGGER_HIT);
        SoundHandler.PlaySample<MyAudioStreamPlayer2D>(player.ATV.Bear, Spring.BOING_SAMPLE);
        var springNormalRadians = this.Rotation + (1.5f * Math.PI);
        var springNormalVec2 = new Vector2((float)Math.Cos(springNormalRadians), (float)Math.Sin(springNormalRadians));

        //Find the forward velocity of ATV, but cancel out all vertical velocity relative to spring
        var vel = player.ATV.GetVelocityOfTwoWheels();
        var theta = vel.Normalized().AngleTo(springNormalVec2.Normalized());
        var forwardComponentVel = (Math.Abs(theta) / theta ) * (float)Math.Sin(theta) * vel;
        player.ATV.SetVelocityOfTwoWheels((springNormalVec2 * this.VelocityToApply) + forwardComponentVel);

    }

    public void TriggerHit(){
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case StageSpringState.NOT_READY_TO_BE_HIT:
                this.CollisionShape2D.Disabled = true;
                break;
            case StageSpringState.READY_TO_BE_HIT:
                this.CollisionShape2D.Disabled = false;
                this.Extended.Visible = false;
                this.WoundUp.Visible = true;
                break;
            case StageSpringState.TRIGGER_HIT:
                this.ResetActiveState(StageSpringState.NOT_READY_TO_BE_HIT);
                this.ResetActiveStateAfter(StageSpringState.HIT_STAGE_1, 0.001f);
                this.ResetActiveStateAfter(StageSpringState.HIT_STAGE_2, 0.2f);
                this.ResetActiveStateAfter(StageSpringState.READY_TO_BE_HIT, 0.5f);
                break;
            case StageSpringState.HIT_STAGE_1:
                this.Extended.Visible = true;
                this.WoundUp.Visible = false;
                break;
            case StageSpringState.HIT_STAGE_2:
                this.Extended.Visible = false;
                this.WoundUp.Visible = true;
                break;
        }
    }

}
