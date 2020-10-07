using Godot;
using System;
using System.Linq;


public class LevelPortal : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public LevelPortalChain LevelPortalChain;
    public TouchScreenButton TouchScreenButton;
    public ILevel level;
    private Sprite On;
    private Sprite Off;
    private Node2D ForwardArrow;
    private Boolean IsNextLevelUnlocked = false;
    private Node2D BackwardArrow;
    public String forwardInputString1 = "";
    public String forwardInputString2 = "";
    public String backwardInputString1 = "";
    public String backwardInputString2 = "";
    public TouchScreenButton ToForwardLevelPortalTouchScreenButton;
    public TouchScreenButton ToBackwardLevelPortalTouchScreenButton;
    public int BigArrowLevelNumToGoTo = -1;

    public ILevel GetLevel(){return this.level;}
    public float Offset = 0f; //The offset on the path2D the player is on

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        this.setLevel();
        this.IsNextLevelUnlocked = DbHandler.IsLevelUnlocked(this.GetLevel().LevelNum + 1);
        if(this.GetParent() is LevelPortalChain){
            this.LevelPortalChain = (LevelPortalChain)this.GetParent();}
        foreach(Node2D child in this.GetChildren()){
            if(child is TouchScreenButton){
                this.TouchScreenButton = (TouchScreenButton)child;}
            if(child is Sprite && child.Name.ToLower().Contains("on")){
                this.On = (Sprite)child;}
            if(child is Sprite && child.Name.ToLower().Contains("off")){
                this.Off = (Sprite)child;}
            if(child is Node2D && child.Name.ToLower().Contains("forward")){
                this.ForwardArrow = (Node2D)child;
                if(this.ForwardArrow is Sprite){
                    this.makeArrowHoverable((MakeMyParentHover)this.ForwardArrow.GetChild(0), false);
                    this.checkForButtonFromArrow(child, forward: true);}
               else {
                    foreach(Node2D subChild in this.ForwardArrow.GetChildren()){
                        this.makeArrowHoverable((MakeMyParentHover)subChild.GetChild(0), false);
                        this.checkForButtonFromArrow(subChild, forward: true);}}}
            if(child is Node2D && child.Name.ToLower().Contains("backward")){
                this.BackwardArrow = (Node2D)child;
                if(this.BackwardArrow is Sprite){
                    this.makeArrowHoverable((MakeMyParentHover)this.BackwardArrow.GetChild(0), true);
                    this.checkForButtonFromArrow(child, forward: false);}
                else {
                    foreach(Node2D subChild in this.BackwardArrow.GetChildren()){
                        this.makeArrowHoverable((MakeMyParentHover)subChild.GetChild(0), true);
                        this.checkForButtonFromArrow(subChild, forward: false);}}}}}

    private void makeArrowHoverable(MakeMyParentHover makeMyParentHoverNode, Boolean reverse){
          makeMyParentHoverNode.HoverVertical = false;
          makeMyParentHoverNode.HoverHorizontal = true;
          makeMyParentHoverNode.Reverse = reverse;
          makeMyParentHoverNode.RandomStartAmpl = false;}

    private void checkForButtonFromArrow(Node2D ArrowSprite, Boolean forward){
        foreach(Node2D child in ArrowSprite.GetChildren()){
            if(child is TouchScreenButton){
                if(forward){
                    this.ToForwardLevelPortalTouchScreenButton = (TouchScreenButton)child;}
                else{
                    this.ToBackwardLevelPortalTouchScreenButton = (TouchScreenButton)child;}}}}

    private void setLevel(){
        if(this.Name.Contains("0")){
            this.level = Level.AssembleLevelFrom(title: level0.ConstTitle,
                                                 zone: -1,
                                                 levelNum: 0,
                                                 unitOffset: level0.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level0z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(0));
            this.forwardInputString1 = "ui_right";}
        if(this.Name.Contains("1")){
            this.level = Level.AssembleLevelFrom(title: level1.ConstTitle,
                                                 zone: 1,
                                                 levelNum: 1,
                                                 unitOffset: level1.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level1z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(1));
            this.forwardInputString1 = "ui_right";
            this.backwardInputString1 = "ui_left";}
        if(this.Name.Contains("2")){
            this.level = Level.AssembleLevelFrom(title: level2.ConstTitle,
                                                 levelNum: 2,
                                                 zone: 1,
                                                 unitOffset: level2.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level2z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(2));
            this.forwardInputString1 = "ui_right";
            this.backwardInputString1 = "ui_left";} 
        if(this.Name.Contains("3")){
            this.level = Level.AssembleLevelFrom(title: level3.ConstTitle,
                                                 levelNum: 3,
                                                 zone: 1,
                                                 unitOffset: level3.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level3z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(3));
            this.forwardInputString1 = "ui_right";
            this.forwardInputString2 = "ui_up";
            this.backwardInputString1 = "ui_left";} 
        if(this.Name.Contains("4")){
            this.level = Level.AssembleLevelFrom(title: level4.ConstTitle,
                                                 levelNum: 4,
                                                 zone: 1,
                                                 unitOffset: level4.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level4z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(4));
            this.forwardInputString1 = "ui_left";
            this.backwardInputString1 = "ui_right";}
        if(this.Name.Contains("5")){
            this.level = Level.AssembleLevelFrom(title: level5.ConstTitle,
                                                 levelNum: 5,
                                                 zone: 1,
                                                 unitOffset: level5.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level5z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(5));
            this.forwardInputString1 = "ui_left";
            this.forwardInputString2 = "ui_down";
            this.backwardInputString1 = "ui_right";
            this.backwardInputString2 = "ui_up";} 
        if(this.Name.Contains("6")){
            this.level = Level.AssembleLevelFrom(title: level6.ConstTitle,
                                                 levelNum: 6,
                                                 zone: 1,
                                                 unitOffset: level6.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level6z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(6));
            this.forwardInputString1 = "ui_right";
            this.backwardInputString1 = "ui_left";
            this.backwardInputString2 = "ui_down";} 

        if(this.Name.Contains("7")){
            this.level = Level.AssembleLevelFrom(title: level7.ConstTitle,
                                                 levelNum: 7,
                                                 zone: 1,
                                                 unitOffset: level7.ConstUnitOffset,
                                                 nodePath: "res://scenes/levels/level7z1.tscn",
                                                 levelStatsRecord: DbHandler.GetLevelStatsRecordFor(7));
            this.backwardInputString1 = "ui_left";
            this.backwardInputString2 = "ui_up";} 
    }

    public override void _Process(float delta){
        this.checkForwardAndBackwardButtonPress();
        var prevPos = this.GlobalPosition;
        var col = this.MoveAndCollide(new Vector2(0,0));
        this.GlobalPosition = prevPos;
        if(col != null && col.Collider is LevelSelectPlayer){
            this.setButtonGraphicTo(true);
            this.setArrowsToVisible(true);}
        else{
            this.setButtonGraphicTo(false);
            this.setArrowsToVisible(false);
            if(this.TouchScreenButton.IsPressed()){
                SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                    new string[]{"res://media/samples/ui/click_1.wav"},
                    SkipIfAlreadyPlaying: true,
                    MaxNumberTimesPlayPerSecond: 1f);
                var levelSelectPlayer = this.LevelPortalChain.LevelSelect.LevelSelectPlayer;
                levelSelectPlayer.MoveTo(this);}}}

    private void checkForwardAndBackwardButtonPress(){
        if(this.ToForwardLevelPortalTouchScreenButton != null &&
           this.ToForwardLevelPortalTouchScreenButton.IsPressed()){
                SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                    new string[]{"res://media/samples/ui/click_1.wav"},
                    MaxNumberTimesPlayPerSecond: 1f);
                var levelPortalToMoveTo = this.LevelPortalChain.LevelPortals[this.GetLevel().LevelNum + 1];
                this.LevelPortalChain.LevelSelect.LevelSelectPlayer.MoveTo(levelPortalToMoveTo);}
        if(this.ToBackwardLevelPortalTouchScreenButton != null &&
           this.ToBackwardLevelPortalTouchScreenButton.IsPressed()){
                SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                    new string[]{"res://media/samples/ui/click_1.wav"},
                    MaxNumberTimesPlayPerSecond: 1f);
                var levelPortalToMoveTo = this.LevelPortalChain.LevelPortals[this.GetLevel().LevelNum - 1];
                this.LevelPortalChain.LevelSelect.LevelSelectPlayer.MoveTo(levelPortalToMoveTo);}}

    public void setButtonGraphicTo(Boolean state){
        if(state){
            this.On.Visible = true;
            this.Off.Visible = false;
        } else {
            this.On.Visible = false;
            this.Off.Visible = true;}}

    public void setArrowsToVisible(Boolean state){
        if(this.ForwardArrow != null){
            this.ForwardArrow.Visible = state && this.IsNextLevelUnlocked;}
        if(this.BackwardArrow != null){
            this.BackwardArrow.Visible = state;}
    }

    public static LevelPortal None { get { return null;}}

    public static Boolean IsSpaceLevel(int levelNum){
        if((new int[]{4,5,6}).Contains(levelNum)){
            return true;}
        else {
            return false;
        }
    }

}
