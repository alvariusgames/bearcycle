using Godot;
using System;

public enum SceneTransitionEffect { DIRECT_CUT, FADE_INTO, FADE_BLACK }

public class SceneTransitionerArguments{
    public Node FromScene;
    public String ToSceneStr;
    public String ToLevelStr;
    public String LevelTitle;
    public int LevelZone = -1;
    public SceneTransitionEffect effect;
    public float numSeconds;
    public Boolean FadeOutAudio;}

public static class SceneTransitioner{

    public static void TransitionToLevel(Node FromScene, String ToLevelStr, String LevelTitle, int LevelZone = -1,
                                         SceneTransitionEffect effect = SceneTransitionEffect.DIRECT_CUT,
                                         float numSeconds = 0f,
                                         Boolean FadeOutAudio = false){
        var interactiveLoaderScene = (InteractiveLoader)(GD.Load("res://scenes/frames/InteractiveLoader.tscn") as PackedScene).Instance();
        interactiveLoaderScene.Args = new SceneTransitionerArguments{
            FromScene = FromScene,
            ToLevelStr = ToLevelStr,
            LevelTitle = LevelTitle,
            LevelZone = LevelZone,
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

    public static void RestartLevel(Node FromScene, ILevel Level,
                                   SafetyCheckPoint SafetyCheckPoint,
                                   int onLoadPlayerCalories,
                                   SceneTransitionEffect effect = SceneTransitionEffect.DIRECT_CUT, 
                                   float numSeconds = 0f,
                                   Boolean FadeOutAudio = false){
        var newLevel = (LevelNode2D)(GD.Load(Level.NodePath) as PackedScene).Instance();
        newLevel.SpaceRock1Collected = Level.SpaceRock1Collected;
        newLevel.SpaceRock2Collected = Level.SpaceRock2Collected;
        newLevel.SpaceRock3Collected = Level.SpaceRock3Collected;
        newLevel.OnLoadPlacePlayerAt(SafetyCheckPoint);
        newLevel.OnLoadSetPlayerCaloriesTo(onLoadPlayerCalories);
        var levelFrame = SceneTransitioner.SetLevelInLevelFrame(newLevel, Level.Title, Level.Zone);
        SceneTransitioner.TransitionAlreadyLoadedScenes(FromScene, levelFrame, effect, numSeconds, FadeOutAudio);}
 
    public static LevelFrame SetLevelInLevelFrame(Node loadedLevelScene, String LevelTitle, int LevelZone){
        var levelFrameScene = (LevelFrame)(GD.Load("res://scenes/frames/LevelFrame.tscn") as PackedScene).Instance();
        levelFrameScene._Ready();
        levelFrameScene.SetLevelTitle(LevelTitle, LevelZone);
        levelFrameScene.Viewport.AddChild(loadedLevelScene);
        return levelFrameScene;}

    public static void TransitionToNextLevelZone(Node FromScene, ILevel CurrentLevel, String NextLevelZoneSceneStr,
                                                 int onLoadPlayerCalories,
                                                 SceneTransitionEffect effect = SceneTransitionEffect.DIRECT_CUT,
                                                 int NextLevelZoneNum = 1,
                                                 float numSeconds = 0f,
                                                 Boolean FadeOutAudio = false){
        var newLevel = (LevelNode2D)(GD.Load(NextLevelZoneSceneStr) as PackedScene).Instance();
        newLevel.SpaceRock1Collected = CurrentLevel.SpaceRock1Collected;
        newLevel.SpaceRock2Collected = CurrentLevel.SpaceRock2Collected;
        newLevel.SpaceRock3Collected = CurrentLevel.SpaceRock3Collected;
        newLevel.OnLoadSetPlayerCaloriesTo(onLoadPlayerCalories);
        var levelFrame = SceneTransitioner.SetLevelInLevelFrame(newLevel, CurrentLevel.Title, NextLevelZoneNum);
        SceneTransitioner.TransitionAlreadyLoadedScenes(FromScene, levelFrame, effect, numSeconds, FadeOutAudio);
    }
}