using Godot;
using System;

public enum EndLevelState {NOT_ACTIVATED, SPINNING, ACTIVATED, _TMP_EXIT_TO_MAIN_MENU}

public class EndLevel : FSMKinematicBody2D<EndLevelState>
{
    public override EndLevelState InitialState { get { return EndLevelState.NOT_ACTIVATED;}}
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public AnimatedSprite AnimatedSprite;
    public Sprite UnactivatedSprite;
    public Sprite ActivatedSprite;
    public CollisionShape2D CollisionShape2D;
    private const float NUM_SEC_SPIN = 2f;

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

    public void EndLevel_(Player player){
        player.MoveCameraTo(this, 2f);
        this.CollisionShape2D.Disabled = true;
        this.ResetActiveState(EndLevelState.SPINNING);
        this.ResetActiveStateAfter(EndLevelState.ACTIVATED, NUM_SEC_SPIN);
        SoundHandler.StopAllStream();
        SoundHandler.PlayStream<MyAudioStreamPlayer>(this,
            new String[] {"res://media/music/short/end_level_classic.ogg"});
        SoundHandler.SetSampleVolume(SoundHandler.GetSampleVolume() * 0f);
        this.ResetActiveStateAfter(EndLevelState._TMP_EXIT_TO_MAIN_MENU, 10f); 
        DbHandler.SaveLevelStatsRecord(1, (int)player.TotalCalories, true, 
                                       player.ActiveLevel.SpaceRock1Collected,
                                       player.ActiveLevel.SpaceRock2Collected,
                                       player.ActiveLevel.SpaceRock3Collected
        );
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
            case EndLevelState._TMP_EXIT_TO_MAIN_MENU:
                SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0),
                                             ToSceneStr: "res://scenes/level_select/level_select.tscn",
                                             effect: SceneTransitionEffect.FADE_BLACK,
                                             numSeconds: 1f,
                                             FadeOutAudio: true);
                break;

        }
    }
    public override void ReactStateless(float delta){}

}
