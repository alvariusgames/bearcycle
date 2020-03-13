using Godot;
using System;

public class TutorialRotationThrowaway : KinematicBody2D{

    private Boolean hasBeenCompleted = false;
    private Boolean activated = false;
    private WholeBodyKinBody wholeBody;
    private float prevRotation = 0f;
    private float forwardRadiansTraversed = 0f;
    private float backwardRadiansTraversed = 0f;
    private const float RAD_TRAVERSED_TO_RELEASE = 6f;

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta){
        var collision = this.MoveAndCollide(new Vector2(0,0));
        if(collision != null &&
           collision.Collider is WholeBodyKinBody &&
           !this.hasBeenCompleted){
            this.wholeBody = (WholeBodyKinBody)collision.Collider;
            wholeBody.Player.ATV.tempStopAllMovement();
            this.activated = true;
            this.prevRotation = wholeBody.Player.ATV.GetDeFactorGlobalRotation();}
        if(activated && !this.hasBeenCompleted){
            var atv = wholeBody.Player.ATV;
            var currRot = atv.GetDeFactorGlobalRotation();
            var dRot = this.prevRotation - currRot;
            if(dRot < 0f && dRot > -2f){
                this.backwardRadiansTraversed -= dRot;}
            if(dRot > 0f && dRot < 2f){
                this.forwardRadiansTraversed += dRot;}
            this.prevRotation = currRot;
            if(this.backwardRadiansTraversed > RAD_TRAVERSED_TO_RELEASE && this.forwardRadiansTraversed > RAD_TRAVERSED_TO_RELEASE){
                atv.resumeMovement();
                this.hasBeenCompleted = true;}
      }
  }
//  {
//      
//  }
}
