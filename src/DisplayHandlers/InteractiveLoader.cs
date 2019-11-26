using Godot;
using System;

public class InteractiveLoader : Node {
    public SceneTransitionerArguments Args;
    private Node RootNode;
    private Viewport viewport;
    public TextureProgress ProgressBar;
    private ResourceInteractiveLoader loader;

    public override void _Ready(){
        GD.Print("initialized!");
        foreach(Node child in this.GetChildren()){
            if(child is ViewportContainer){
                this.viewport = (Viewport)child.GetChild(0);
                this.viewport.AddChild(this.Args.FromScene);
            } if(child is TextureProgress){
                this.ProgressBar = (TextureProgress)child;
            } 
        }
        if(this.Args.ToLevelStr != null){
            this.loader = ResourceLoader.LoadInteractive(this.Args.ToLevelStr);
                if(this.loader == null){
                //TODO: FIX ME WHEN BUG IS RESOLVED
                //https://github.com/godotengine/godot/issues/33809
                var res = ResourceLoader.Load(this.Args.ToLevelStr, noCache:true) as PackedScene;
                this.finishLoadingTransitionToScene(res.Instance());}
        }
        else if(this.Args.ToSceneStr != null){
            this.loader = ResourceLoader.LoadInteractive(this.Args.ToSceneStr);
            if(this.loader == null){
                //TODO: FIX ME WHEN BUG IS RESOLVED
                //https://github.com/godotengine/godot/issues/33809
                var res = ResourceLoader.Load(this.Args.ToSceneStr, noCache:true) as PackedScene;
                this.finishLoadingTransitionToScene(res.Instance());}

 }
    }

    public override void _Process(float delta){
        this.ProgressBar.MaxValue = loader.GetStageCount();
        Error err; var startingTimeMs = OS.GetTicksMsec();
        if((err = loader.Poll()) != Error.FileEof){
                var elapsedTimeS = (OS.GetTicksMsec() - startingTimeMs) / 1000f;
                this.ProgressBar.Value = (loader.GetStage());}
        else {
            var loadedScene = (this.loader.GetResource() as PackedScene).Instance();
            this.finishLoadingTransitionToScene(loadedScene);
        }
    }

    private void finishLoadingTransitionToScene(Node loadedScene){
        Node toScene;
        if(this.Args.ToLevelStr != null){
            var levelFrameScene = (LevelFrame)(GD.Load("res://scenes/frames/LevelFrame.tscn") as PackedScene).Instance();
            levelFrameScene._Ready();
            levelFrameScene.Viewport.AddChild(loadedScene);
            toScene = levelFrameScene;}
        else{
            toScene = loadedScene;
        }
        var RootNode = this.GetTree().Root;
        RootNode.RemoveChild(this);
        this.QueueFree();
        this.loader = null;
        this.viewport.RemoveChild(this.Args.FromScene);
        RootNode.AddChild(this.Args.FromScene);
        SceneTransitioner.TransitionAlreadyLoadedScenes(FromScene: this.Args.FromScene,
                                                        ToScene: toScene,
                                                        effect: this.Args.effect,
                                                        numSeconds: this.Args.numSeconds,
                                                        FadeOutAudio: this.Args.FadeOutAudio);
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
