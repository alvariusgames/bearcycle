using Godot;
using System;

public enum CameraManagerState {MOVING_NODE_CAMERA_FOLLOWS};

public class CameraManager : FSMNode2D<CameraManagerState>{
    public override CameraManagerState InitialState {
        get { return CameraManagerState.MOVING_NODE_CAMERA_FOLLOWS;}}
    public Bear Bear;
    public Node2D NodeCameraFollows;
    public Camera2D Camera2D;
    private Vector2 screenSize;

    public override void _Ready()
    {
        this.Bear = (Bear)this.GetParent();
        foreach(var child in this.GetChildren()){
            if(child is Node2D && ((Node2D)child).Name.Equals("NodeCameraFollows")){
                this.NodeCameraFollows = (Node2D)child;
                foreach(var child2 in this.NodeCameraFollows.GetChildren()){
                    if(child2 is Camera2D){
                        this.Camera2D = (Camera2D)child2;
                        this.screenSize = this.Camera2D.GetViewportRect().Size;
                    }
                }
            }
        }        
    }

    public override void ReactStateless(float delta){}

    private const float HORIZONTAL_MULTIPLIER_EFFECT = 1.20f;
    private const float VERTICAL_MULTIPLIER_EFFECT = 0.25f;
    private const float HORIZONTAL_OFFSET = 0f;
    private const float VERTICAL_OFFSET = -50f;
    private const float MAX_POSITION_X = 700f;
    private const float MAX_POSITION_Y = 700f;

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case CameraManagerState.MOVING_NODE_CAMERA_FOLLOWS:
                var vel = this.Bear.ATV.GetRecentAverageVelocityOfTwoWheels();
                var xPosToApply = vel.x * HORIZONTAL_MULTIPLIER_EFFECT + HORIZONTAL_OFFSET;
                if(Math.Abs(xPosToApply) > MAX_POSITION_X){
                    xPosToApply = (Math.Abs(xPosToApply) / xPosToApply) * MAX_POSITION_X;
                }
                var yPosToApply = vel.y * VERTICAL_MULTIPLIER_EFFECT + VERTICAL_OFFSET;
                if(Math.Abs(yPosToApply) > MAX_POSITION_Y){
                    yPosToApply = (Math.Abs(yPosToApply) / yPosToApply) * MAX_POSITION_Y;
                }
                this.NodeCameraFollows.SetPosition(new Vector2(xPosToApply,
                                                               yPosToApply));
                break;
            default:
                throw new Exception("Invalid CameraManagerState");}}

    public override void UpdateState(float delta){}}
