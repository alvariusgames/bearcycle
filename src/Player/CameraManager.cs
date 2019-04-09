using Godot;
using System;

public enum CameraManagerState {NOT_MOVING_FOLLOW_NODE, MOVING_FOLLOW_NODE_RIGHT, MOVING_FOLLOW_NODE_LEFT};

public class CameraManager : FSMNode2D<CameraManagerState>{
    public override CameraManagerState InitialState {
        get { return CameraManagerState.NOT_MOVING_FOLLOW_NODE;}}
    public Bear Bear;
    public Node2D NodeCameraFollows;
    public Camera2D Camera2D;
    private const float PERCENT_FROM_CENTER_TRAVEL_LIMIT = 0.7f;
    private const float TRAVEL_SPEED = 15f;
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

    public override void ReactToState(float delta){
        var followNodePos = this.NodeCameraFollows.GetPosition();
        switch(this.ActiveState){
            case CameraManagerState.NOT_MOVING_FOLLOW_NODE:
                break;
            case CameraManagerState.MOVING_FOLLOW_NODE_RIGHT:
                this.NodeCameraFollows.SetPosition(
                    new Vector2(followNodePos.x-TRAVEL_SPEED, followNodePos.y));
                break;
            case CameraManagerState.MOVING_FOLLOW_NODE_LEFT:
                this.NodeCameraFollows.SetPosition(
                    new Vector2(followNodePos.x+TRAVEL_SPEED, followNodePos.y));
                break;
            default:
                throw new Exception("Invalid State");
        }
    }

    public override void UpdateState(float delta){
        var followNodePos = this.NodeCameraFollows.GetPosition();
        var percentFromCenterTraveled = followNodePos.x / this.screenSize.x;
        if(this.Bear.ATV.IsInAir()){
             this.SetActiveState(CameraManagerState.NOT_MOVING_FOLLOW_NODE, 100);
        } else if(Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_right")){
            this.SetActiveState(CameraManagerState.NOT_MOVING_FOLLOW_NODE, 100);
        } else if(Input.IsActionPressed("ui_left")){
            if(percentFromCenterTraveled > -PERCENT_FROM_CENTER_TRAVEL_LIMIT){
                this.SetActiveState(CameraManagerState.MOVING_FOLLOW_NODE_RIGHT, 100);
            } else {
                this.SetActiveState(CameraManagerState.NOT_MOVING_FOLLOW_NODE, 100);}
        } else if(Input.IsActionPressed("ui_right")){
            if(percentFromCenterTraveled < PERCENT_FROM_CENTER_TRAVEL_LIMIT){
                this.SetActiveState(CameraManagerState.MOVING_FOLLOW_NODE_LEFT, 100);
            } else {
                this.SetActiveState(CameraManagerState.NOT_MOVING_FOLLOW_NODE, 100);}
        } else {
            this.SetActiveState(CameraManagerState.NOT_MOVING_FOLLOW_NODE, 100);
        }
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
