using Godot;
using System;

public class ReloadLocalesWorkaround : Node2D{
    private Boolean transitionBackToMainMenu = false;
    private float timeToWaitHereSec = 6;
    private float counter = 0f;
    private string[] randomLabelsToTranslate = new string[]{"DEER", "BIRD", "BLUEBERRIES", "UI_PRESS_START", "UI_PLAY", "UI_CONTROLS", "UI_CHNG_FORAGE"};
    private Label throwawayLabel;
    Viewport Viewport;
    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child is Label){
                this.throwawayLabel = (Label)child;
            }
            if(child is ViewportContainer){
                this.Viewport = (Viewport)child.GetChild(0);
            }
        }
    }

    public override void _Process(float delta){
        //Needed to boot into a level stay there to get remap resources to load properly (fonts etc)
        //This is a horrible hack of loading the tutorial level, which for some reason works (cache/performance bug?)
        //TODO: find out if this no longer is needed in future bug fixes to Godot
        if(this.counter == 0){
            try{
                var node = (ResourceLoader.Load("res://scenes/levels/level0z1.tscn") as PackedScene).Instance();
                this.Viewport.AddChild(node);}
            catch{}
        }
        this.counter += delta;

        var rnd = new Random();
        var index = rnd.Next(this.randomLabelsToTranslate.Length);
        this.throwawayLabel.Text = this.Tr(this.randomLabelsToTranslate[index]);
        if(this.counter > this.timeToWaitHereSec){
            SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                                         ToSceneStr: "res://scenes/title_screen/title_screen_press_start.tscn",
                                         effect: SceneTransitionEffect.FADE_BLACK,
                                         numSeconds: 1f,
                                         FadeOutAudio: true);
        }

    }

}
