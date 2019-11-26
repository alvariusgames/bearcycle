using Godot;
using System;
using System.Collections.Generic;

public enum LevelSelectState{WAITING_FOR_PLAYER_INPUT, ADVANCING_PLAYER, WAITING_FOR_MENU_INPUT}

public class LevelSelect : FSMNode2D<LevelSelectState>
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public LevelSelectPlayer LevelSelectPlayer;
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
        foreach(var child in this.GetChildren()){
            if(child is LevelSelectPlayer){
                this.LevelSelectPlayer = (LevelSelectPlayer)child;}
            if(child is LevelPortalChain){
                this.LevelPortalChain = (LevelPortalChain)child;}
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
        SoundHandler.PlayStream<MyAudioStreamPlayer>(this, new string[] {"res://media/music/misc/trance.ogg"});
    }

    public override void ReactStateless(float delta){
      if(this.HomeButton.UserHasJustSelected()){
          SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0),
                                       ToSceneStr: "res://scenes/title_screen/title_screen_press_start.tscn",
                                       effect: SceneTransitionEffect.FADE_BLACK,
                                       numSeconds: 1f);
      }
      if(this.ActiveLevelPortal != LevelPortal.None){
          this.LevelSelectBanner.PopulateWith(this.ActiveLevelPortal);
      }
    }
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case LevelSelectState.WAITING_FOR_PLAYER_INPUT:
                this.HomeButton.SetGraphicToUnpressed();
                if(Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("ui_pause")){
                    this.AdvanceButton.MimicTouch();}
                if(Input.IsActionJustReleased("ui_accept") || Input.IsActionJustReleased("ui_pause")){
                    this.AdvanceButton.MimicTouchRelease();}
                break;
            case LevelSelectState.WAITING_FOR_MENU_INPUT:
                if(Input.IsActionJustReleased("ui_right")){
                    this.ResetActiveState(LevelSelectState.WAITING_FOR_PLAYER_INPUT);
                    this.HomeButton.SetGraphicToUnpressed();}
                if(Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("ui_pause")){
                    this.HomeButton.MimicTouch();}
                else{
                    this.HomeButton.SetGraphicToPressed();}
                if(Input.IsActionJustReleased("ui_accept") || Input.IsActionJustReleased("ui_pause")){
                    this.HomeButton.MimicTouchRelease();}
                break;
        }
    }
    public override void UpdateState(float delta){
        if(this.LevelPortalChain.LevelPortals[0] == this.LevelSelectPlayer.LevelPortalCurrentlyCollidingWith){
            //If at first level and left is pressed, hover over "home" button
            if(Input.IsActionJustReleased("ui_left")){
                this.ResetActiveState(LevelSelectState.WAITING_FOR_MENU_INPUT);
            }
        }
    }

}
