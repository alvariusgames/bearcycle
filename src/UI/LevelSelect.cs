using Godot;
using System;
using System.Collections.Generic;

public enum LevelSelectState{WAITING_FOR_PLAYER_INPUT, ADVANCING_PLAYER, WAITING_FOR_MENU_INPUT}

public class LevelSelect : FSMNode2D<LevelSelectState>
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public VisiblePath2D LevelSelectPlayerPath2D;
    public LevelSelectPlayer LevelSelectPlayer;
    public LevelSelectView LevelSelectView;
    public LevelPortalChain LevelPortalChain;
    public LevelPortal ActiveLevelPortal { get { return this.LevelSelectPlayer.LevelPortalCurrentlyCollidingWith; }}
    public LevelSelectBanner LevelSelectBanner;
    public HoverableTouchScreenButton HomeButton;
    public HoverableTouchScreenButton AdvanceButton;
    public override LevelSelectState InitialState { get { return LevelSelectState.WAITING_FOR_PLAYER_INPUT;}}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        if(this.GetChild(0) is PlatformSpecificChildren){
            ((PlatformSpecificChildren)this.GetChild(0)).PopulateChildrenWithPlatformSpecificNodes(this);}
        foreach(Node child in this.GetChildren()){
            if(child is LevelSelectView){
                this.LevelSelectView = (LevelSelectView)child;
                foreach(var subChild in child.GetChildren()){
                    if(subChild is VisiblePath2D){
                        this.LevelSelectPlayerPath2D = (VisiblePath2D)subChild;
                        this.LevelSelectPlayerPath2D.Color = new Color(0.8f, 0.8f, 0.8f);
                        if(main.PlatformType.Equals(PlatformType.DESKTOP)){ 
                           //!DbHandler.Globals.PerformanceMode){ //TODO: uncomment me and implement performance mode
                            //Mobile can't handle the number of dotted lines, so make it a solid line
                            this.LevelSelectPlayerPath2D.DottedSpace = 3;
                            this.LevelSelectPlayerPath2D.DottedLength = 3;}
                        this.LevelSelectPlayerPath2D.Width = 6;
                        this.LevelSelectPlayer = (LevelSelectPlayer)(this.LevelSelectPlayerPath2D.GetChild(0).GetChild(0));}
                    if(subChild is LevelPortalChain){
                        this.LevelPortalChain = (LevelPortalChain)subChild;}
                }
            }
            if(child is LevelSelectBanner){
                this.LevelSelectBanner = (LevelSelectBanner)child;
                foreach(Node subchild in this.LevelSelectBanner.GetChildren()){
                    if(subchild is HoverableTouchScreenButton && subchild.Name.ToLower().Contains("home")){
                        this.HomeButton = (HoverableTouchScreenButton)subchild;}
                    if(subchild is HoverableTouchScreenButton && subchild.Name.ToLower().Contains("advance")){
                        this.AdvanceButton = (HoverableTouchScreenButton)subchild;}
                }
            }
        }
        SoundHandler.PlayStream<MyAudioStreamPlayer>(this, new string[] {"res://media/music/misc/trance.ogg"},
                                                     Loop: true);
        //DbHandler.SetHighestLevelUnlocked(7); //uncomment this line to h4x and unlock all levels
        this.UnlockLevelsUpTo(DbHandler.ActiveSlot.HighestLevelNumUnlocked);
        this.PlacePlayerOverLevel(DbHandler.ActiveSlot.currentLevelNumHoveringOver);
    }

    public override void ReactStateless(float delta){
      if(this.HomeButton.UserHasJustSelected()){
        SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
            new string[]{"res://media/samples/ui/decline_1.wav"});
         SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0),
                                      ToSceneStr: "res://scenes/title_screen/title_screen_press_start.tscn",
                                      effect: SceneTransitionEffect.FADE_BLACK,
                                      numSeconds: 1f);
      }
      this.LevelSelectBanner.PopulateWith(this.ActiveLevelPortal);
    }
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case LevelSelectState.WAITING_FOR_PLAYER_INPUT:
                if(Input.IsActionJustPressed("menu_accept")){
                    this.AdvanceButton.MimicTouch();}
                if(Input.IsActionJustReleased("menu_accept")){
                    this.AdvanceButton.MimicTouchRelease();}
                if(Input.IsActionJustPressed("menu_back")){
                    this.HomeButton.MimicTouch();}
                if(Input.IsActionJustReleased("menu_back")){
                    this.HomeButton.MimicTouchRelease();}
 
                break;
        }
    }
    public override void UpdateState(float delta){}

    public void UnlockLevelsUpTo(int levelNum){
        GD.Print("unlocking up to " + levelNum);
        this.LevelPortalChain.UnlockLevelPortalsUpTo(levelNum);
        for(int i=0; i<this.LevelPortalChain.LevelPortals.Count; i++){
            var levelPortal = this.LevelPortalChain.LevelPortals[i];
            if(i<levelNum){
                levelPortal.Visible = true;}
            else if(i==levelNum){
                this.LevelSelectPlayerPath2D.PercentOfCurveToDraw = levelPortal.GetLevel().UnitOffset;
                if(this.LevelSelectPlayerPath2D.PercentOfCurveToDraw == level7.ConstUnitOffset){
                    //Don't draw any lines to the bonus level
                    this.LevelSelectPlayerPath2D.PercentOfCurveToDraw = level6.ConstUnitOffset;
                }}
            else if(i>levelNum){
                levelPortal.Visible = false;}

        }
    }

    public void PlacePlayerOverLevel(int levelNum){
        var levelPortal = this.LevelPortalChain.LevelPortals[levelNum];
        var level = levelPortal.GetLevel();
        this.LevelSelectPlayer.PathFollow2D.UnitOffset = level.UnitOffset-0.001f;
        this.LevelSelectPlayer.MoveTo(levelPortal);
        if(LevelPortal.IsSpaceLevel(levelNum)){
            this.LevelSelectView.SetToSpaceView(transitionSec: 0f);
        } else {
            this.LevelSelectView.SetToEarthView(transitionSec: 0f);
        }
    }

}
