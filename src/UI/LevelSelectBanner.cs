using Godot;
using System;
using System.Linq;

public class LevelSelectBanner : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public HoverableTouchScreenButton HomeButton;
    public HoverableTouchScreenButton AdvanceButton;
    public Label TitleLabel;
    public Label NumSpaceRocksLabel;
    public Node2D SpaceRock;

    public AnimatedSprite SpaceRock1Animated;
    public Sprite SpaceRock1Outline;
    public AnimatedSprite SpaceRock2Animated;
    public Sprite SpaceRock2Outline;
    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child is Label && child.Name.ToLower().Contains("title")){
                this.TitleLabel = (Label)child;}
            if(child is HoverableTouchScreenButton && child.Name.ToLower().Contains("home")){
                this.HomeButton = (HoverableTouchScreenButton)child;}
            if(child is HoverableTouchScreenButton && child.Name.ToLower().Contains("advance")){
                this.AdvanceButton = (HoverableTouchScreenButton)child;}
            if(child.Name.ToLower().Contains("spacerock")){
                this.SpaceRock = (Node2D)child;
                foreach(Node subchild in child.GetChildren()){
                   if(subchild is AnimatedSprite){
                       if(subchild.Name.Contains("2")){
                           this.SpaceRock2Animated = (AnimatedSprite)subchild;}
                       else{
                           this.SpaceRock1Animated = (AnimatedSprite)subchild;}
                   }
                   if(subchild is Sprite){
                       if(subchild.Name.Contains("2")){
                           this.SpaceRock2Outline = (Sprite)subchild;}
                       else{
                           this.SpaceRock1Outline = (Sprite)subchild;}
                   } 
                }
            }
        }    
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public void PopulateWith(LevelPortal levelPortal){
        if(levelPortal == LevelPortal.None){
            this.TitleLabel.Text = "";
            this.SpaceRock.Visible = false;
            this.AdvanceButton.SelfModulate = new Color(1f,1f,1f,0.25f);}
        else{
            this.TitleLabel.Text = this.Tr(levelPortal.GetLevel().Title);
            this.SpaceRock.Visible = true;
            this.AdvanceButton.SelfModulate = new Color(1f,1f,1f,1f);
            /*
            if(levelPortal.GetLevel().SpaceRock1Collected){
                this.SpaceRock1Outline.Visible = false;
                this.SpaceRock1Animated.Visible = true;
            } else {
                this.SpaceRock1Outline.Visible = true;
                this.SpaceRock1Animated.Visible = false;}
            
            if(levelPortal.GetLevel().SpaceRock2Collected){
                //this.SpaceRock2Outline.Visible = false;
                //this.SpaceRock2Animated.Visible = true;
            } else {
                this.SpaceRock2Outline.Visible = true;
                this.SpaceRock2Animated.Visible = false;}
            */
           //exceptions to the rule
            if(levelPortal.GetLevel().Title == level0.ConstTitle){
                this.SpaceRock.Visible = false;}}}

    public override void _Process(float delta){
        base._Process(delta);
        this.modulateSpaceRockProcess(this.SpaceRock1Animated, delta);
        this.modulateSpaceRockProcess(this.SpaceRock2Animated, delta);
    }

    private void modulateSpaceRockProcess(Node2D node, float delta){
        return;
        var mod = node.Modulate;
        mod.s = 0.5f;
        mod.h += delta;
        if(mod.h > 1){
            mod.h = 0;}
        node.Modulate = mod;
    }

}