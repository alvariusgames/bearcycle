using Godot;
using System;
using System.Collections.Generic;


public enum BossFightManagerState{NOT_ACTIVE, 
    TRIGGER_ACTIVATION, ACTIVATION_TRANSITION,
    PRE_INTRO_LOADING,
    TRIGGER_INTRO_DIALOGUE, INTRO_DIALOGUE,
    TRIGGER_FIGHT, INTRO_DIALOGUE_TO_FIGHT_TRANSITION, FIGHT,
    TRIGGER_END_FIGHT, FIGHT_TO_END_FIGHT_TRANSITION_1, FIGHT_TO_END_FIGHT_TRIGGER_1_TO_2, FIGHT_TO_END_FIGHT_TRANSITION_2, END_FIGHT,
    TRIGGER_DEFEATED, END_FIGHT_TO_DEFEATED_TRANSITION_1, END_FIGHT_TO_DEFEATED_TRIGGER_1_TO_2, END_FIGHT_TO_DEFEATED_TRANSITION_2, DEFEATED}
public abstract class BossFightManager : FSMNode2D<BossFightManagerState>, ITrackable{
    [Export]
    public Texture BossFightUIAvatar {get; set;}
    [Export]
    public bool BumpPlayerHealthTo50PercHealthOnStartOfFight {get; set;} = true;
    [Export]
    public bool BumpPlayerHealthTo50PercHealthOnEndOfFight {get; set;} = true;

    public override BossFightManagerState InitialState{get { return BossFightManagerState.NOT_ACTIVE;} set{}}
    [Export]
    public String IntroMusic {get; set;} = "res://media/music/short/boss_theme1_dialogue.ogg";
    [Export]
    public DialogueId IntroDialogue{get; set;}
    [Export] 
    public String FightMusic {get; set;} = "res://media/music/boss_theme1.ogg";
    [Export]
    public String OutroMusic {get; set;} = "res://media/music/misc/trance.ogg";
    [Export]
    public DialogueId OutroDialogue{get; set;}
    public TriggerBossFight TriggerBossFight;
    public Node2D IntroDialogueCameraFocus;
    private ILevel level;
    [Export]
    public bool DisplayDiscreteFightUnits {get; set; } = true;
    [Export]
    public NodePath FreeUnreachablePath {get; set;}
    private FreeUnreachable freeUnreachable;
    public FreeUnreachable FreeUnreachable { get {
        if(this.freeUnreachable is null){
            this.freeUnreachable = this.GetNode<FreeUnreachable>(this.FreeUnreachablePath);}
        return this.freeUnreachable;}}

    public ILevel Level{get{
        if(this.level == null){
            this.level = this.getActiveLevel(this);}
        return this.level;}}
    private Player player;
    public Player Player { get {
        if(this.player == null){
            this.player = this.getActivePlayer(this.Level.NodeInst);}
        return this.player;}}

    public IBoss Boss;
    private float tempFadeoutLinearUnit = 1f;

    private ILevel getActiveLevel(Node n){
        if(n is ILevel){
            return (ILevel)n;}
        else{
            return this.getActiveLevel(n.GetParent());}}

    private Player getActivePlayer(Node n){
        if(n is Player){
            return (Player)n;}
        foreach(Node child in n.GetChildren()){
            var potPlayer = this.getActivePlayer(child);
            if(potPlayer != null){
                return potPlayer;}}
        return null;}

    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child is TriggerBossFight){
                this.TriggerBossFight = (TriggerBossFight)child;}
            if(child.Name.ToLower().Contains("introdialoguecamerafocus")){
                this.IntroDialogueCameraFocus = (Node2D)child;}}}

    public abstract void OnActivation();
    public abstract Boolean FightIsLoadedProcess();
    public override void ReactStateless(float delta){}
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case BossFightManagerState.NOT_ACTIVE:
                if(this.TriggerBossFight.IsCurrentlyCollidingWithPlayer){
                    if(this.BumpPlayerHealthTo50PercHealthOnStartOfFight){
                        this.Player.Health = Math.Max((Player.MAX_HEALTH / 2f),
                                                       this.Player.Health);}
                    this.ResetActiveState(BossFightManagerState.TRIGGER_ACTIVATION);}
                break;
            case BossFightManagerState.TRIGGER_ACTIVATION:
                this.Player.ATV.FrontWheel.SetActiveState(WheelState.LOCKED, Priorities.BossFight);
                this.Player.ATV.BackWheel.SetActiveState(WheelState.LOCKED, Priorities.BossFight);
                this.ResetActiveState(BossFightManagerState.ACTIVATION_TRANSITION);
                this.ResetActiveStateAfter(BossFightManagerState.PRE_INTRO_LOADING, 1.1f);
                this.tempFadeoutLinearUnit = 1f;
                this.Player.MoveCameraTo(this.IntroDialogueCameraFocus, new Vector2(0,0));
                break;
            case BossFightManagerState.ACTIVATION_TRANSITION:
                SoundHandler.TempSetAllStream(this.tempFadeoutLinearUnit);
                this.tempFadeoutLinearUnit -= (delta / 2f);
                break;
            case BossFightManagerState.PRE_INTRO_LOADING:
                if(this.FightIsLoadedProcess()){
                    this.ResetActiveState(BossFightManagerState.TRIGGER_INTRO_DIALOGUE);
                }
                break;
            case BossFightManagerState.TRIGGER_INTRO_DIALOGUE:
                this.Player.LevelFrame.DialogueHandler.StartDialogueBetween(this.Player, Character.NbsRanger, 
                                                                            Dialogues.Get(this.IntroDialogue));
                SoundHandler.EndTempSetAllStreamAndSample();
                SoundHandler.StopStream(this, this.Level.MusicPath);
                SoundHandler.PlayStream<MyAudioStreamPlayer>(this, 
                    this.IntroMusic, Loop: true);
                this.ResetActiveState(BossFightManagerState.INTRO_DIALOGUE);
                this.OnActivation();
                break;
            case BossFightManagerState.INTRO_DIALOGUE:
                if(this.Player.LevelFrame.DialogueHandler.CurrentDialogueIsFinished()){
                    this.tempFadeoutLinearUnit = 1f;
                    this.ResetActiveState(BossFightManagerState.INTRO_DIALOGUE_TO_FIGHT_TRANSITION);
                    this.ResetActiveStateAfter(BossFightManagerState.TRIGGER_FIGHT, 1f);
                    this.Player.ResetCameraToDefaultFollowPlayerBehavior();}
                break;
            case BossFightManagerState.INTRO_DIALOGUE_TO_FIGHT_TRANSITION:
                SoundHandler.TempSetAllStream(this.tempFadeoutLinearUnit);
                this.tempFadeoutLinearUnit -= (delta / 1f);
                break;
            case BossFightManagerState.TRIGGER_FIGHT:
                SoundHandler.EndTempSetAllStreamAndSample();
                SoundHandler.StopStream(this, this.IntroMusic);
                SoundHandler.PlayStream<MyAudioStreamPlayer>(this, 
                    this.FightMusic, Loop: true);
                this.Player.ATV.FrontWheel.ResetActiveState(WheelState.IDLING);
                this.Player.ATV.BackWheel.ResetActiveState(WheelState.IDLING);
                this.ResetActiveState(BossFightManagerState.FIGHT);
                this.StartOfFight(delta);
                break;
            case BossFightManagerState.FIGHT:
                this._FightProcess(delta);
                if(this.IsFightOver(delta)){
                    this.ResetActiveState(BossFightManagerState.FIGHT_TO_END_FIGHT_TRANSITION_1);
                    this.ResetActiveStateAfter(BossFightManagerState.FIGHT_TO_END_FIGHT_TRIGGER_1_TO_2, 1f);
                    this.tempFadeoutLinearUnit = 1f;
                    this.Player.ATV.FrontWheel.SetActiveState(WheelState.LOCKED, Priorities.BossFight);
                    this.Player.ATV.BackWheel.SetActiveState(WheelState.LOCKED, Priorities.BossFight);}
                break;
            case BossFightManagerState.FIGHT_TO_END_FIGHT_TRANSITION_1:
                SoundHandler.TempSetAllStream(this.tempFadeoutLinearUnit);
                this.tempFadeoutLinearUnit -= (delta / 1f);
                break;
            case BossFightManagerState.FIGHT_TO_END_FIGHT_TRIGGER_1_TO_2:
                SoundHandler.StopStream(this, this.FightMusic);
                SoundHandler.PlayStream<MyAudioStreamPlayer>(this,
                    this.OutroMusic, Loop: true);
                this.ResetActiveState(BossFightManagerState.FIGHT_TO_END_FIGHT_TRANSITION_2);
                this.ResetActiveStateAfter(BossFightManagerState.TRIGGER_END_FIGHT, 1f);
                this.tempFadeoutLinearUnit = 0f;
                break;
            case BossFightManagerState.FIGHT_TO_END_FIGHT_TRANSITION_2:
                SoundHandler.TempSetAllStream(this.tempFadeoutLinearUnit);
                this.tempFadeoutLinearUnit += (delta / 1f);
                break;
            case BossFightManagerState.TRIGGER_END_FIGHT:
                this.Player.LevelFrame.DialogueHandler.StartDialogueBetween(this.Player, Character.NbsRanger,
                                                                            Dialogues.Get(this.OutroDialogue));
                SoundHandler.EndTempSetAllStreamAndSample();
                SoundHandler.StopStream(this, this.FightMusic);
                this.ResetActiveState(BossFightManagerState.END_FIGHT);
                this.EndOfFight(delta);
                if(this.BumpPlayerHealthTo50PercHealthOnEndOfFight){
                    this.Player.Health = Math.Max((Player.MAX_HEALTH / 2f),
                                                   this.Player.Health);}
                break;
            case BossFightManagerState.END_FIGHT:
                if(this.Player.LevelFrame.DialogueHandler.CurrentDialogueIsFinished()){
                    this.ResetActiveState(BossFightManagerState.END_FIGHT_TO_DEFEATED_TRANSITION_1);
                    this.ResetActiveStateAfter(BossFightManagerState.END_FIGHT_TO_DEFEATED_TRIGGER_1_TO_2, 1f);
                    this.tempFadeoutLinearUnit = 1f;}
                break;
            case BossFightManagerState.END_FIGHT_TO_DEFEATED_TRANSITION_1:
                SoundHandler.TempSetAllStream(this.tempFadeoutLinearUnit);
                this.tempFadeoutLinearUnit -= (delta / 1f);
                break;
            case BossFightManagerState.END_FIGHT_TO_DEFEATED_TRIGGER_1_TO_2:
                SoundHandler.StopStream(this, this.OutroMusic);
                SoundHandler.PlayStream<MyAudioStreamPlayer>(this.Level.NodeInst, this.Level.MusicPath, fromPosition: 5f, VolumeMultiplier: 0f);
                this.ResetActiveState(BossFightManagerState.FIGHT_TO_END_FIGHT_TRANSITION_2);
                this.ResetActiveStateAfter(BossFightManagerState.TRIGGER_DEFEATED, 1f);
                this.tempFadeoutLinearUnit = 0f; 
                break;
            case BossFightManagerState.END_FIGHT_TO_DEFEATED_TRANSITION_2:
                SoundHandler.TempSetAllStream(this.tempFadeoutLinearUnit);
                this.tempFadeoutLinearUnit += (delta / 1f);
                break;
            case BossFightManagerState.TRIGGER_DEFEATED:
                SoundHandler.EndTempSetAllStreamAndSample();
                this.Player.ATV.FrontWheel.ResetActiveState(WheelState.IDLING);
                this.Player.ATV.BackWheel.ResetActiveState(WheelState.IDLING);
                this.ResetActiveState(BossFightManagerState.DEFEATED);
                break;
            case BossFightManagerState.DEFEATED:

                break;
        }
    }
    public override void UpdateState(float delta){}
    public abstract float NumFightUnitsCompleted {get; set;}
    public abstract float NumFightUnitsTotal {get; set;}
    public abstract Boolean IsFightOver(float delta);
    public abstract void StartOfFight(float delta);
    public abstract void _FightProcess(float delta);
    public abstract void EndOfFight(float delta);
    public abstract IEnumerable<Node2D> NodesToTrack {get; }
    public abstract Boolean ShouldTrackNodesNow {get;}

}
