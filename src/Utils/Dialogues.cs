using Godot;
using System;
using System.Collections.Generic;

public enum DialogueEffects{ MID_PACE, PAUSE }

public enum DialoguePosition {LEFT, RIGHT}

public class Dialogue {
    public DialoguePosition Position;
    public String Text = "";
    public float NumSeconds = -1;
    public Dialogue(String translationKey, DialoguePosition position, float numSeconds = -1, DialogueEffects dialoguePace = DialogueEffects.MID_PACE){
        this.Text = (new Node()).Tr(translationKey);
        this.Position = position;
        this.NumSeconds = numSeconds;}}

public enum DialogueId {
    LEVEL1_BOSS_INTRO,
    LEVEL1_BOSS_OUTRO
}

public static class Dialogues{
    public static Dialogue[] Get(DialogueId dialogueId){
        return Dialogues.dialogues[dialogueId];}

    private static readonly Dictionary<DialogueId, Dialogue[]> dialogues = new Dictionary<DialogueId, Dialogue[]>(){
        { DialogueId.LEVEL1_BOSS_INTRO, 
        new Dialogue[]{
            new Dialogue("DIALOGUE_L1B_BOSS1", position: DialoguePosition.RIGHT),
            new Dialogue("DIALOGUE_L1B_BEAR1", position: DialoguePosition.LEFT),
            new Dialogue("DIALOGUE_L1B_BOSS2", position: DialoguePosition.RIGHT),
            new Dialogue("DIALOGUE_L1B_BOSS3", position: DialoguePosition.RIGHT), 
            new Dialogue("DIALOGUE_L1B_BEAR2", position: DialoguePosition.LEFT),
            new Dialogue("DIALOGUE_L1B_BOSS4", position: DialoguePosition.RIGHT),
            new Dialogue("DIALOGUE_L1B_BOSS5", position: DialoguePosition.RIGHT)}
        },
        { DialogueId.LEVEL1_BOSS_OUTRO, 
        new Dialogue[]{
            new Dialogue("DIALOGUE_L1B_BOSS6", position: DialoguePosition.RIGHT),
            new Dialogue("DIALOGUE_L1B_BOSS7", position: DialoguePosition.RIGHT)}
       },
    };

    public static String[] DefaultShortDialogueSamples = new String[]{
        "res://media/samples/dialogue/typing/neutral_short1.wav"
    };

    public static String[] DefaultMidDialogueSamples = new String[]{
        "res://media/samples/dialogue/typing/neutral_mid1.wav",
        "res://media/samples/dialogue/typing/neutral_mid2.wav"
    };

    public static String[] DefaultLongDialogueSamples = new String[]{
        "res://media/samples/dialogue/typing/neutral_long1.wav",
        "res://media/samples/dialogue/typing/neutral_long2.wav"
    };

}