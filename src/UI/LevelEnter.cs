using Godot;
using System;

public class LevelEnter : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public Sprite LeftBar;
    public Sprite Background;
    public Label LevelLabel;
    public Label ZoneLabel;
    private float timeAlive = 0f;
    const float TIME_TO_LIVE_SEC = 2f;
    const float TIME_TO_MAKE_INVISIBLE = 3f;

    public override void _Ready(){
        this.Visible = true;
        this.timeAlive = 0f;
        foreach(Node2D child in this.GetChildren()){
            if(child is Sprite && child.Name.ToLower().Contains("background")){
                this.Background = (Sprite)child;
                foreach(Node subChild in child.GetChildren()){
                    if(subChild is Label && subChild.Name.ToLower().Contains("level")){
                        this.LevelLabel = (Label)subChild;}
                    if(subChild is Label && subChild.Name.ToLower().Contains("zone")){
                        this.ZoneLabel = (Label)subChild;}}}
            if(child is Sprite && child.Name.ToLower().Contains("leftbar")){
                this.LeftBar = (Sprite)child;}}}

  public override void _Process(float delta)  {
    this.timeAlive += delta;
    var speed = 600f * delta * timeAlive;
    if(this.timeAlive >= TIME_TO_LIVE_SEC){
        this.LeftBar.Position -= new Vector2(speed,0);
        this.Background.Position -= new Vector2(0, speed*1.2f);}
    if(this.timeAlive >= TIME_TO_MAKE_INVISIBLE){
        this.Visible = false;
    }
    
  }
}
