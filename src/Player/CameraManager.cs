using Godot;
using System;

public enum CameraManagerState {MOVING_NODE_CAMERA_FOLLOWS, MOVE_TO_ARBITRARY_NODE,
                                STAY_STATIONARY};

public class CameraManager : FSMNode2D<CameraManagerState>{
    public override CameraManagerState InitialState {
        get { return CameraManagerState.MOVING_NODE_CAMERA_FOLLOWS;}set{}}
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

    private readonly Vector2 MOBILE_ZOOM = new Vector2(0.85f, 0.85f);
    private readonly Vector2 DESKTOP_ZOOM = new Vector2(1f, 1f);
    private void SetPlatformSpecificVars(){
        if(main.PlatformType == PlatformType.MOBILE){
            this.SetCameraZoom(MOBILE_ZOOM);}
        if(main.PlatformType == PlatformType.DESKTOP){
            this.SetCameraZoom(DESKTOP_ZOOM);
        }
    }

    public void SetCameraZoom(Vector2 zoom){
        if(this.Camera2D != null){
            this.Camera2D.Zoom = zoom;}
    }

    private bool runOnlyOnStartup = true;
    public override void ReactStateless(float delta){
        if(runOnlyOnStartup){
            this.SetPlatformSpecificVars();
            this.runOnlyOnStartup = false;}
        this.SetGlobalRotation(0);
        this.secondsForwardBackwardBookkeeping(delta);

    }

    private void secondsForwardBackwardBookkeeping(float delta){
        if(this.Bear.ATV.IsInAirNormalized()){
            //don't track ground seconds when in air
            return;}

        var isOnATV = this.Bear.ActiveState.Equals(BearState.ON_ATV);
        if(Input.IsActionPressed("ui_right") && isOnATV){
            this.aliveGroundSecForward += delta; }
        else {
            this.aliveGroundSecForward -= delta;
            this.aliveGroundSecForward = Math.Max(this.aliveGroundSecForward, 0f);
            this.aliveGroundSecForward = Math.Min(this.aliveGroundSecForward, MAX_NUM_SECONDS_TO_TRACK);}

        if(Input.IsActionPressed("ui_left") && isOnATV){
            this.aliveGroundSecBackward += delta;}
        else{
            this.aliveGroundSecBackward -= delta;
            this.aliveGroundSecBackward = Math.Max(this.aliveGroundSecBackward, 0f);
            this.aliveGroundSecBackward = Math.Min(this.aliveGroundSecBackward, MAX_NUM_SECONDS_TO_TRACK);}

    }

    private Node2D MoveCameraToNode;
    private Vector2 Offset;
    private float moveVelocity;
    public void MoveCameraToArbitraryNode(Node2D node, Vector2 offset, float numSecondsToTransition=1f){
        this.MoveCameraToNode = node;
        this.Offset = offset;
        this.moveVelocity = node.GlobalPosition.DistanceTo(this.NodeCameraFollows.GlobalPosition + offset) / numSecondsToTransition;
        var prevGlobalPosition = this.NodeCameraFollows.GlobalPosition;
        this.NodeCameraFollows.GetParent().RemoveChild(this.NodeCameraFollows);
        this.MoveCameraToNode.AddChild(this.NodeCameraFollows);
        this.NodeCameraFollows.GlobalPosition = prevGlobalPosition;

        this.ResetActiveState(CameraManagerState.MOVE_TO_ARBITRARY_NODE);
        this.ResetActiveStateAfter(CameraManagerState.STAY_STATIONARY, numSecondsToTransition);
    }

    public void ResetToDefaultFollowPlayerBehavior(){
        this.MoveCameraToArbitraryNode(this, new Vector2(0,0), 1f);
        this.prevXPosToApply = 0;
        this.prevYPosToApply = 0 ;
        this.ResetActiveStateAfter(CameraManagerState.MOVING_NODE_CAMERA_FOLLOWS, 1f);
    }

    private const float HORIZONTAL_VELOCITY_MULTIPLIER = 1f;
    private const float HORIZONTAL_DRAG_EFFECT = 0.97f;
    private const float VERTICAL_VELOCITY_MULTIPLIER = 0.3f;
    private const float VERTICAL_DRAG_EFFECT = 0.99f;
    private const float HORIZONTAL_OFFSET = 0f;
    private const float DESKTOP_VERTICAL_OFFSET = -100f;
    private const float MOBILE_VERTICAL_OFFSET = 0f;
    private float VerticalOffset { get {
        if(main.PlatformType.Equals(PlatformType.DESKTOP)){
            return DESKTOP_VERTICAL_OFFSET;}
        else if(main.PlatformType.Equals(PlatformType.MOBILE)){
            return MOBILE_VERTICAL_OFFSET;}
        else{
            return DESKTOP_VERTICAL_OFFSET;}}}
    private const float DESKTOP_MAX_POSITION_X = 700f;
    private const float DESKTOP_MAX_POSITION_Y = 700f;
    private const float MOBILE_MAX_POSITION_X = 500f;
    private const float MOBILE_MAX_POSITION_Y = 500f;

    private Vector2 MaxPosition{ get {
        if(main.PlatformType.Equals(PlatformType.DESKTOP)){
            return new Vector2(DESKTOP_MAX_POSITION_X, DESKTOP_MAX_POSITION_Y);}
        else if(main.PlatformType.Equals(PlatformType.MOBILE)){
            return new Vector2(MOBILE_MAX_POSITION_X, MOBILE_MAX_POSITION_Y);}
        else{
            return new Vector2(DESKTOP_MAX_POSITION_X, DESKTOP_MAX_POSITION_Y);}}}

    private float prevXPosToApply = 0f;
    private float prevYPosToApply = 0f;

    private float aliveGroundSecForward = 0f;
    private float aliveGroundSecBackward = 0f;
    private const float MAX_NUM_SECONDS_TO_TRACK = 3f;
    private const float UIACTION_VELOCITY_MULT = 400f;
    private Vector2 getUiActionVelocity(){
        var aliveGroundSecVelocity = new Vector2(this.aliveGroundSecForward - this.aliveGroundSecBackward, 0f) * 
                                                 UIACTION_VELOCITY_MULT;
        var playerVelocity = this.Bear.ATV.GetRecentAverageVelocityOfTwoWheels();
        var playerVelocityComponent = new Vector2(0.5f * playerVelocity.x,
                                                  0.25f * playerVelocity.y);
        return 0.5f * aliveGroundSecVelocity + 0.5f * playerVelocityComponent;
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case CameraManagerState.MOVING_NODE_CAMERA_FOLLOWS:
                var uiActionVelocity = this.getUiActionVelocity();
                var xPosToApply = uiActionVelocity.x * HORIZONTAL_VELOCITY_MULTIPLIER + HORIZONTAL_OFFSET;
                xPosToApply = ((1 - HORIZONTAL_DRAG_EFFECT) * xPosToApply) + (HORIZONTAL_DRAG_EFFECT * this.prevXPosToApply);
                if(Math.Abs(xPosToApply) > MaxPosition.x){
                    xPosToApply = (Math.Abs(xPosToApply) / xPosToApply) * MaxPosition.x;}
                this.prevXPosToApply = xPosToApply;

                var yPosToApply = uiActionVelocity.y * VERTICAL_VELOCITY_MULTIPLIER + VerticalOffset;
                yPosToApply = ((1 - HORIZONTAL_DRAG_EFFECT) * yPosToApply) + (HORIZONTAL_DRAG_EFFECT * this.prevYPosToApply);
                if(Math.Abs(yPosToApply) > MaxPosition.y){
                    yPosToApply = (Math.Abs(yPosToApply) / yPosToApply) * MaxPosition.y;}
                this.prevYPosToApply = yPosToApply;

                this.NodeCameraFollows.SetPosition(new Vector2(xPosToApply,
                                                               yPosToApply));            
               break;
            case CameraManagerState.MOVE_TO_ARBITRARY_NODE:
                var directionVec = (this.MoveCameraToNode.GlobalPosition + this.Offset - this.NodeCameraFollows.GlobalPosition).Normalized();
                var step = directionVec * this.moveVelocity * delta;
                this.NodeCameraFollows.GlobalPosition += step;
                break;
            case CameraManagerState.STAY_STATIONARY:
                break;
            default:
                throw new Exception("Invalid CameraManagerState");}}

    public override void UpdateState(float delta){}}
