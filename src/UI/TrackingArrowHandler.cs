using Godot;
using System;
using System.Collections.Generic;

public class TrackingArrowHandler : Node2D{

    public LevelFrame LevelFrame;
    public ILevel Level { get { return this.LevelFrame.Level;}}

    public Player Player { get { return this.LevelFrame.Player;}}

    [Export]
    public NodePath TrackerPointerTemplatePath {get; set;}
    private Sprite trackerPointerTemplate;

    private Dictionary<ulong, Sprite> activeTrackerPointerSprites = new Dictionary<ulong, Sprite>();
    public Sprite TrackerPointerTemplate { get {
        if((this.trackerPointerTemplate is null ) && !(this.TrackerPointerTemplatePath is null)){
            this.trackerPointerTemplate = this.GetNode<Sprite>(this.TrackerPointerTemplatePath);}
        return this.trackerPointerTemplate;}}


    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public override void _Ready(){
        this.LevelFrame = (LevelFrame)this.GetParent();
    }

    public override void _Process(float delta){
        foreach(var trackable in this.Level.Trackables){
            if(trackable.ShouldTrackNodesNow){
                this._TrackingProcess(trackable, delta);}}}

    private void _TrackingProcess(ITrackable trackable, float delta){
        foreach(Node2D node in trackable.NodesToTrack){
            var id = node.GetInstanceId();
            Sprite trackerSprite;
            if(node is IVisibilityTrackable && !((IVisibilityTrackable)node).IsOnScreen()){
                if(this.activeTrackerPointerSprites.ContainsKey(id)){
                    trackerSprite = this.activeTrackerPointerSprites[id];
                } else {
                    trackerSprite = (Sprite)this.TrackerPointerTemplate.Duplicate();
                    this.AddChild(trackerSprite);
                    this.activeTrackerPointerSprites[id] = trackerSprite;}
                const float ARBITRARY_NODE_OFFSET = 0f;
                var modifiedNGlobalPos = new Vector2(node.GlobalPosition.x, 
                    node.GlobalPosition.y - ARBITRARY_NODE_OFFSET);
                var towardsAngle = modifiedNGlobalPos.AngleToPoint(
                    this.Player.ATV.GetGlobalCenterOfATV());
                this.placeAndFaceNodeAtBorder(trackerSprite, towardsAngle);
            } else if(this.activeTrackerPointerSprites.ContainsKey(id)){
                //A previously visible entity is no longer visible -- remove it
                var sprite = this.activeTrackerPointerSprites[id];
                this.activeTrackerPointerSprites.Remove(id);
                sprite.GetParent().RemoveChild(sprite);}
        }}

    private void placeAndFaceNodeAtBorder(Node2D node, float towardsAngle){
        var towardsVec = new Vector2(1,0).Rotated(towardsAngle);

        node.Rotation = towardsAngle;
        const float BORDER_BUFFER = 100f;
        var vpSize = this.LevelFrame.Viewport.Size;

        Vector2 GetVecInBorder(float x, float y){
            if(x < BORDER_BUFFER){
                x = BORDER_BUFFER;}
            if(x > vpSize.x - BORDER_BUFFER){
                x = vpSize.x - BORDER_BUFFER;}
            if(y < BORDER_BUFFER){
                y = BORDER_BUFFER;}
            if(y > vpSize.y - BORDER_BUFFER){
                y = vpSize.y - BORDER_BUFFER;}
            return new Vector2(x,y);}

        var vpHalfY = vpSize.y / 2f;
        var vpHalfX = vpSize.x / 2f;
        const float SQRT_2_OVER_2 = 0.707106781f;
        if(Math.Abs(towardsVec.x) > Math.Abs(towardsVec.y)){
            if(towardsVec.x > 0){
                //Right
                node.Position = GetVecInBorder(vpSize.x - BORDER_BUFFER,
                                               (vpHalfY + vpHalfY * (towardsVec.y / SQRT_2_OVER_2)));
            }
            else {
                //Left
                node.Position = GetVecInBorder(BORDER_BUFFER,
                                               (vpHalfY + vpHalfY * (towardsVec.y / SQRT_2_OVER_2)));

            }}
        else {
            if(towardsVec.y > 0){
                //Down
                node.Position = GetVecInBorder((vpHalfX + vpHalfX * (towardsVec.x / SQRT_2_OVER_2)),
                                               vpSize.y - BORDER_BUFFER);

            }
            else{
                //Up
                node.Position = GetVecInBorder((vpHalfX + vpHalfX * (towardsVec.x / SQRT_2_OVER_2)),
                                               BORDER_BUFFER);

            }}} 

}