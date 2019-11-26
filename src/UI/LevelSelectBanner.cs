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


    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child is Label && child.Name.ToLower().Contains("title")){
                this.TitleLabel = (Label)child;}
            if(child is HoverableTouchScreenButton && child.Name.ToLower().Contains("home")){
                this.HomeButton = (HoverableTouchScreenButton)child;}
            if(child is HoverableTouchScreenButton && child.Name.ToLower().Contains("advance")){
                this.AdvanceButton = (HoverableTouchScreenButton)child;}
            if(child.Name.ToLower().Contains("spacerock")){
                foreach(Node subchild in child.GetChildren()){
                    if(subchild is Label){
                        this.NumSpaceRocksLabel = (Label)subchild;
                    }
                }
            }
        }    
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public void PopulateWith(LevelPortal levelPortal){
      this.TitleLabel.Text = levelPortal.GetLevel().Title;
      var numSpaceRocks = (new Boolean[]{levelPortal.GetLevel().SpaceRock1Collected,
                                        levelPortal.GetLevel().SpaceRock2Collected,
                                        levelPortal.GetLevel().SpaceRock3Collected}).Count(x => x);
      this.NumSpaceRocksLabel.Text = "x " + numSpaceRocks.ToString();
  }
//  {
//      
//  }
}
