using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum DialogueHandlerState{WAITING_FOR_ACTIVATION,
                                 TRIGGER_NEXT_DIALOGUE,
                                 TRIGGER_CHARACTER_A_TALKING, CHARACTER_A_TALKING,
                                 TRIGGER_CHARACTER_B_TALKING, CHARACTER_B_TALKING,
                                 TRIGGER_RELEASE_CONTROL}

public enum DialoguePromptType {NONE, FREEZE_PLAYER_INPUT_USER_PROMPT}

public class DialogueHandler : FSMNode2D<DialogueHandlerState>{
    public LevelFrame LevelFrame;
    private Sprite CharacterADialogueBackdrop;
    private Sprite CharacterAAvatar;
    private Label CharacterAName;
    private Label CharacterADialogue;
    private Sprite CharacterBDialogueBackdrop;
    private Sprite CharacterBAvatar;
    private Label CharacterBName;
    private Label CharacterBDialogue;
    public override DialogueHandlerState InitialState { get { return DialogueHandlerState.WAITING_FOR_ACTIVATION;} set{}}
    private ICharacter CharacterA;
    private ICharacter CharacterB;
    private Queue<Dialogue> unshownDialogues;
    private DialoguePromptType DialoguePromptType = DialoguePromptType.NONE;
    private Dialogue activeDialogue;
    private const float ONE_LETTER_TYPE_SPEED_SEC = 0.05f;
    private const float SPACE_TYPE_SPEED_SEC = 0.00f;
    private const float PUNCTUATION_TYPE_SPEED_LONG = 1f;
    private const float PUNCTUATION_TYPE_SPEED_SHORT = 0.1f;
    private float typingAggrSec = 0f;
    private int typingStrIndex = 0;
    private float typingAudioAggrSec = 0f;
    private const float TYPING_AUDIO_DEADZONE = 0.6f;

    public override void _Ready(){
        this.LevelFrame = (LevelFrame)this.GetParent();
        foreach(Node child in this.GetChildren()){
            if(child is PlatformSpecificChildren){
                ((PlatformSpecificChildren)child).PopulateChildrenWithPlatformSpecificNodes(this);}}
        foreach(Node child in this.GetChildren()){
            if(child is Sprite && child.Name.ToLower().Contains("characteradialoguebackdrop")){
                this.CharacterADialogueBackdrop = (Sprite)child;
                foreach(Node subChild in child.GetChildren()){
                    if(subChild is Sprite){
                        this.CharacterAAvatar = (Sprite)subChild;}
                    if(subChild is Label && subChild.Name.ToLower().Contains("name")){
                        this.CharacterAName = (Label)subChild;}
                    if(subChild is Label && subChild.Name.ToLower().Contains("dialogue")){
                        this.CharacterADialogue = (Label)subChild;}}}
            if(child is Sprite && child.Name.ToLower().Contains("characterbdialoguebackdrop")){
                this.CharacterBDialogueBackdrop = (Sprite)child;
                foreach(Node subChild in child.GetChildren()){
                    if(subChild is Sprite){
                        this.CharacterBAvatar = (Sprite)subChild;}
                    if(subChild is Label && subChild.Name.ToLower().Contains("name")){
                        this.CharacterBName = (Label)subChild;}
                    if(subChild is Label && subChild.Name.ToLower().Contains("dialogue")){
                        this.CharacterBDialogue = (Label)subChild;}}}}
        this.Visible = false;
        this.CharacterADialogueBackdrop.Visible = false;
        this.CharacterBDialogueBackdrop.Visible = false;
    }

    public void StartDialogueBetween(ICharacter characterA, ICharacter characterB, Dialogue[] dialogues,
                                     DialoguePromptType dialoguePromptType = DialoguePromptType.FREEZE_PLAYER_INPUT_USER_PROMPT){
        this.Visible = true;
        this.LevelFrame.PlayerStatsDisplayHandler.Visible = false;
        this.CharacterADialogueBackdrop.Visible = false;
        this.CharacterBDialogueBackdrop.Visible = false;
        this.Visible = true;
        this.CharacterA = characterA;
        this.CharacterB = characterB;
        this.DialoguePromptType = dialoguePromptType;
        this.unshownDialogues = new Queue<Dialogue>();
        foreach(var dialogue in dialogues){
            this.unshownDialogues.Enqueue(dialogue);}
        this.ResetActiveState(DialogueHandlerState.TRIGGER_NEXT_DIALOGUE);
    }

    public Boolean CurrentDialogueIsFinished(){
        //Not a perfect solution, could return dialogue as "finished" before even starting...
        return this.ActiveState.Equals(DialogueHandlerState.TRIGGER_RELEASE_CONTROL) ||
               this.ActiveState.Equals(DialogueHandlerState.WAITING_FOR_ACTIVATION);
    }

    public override void ReactStateless(float delta){
        this.typingAudioAggrSec += delta;
    }

    private void reactToDialoguePromptType(){
        this.freezePlayerIfApplicable();
    }

    private void freezePlayerIfApplicable(){
        if(this.DialoguePromptType.Equals(DialoguePromptType.FREEZE_PLAYER_INPUT_USER_PROMPT)){
            this.LevelFrame.Player.ATV.FrontWheel.SetActiveState(WheelState.LOCKED, Priorities.FreezePlayerDialogue);
            this.LevelFrame.Player.ATV.BackWheel.SetActiveState(WheelState.LOCKED, Priorities.FreezePlayerDialogue);
            this.LevelFrame.Player.ClawAttack.SetActiveState(ClawAttackState.LOCKED, Priorities.FreezePlayerDialogue);
            this.LevelFrame.Player.ATV.SetVelocityOfTwoWheels(new Vector2(0,0));
        }
    }

    private void releaseControlDialoguePromptType(){
        this.unfreezePlayerIfApplicable();
    }

    private void unfreezePlayerIfApplicable(){
        if(this.DialoguePromptType.Equals(DialoguePromptType.FREEZE_PLAYER_INPUT_USER_PROMPT)){
            this.LevelFrame.Player.ATV.FrontWheel.ResetActiveState(WheelState.IDLING);
            this.LevelFrame.Player.ATV.BackWheel.ResetActiveState(WheelState.IDLING);
            this.LevelFrame.Player.ClawAttack.ResetActiveState(ClawAttackState.NORMAL);}}

    public override void UpdateState(float delta){
        //TODO: remove me before final
        if(main.IsDebug && Input.IsActionJustPressed("ui_forage")){
            this.ResetActiveState(DialogueHandlerState.TRIGGER_RELEASE_CONTROL);
        }
    }

    private Boolean userIsSelecting(){
        return Input.IsActionJustReleased("ui_accept") || Input.IsActionJustReleased("mouse_left_button");}

    private Boolean textHasFullyCompleted(){
        try{
            return this.typingStrIndex == this.activeDialogue.Text.Length;
        } catch{
            return false;}}

    private void typeTextProcess(float delta, Label labelToUpdate){
        reactToDialoguePromptType();
        handleDialogueAudio(delta);
        if(this.textHasFullyCompleted() && this.userIsSelecting()){
            this.ResetActiveState(DialogueHandlerState.TRIGGER_NEXT_DIALOGUE);}
        if(this.userIsSelecting()){
            this.typingStrIndex = this.activeDialogue.Text.Length - 1;}

        this.typingAggrSec += delta;
        if(this.typingAggrSec > ONE_LETTER_TYPE_SPEED_SEC){
            this.typingAggrSec = 0f;
            if(this.typingStrIndex < this.activeDialogue.Text.Length){
                this.subtractTimeForCurrCharacter();
                this.typingStrIndex++;
                var str = this.activeDialogue.Text.Substring(0, typingStrIndex);
                labelToUpdate.Text = str;
            }
        }
    }

    private char currChar{ get{
        try{
            return this.activeDialogue.Text[this.typingStrIndex];}
        catch{
            return '0';}}}

    private char nextChar{ get {
        try{
            return this.activeDialogue.Text[typingStrIndex + 1];}
        catch{
            return '\0';}}}

    private void handleDialogueAudio(float delta){
        Sprite characterDialogueBackdrop = null;
        ICharacter character = null;
        if(this.ActiveState.Equals(DialogueHandlerState.CHARACTER_A_TALKING)){
            characterDialogueBackdrop = this.CharacterADialogueBackdrop;
            character = this.CharacterA;}
        if(this.ActiveState.Equals(DialogueHandlerState.CHARACTER_B_TALKING)){
            characterDialogueBackdrop = this.CharacterBDialogueBackdrop;
            character = this.CharacterB;}
        if(characterDialogueBackdrop is null){
            return;}
        
        var punctuation = new char[]{ '.', ',', '!', ';', ':'};
        var isNaturalSpotToPlayAudio = punctuation.Contains(this.currChar) &&
                                       punctuation.Contains(this.nextChar);
        isNaturalSpotToPlayAudio = isNaturalSpotToPlayAudio ||
                                   punctuation.Contains(this.currChar) &&
                                   (this.nextChar.Equals(' ') || this.nextChar.Equals('\0'));
        isNaturalSpotToPlayAudio = isNaturalSpotToPlayAudio || 
                                    this.currChar.Equals(' ');
        if(!this.textHasFullyCompleted() && isNaturalSpotToPlayAudio){
            this.playDialogueAudioFor(delta, characterDialogueBackdrop, character);
        }
    }

    private void playDialogueAudioFor(float delta, Node2D caller, ICharacter character){
        var rand = new Random();
        List<String> dialogueSamples = new List<String>();
        dialogueSamples.AddRange(character.ShortDialogueSamples);
        dialogueSamples.AddRange(character.MidDialogueSamples);
        dialogueSamples.AddRange(character.LongDialogueSamples);

        var dialogueSamplePath = dialogueSamples[rand.Next(dialogueSamples.Count)];
        //TODO: make positional audio so Char A talking is left ear, Char B is right
        if(this.typingAudioAggrSec > TYPING_AUDIO_DEADZONE){
            this.typingAudioAggrSec = 0f;
            SoundHandler.PlaySample<MyAudioStreamPlayer>(caller,
                                                         dialogueSamplePath,
                                                         SkipIfAlreadyPlaying: true);}}

    private void subtractTimeForCurrCharacter(){
        if(this.currChar.Equals(' ')){
            this.typingAggrSec -= SPACE_TYPE_SPEED_SEC;}
        var punctuation = new char[]{ '.', ',', '!', ';', ':'};
        if(punctuation.Contains(this.currChar) && punctuation.Contains(this.nextChar)){
            this.typingAggrSec -= PUNCTUATION_TYPE_SPEED_SHORT;}
        if(punctuation.Contains(this.currChar) && !this.nextChar.Equals(' ')){
            this.typingAggrSec -= PUNCTUATION_TYPE_SPEED_SHORT;}
        else if(punctuation.Contains(this.currChar) && !punctuation.Contains(this.nextChar)){
            this.typingAggrSec -= PUNCTUATION_TYPE_SPEED_LONG;}}

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case DialogueHandlerState.WAITING_FOR_ACTIVATION:
                break;
            case DialogueHandlerState.TRIGGER_NEXT_DIALOGUE:
                if(this.unshownDialogues.Count == 0){
                    this.ResetActiveState(DialogueHandlerState.TRIGGER_RELEASE_CONTROL);
                    break;}
                this.activeDialogue = this.unshownDialogues.Dequeue();
                if(this.activeDialogue.Position.Equals(DialoguePosition.LEFT)){
                    this.ResetActiveState(DialogueHandlerState.TRIGGER_CHARACTER_A_TALKING);}
                if(this.activeDialogue.Position.Equals(DialoguePosition.RIGHT)){
                    this.ResetActiveState(DialogueHandlerState.TRIGGER_CHARACTER_B_TALKING);}
                break;
            case DialogueHandlerState.TRIGGER_CHARACTER_A_TALKING:
                this.CharacterADialogueBackdrop.Visible = true;
                this.CharacterBDialogueBackdrop.Visible = false;
                this.CharacterAAvatar.Texture = this.CharacterA.DialogueAvatar;
                this.CharacterAName.Text = this.CharacterA.DialogueName;
                this.CharacterADialogue.Text = "";
                this.typingAggrSec = 0f;
                this.typingStrIndex = 0;
                this.ResetActiveState(DialogueHandlerState.CHARACTER_A_TALKING);
                break;
            case DialogueHandlerState.CHARACTER_A_TALKING:
                this.typeTextProcess(delta, this.CharacterADialogue);
                break;
            case DialogueHandlerState.TRIGGER_CHARACTER_B_TALKING:
                this.CharacterADialogueBackdrop.Visible = false;
                this.CharacterBDialogueBackdrop.Visible = true;
                this.CharacterBAvatar.Texture = this.CharacterB.DialogueAvatar;
                this.CharacterBName.Text = this.CharacterB.DialogueName;
                this.CharacterBDialogue.Text = "";
                this.typingAggrSec = 0f;
                this.typingStrIndex = 0;
                this.ResetActiveState(DialogueHandlerState.CHARACTER_B_TALKING);
                break;
            case DialogueHandlerState.CHARACTER_B_TALKING:
                this.typeTextProcess(delta, this.CharacterBDialogue);
                break;
            case DialogueHandlerState.TRIGGER_RELEASE_CONTROL:
                this.CharacterADialogueBackdrop.Visible = false;
                this.CharacterBDialogueBackdrop.Visible = false;
                this.Visible = false;
                this.LevelFrame.PlayerStatsDisplayHandler.Visible = true;
                this.releaseControlDialoguePromptType();
                this.DialoguePromptType = DialoguePromptType.NONE;
                this.CharacterA = null;
                this.CharacterB = null;
                this.ResetActiveState(DialogueHandlerState.WAITING_FOR_ACTIVATION);
                break;
            default:
                break;
        }
    }

}
