using Godot;
using System;

public enum SceneTransitionEffect { DIRECT_CUT, FADE_INTO, FADE_BLACK }

public class SceneTransitionerArguments{
    public Node FromScene;
    public String ToSceneStr;
    public String ToLevelStr;
    public SceneTransitionEffect effect;
    public float numSeconds;
    public Boolean FadeOutAudio;}

public static class SceneTransitioner{

    public static void TransitionToLevel(Node FromScene, String ToLevelStr,
                                         SceneTransitionEffect effect = SceneTransitionEffect.DIRECT_CUT,
                                         float numSeconds = 0f,
                                         Boolean FadeOutAudio = false){
        var interactiveLoaderScene = (InteractiveLoader)(GD.Load("res://scenes/frames/InteractiveLoader.tscn") as PackedScene).Instance();
        interactiveLoaderScene.Args = new SceneTransitionerArguments{
            FromScene = FromScene,
            ToLevelStr = ToLevelStr,
            effect = effect,
            numSeconds = numSeconds,
            FadeOutAudio = FadeOutAudio
        };
        var RootNode = FromScene.GetTree().Root;
        RootNode.RemoveChild(FromScene);
        RootNode.AddChild(interactiveLoaderScene);
        }
                                         

    public static void Transition(Node FromScene, String ToSceneStr,
                                  SceneTransitionEffect effect = SceneTransitionEffect.DIRECT_CUT, 
                                  float numSeconds = 0f,
                                  Boolean FadeOutAudio = false){
        var ToScene = (GD.Load(ToSceneStr) as PackedScene).Instance();
        SceneTransitioner.TransitionAlreadyLoadedScenes(FromScene, ToScene, effect, numSeconds, FadeOutAudio);}
    public static void TransitionAlreadyLoadedScenes(Node FromScene, Node ToScene,
                                   SceneTransitionEffect effect = SceneTransitionEffect.DIRECT_CUT, 
                                   float numSeconds = 0f,
                                   Boolean FadeOutAudio = false){
 
        // Initialize scenes, pause game, remove active scene
        var tree = FromScene.GetTree();
        //tree.Paused = true;
        var RootNode = tree.GetRoot();
        var transitionerScene = (GD.Load("res://scenes/frames/SceneTransitioner.tscn") as PackedScene).Instance();
        RootNode.RemoveChild(FromScene);

        transitionerScene.PauseMode = Node.PauseModeEnum.Process;
        ToScene.PauseMode = Node.PauseModeEnum.Stop;
        FromScene.PauseMode = Node.PauseModeEnum.Stop;
        tree.Paused = true;

        // Find the viewports in the TransitionScene
        ViewportContainer toViewportContainer = null;
        Viewport toViewport = null;
        ViewportContainer fromViewportContainer= null;
        Viewport fromViewport = null;
        foreach(Node child in transitionerScene.GetChildren()){
            if(child.Name.ToLower().Contains("to")){
                toViewportContainer = (ViewportContainer)child;
                toViewport = toViewportContainer.GetChild(0) as Viewport;
                toViewport.AddChild(ToScene);
            }
            if(child.Name.ToLower().Contains("from")){
                fromViewportContainer = (ViewportContainer)child;
                fromViewport = fromViewportContainer.GetChild(0) as Viewport;
                fromViewport.AddChild(FromScene);
            }
        }

        foreach(Node child in transitionerScene.GetChildren()){
            if(child.Name.ToLower().Contains("runner")){
                var runner = (TransitionEffectRunner)child;
                runner.ToScene = ToScene;
                runner.FromScene = FromScene;
                runner.TransitionerScene = transitionerScene;
                runner.ToViewport = toViewport;
                runner.FromViewport = fromViewport;
                runner.ToViewportContainer = toViewportContainer;
                runner.FromViewportContainer = fromViewportContainer;
                runner.NumSeconds = numSeconds;
                runner.elapsedSeconds = 0f;
                runner.FadeOutAudio = FadeOutAudio;
                runner.ResetActiveState(effect);
                runner.ResetActiveStateAfter(SceneTransitionEffect.DIRECT_CUT, numSeconds);
            }
        }

        //Apply the transition effect

        RootNode.AddChild(transitionerScene);
        return;
    }
}
