using Godot;
using System;

public enum EndLevelState {NOT_ACTIVATED, SPINNING, ACTIVATED, EXIT_TO_MAIN_MENU, TRIGGER_TRANSITION_TO_NEXT_ZONE}

public class EndLevel : FSMKinematicBody2D<EndLevelState>, IConsumeable, IVisibilityTrackable {
    public override EndLevelState InitialState { get { return EndLevelState.NOT_ACTIVATED;}set{}}
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public AnimatedSprite AnimatedSprite;
    public Sprite UnactivatedSprite;
    public Sprite ActivatedSprite;
    public CollisionShape2D CollisionShape2D;
    private ILevel currentLevel;
    private Player player;
    private const float NUM_SEC_SPIN = 2f;
    private const int NUM_UNITS_CAMERA_ABOVE = 300;

    [Export]
    NodePath VisibilityNotifierPath {get; set;}
    private VisibilityNotifier2D visibilityNotifier;
    public VisibilityNotifier2D VisibilityNotifier {get {
        if(this.visibilityNotifier is null){
            this.visibilityNotifier = this.GetNode<VisibilityNotifier2D>(this.VisibilityNotifierPath);}
        return this.visibilityNotifier;
    }}

    public bool IsOnScreen(){
        return this.VisibilityNotifier.IsOnScreen();
    }
    public override void _Ready(){
        foreach(Node2D child in this.GetChildren()){
            if(child is AnimatedSprite){
                this.AnimatedSprite = (AnimatedSprite)child;}
            if(child is Sprite){
                if(child.Name.ToLower().Contains("unactivated")){
                    this.UnactivatedSprite = (Sprite)child;}
                else if(child.Name.ToLower().Contains("activated")){
                    this.ActivatedSprite = (Sprite)child;}}
            if(child is CollisionShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;}}}

    public void Consume(Player player){
        this.player = player;
        this.currentLevel = this.player.ActiveLevel;
        player.Health = Player.MAX_HEALTH;
        player.Strength = Math.Max(player.Strength, Player.MAX_STRENGTH * 0.5f);
        player.MoveCameraTo(this, new Vector2(0, -NUM_UNITS_CAMERA_ABOVE), 2f);
        this.CollisionShape2D.Disabled = true;
        this.ResetActiveState(EndLevelState.SPINNING);
        this.ResetActiveStateAfter(EndLevelState.ACTIVATED, NUM_SEC_SPIN);
        SoundHandler.StopAllStream();
        SoundHandler.PlayStream<MyAudioStreamPlayer>(this,
            new String[] {"res://media/music/short/end_level_classic.ogg"});
        SoundHandler.StopAllSample(); //???
        if(this.levelIsLastZone(this.currentLevel)){
            this.ResetActiveStateAfter(EndLevelState.EXIT_TO_MAIN_MENU, 10f);
            DbHandler.SaveLevelStatsRecord(this.currentLevel.LevelNum, (int)this.player.TotalCalories, true, 
                                           player.ActiveLevel.SpaceRock1Collected,
                                           player.ActiveLevel.SpaceRock2Collected,
                                           player.ActiveLevel.SpaceRock3Collected);
            DbHandler.SetHighestLevelUnlocked(this.currentLevel.LevelNum + 1);}
        else{
            this.ResetActiveStateAfter(EndLevelState.TRIGGER_TRANSITION_TO_NEXT_ZONE, 10f);
    }}

    private Boolean levelIsLastZone(ILevel level){
        return EndLevel.GetNextZoneNodePath(level) == null;
    }

    public static String GetNextZoneNodePath(ILevel level){
        var nextZoneNumber = LevelNode2D.GetZoneNumFromNodePath(level.NodePath) + 1;
        var nextZoneNodePath = String.Format("res://scenes/levels/level{0}z{1}.tscn",
                                             level.LevelNum, nextZoneNumber);
        if((new File()).FileExists(nextZoneNodePath)){
            return nextZoneNodePath;}
        else{
            return null;}
    }
    public override void UpdateState(float delta){}
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case EndLevelState.NOT_ACTIVATED:
                this.UnactivatedSprite.Visible = true;
                this.AnimatedSprite.Visible = false;
                this.ActivatedSprite.Visible = false;
                break;
            case EndLevelState.SPINNING:
                this.UnactivatedSprite.Visible = false;
                this.AnimatedSprite.Visible = true;
                this.AnimatedSprite.Playing = true;
                this.ActivatedSprite.Visible = false;
                break;
            case EndLevelState.ACTIVATED:
                this.UnactivatedSprite.Visible = false;
                this.AnimatedSprite.Visible = false;
                this.ActivatedSprite.Visible = true;
                break;
            case EndLevelState.EXIT_TO_MAIN_MENU:
                FourDirectJoystick.SendButtonReleaseToAllDirections(); //Bug where input is still pressed going into next level...
                SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0),
                                             ToSceneStr: "res://scenes/level_select/level_select.tscn",
                                             effect: SceneTransitionEffect.FADE_BLACK,
                                             numSeconds: 1f,
                                             FadeOutAudio: true);
                break;
            case EndLevelState.TRIGGER_TRANSITION_TO_NEXT_ZONE:
                FourDirectJoystick.SendButtonReleaseToAllDirections();
                SceneTransitioner.TransitionToNextLevelZone(FromScene: this.GetTree().Root.GetChild(0),
                                                            CurrentLevel: this.currentLevel,
                                                            NextLevelZoneSceneStr: EndLevel.GetNextZoneNodePath(this.currentLevel),
                                                            NextLevelZoneNum: LevelNode2D.GetZoneNumFromNodePath(this.currentLevel.NodePath) + 1,
                                                            onLoadPlayerCalories: this.player.TotalCalories,
                                                            effect: SceneTransitionEffect.FADE_BLACK,
                                                            numSeconds: 2f,
                                                            FadeOutAudio: true);
                break;
        }
    }
    public override void ReactStateless(float delta){}

}
