using Godot;
using System;

public interface ICharacter{
    int Id {get;set;}
    String DialogueName {get;set;}
    Texture DialogueAvatar{get;set;}
    String[] ShortDialogueSamples{get;set;}
    String[] MidDialogueSamples{get;set;}
    String[] LongDialogueSamples{get;set;}}

public class Character : ICharacter{
    public int Id{get;set;}
    public String DialogueName {get;set;}
    public Texture DialogueAvatar{get;set;}
    public String[] ShortDialogueSamples{get;set;}
    public String[] MidDialogueSamples{get;set;}
    public String[] LongDialogueSamples{get;set;}
    
    public static Character NbsRanger{
        get{ return new Character(){
            Id = CharacterIds.Level1Boss,
            DialogueName = (new Node()).Tr("UI_RANGER_DNAME"),
            DialogueAvatar = (Texture)GD.Load("res://media/sprites/npc/ranger_1/avatar.png"),
            ShortDialogueSamples = Dialogues.DefaultShortDialogueSamples,
            MidDialogueSamples = Dialogues.DefaultMidDialogueSamples,
            LongDialogueSamples = Dialogues.DefaultLongDialogueSamples};
        }
    }
}

public static class CharacterIds {
    public static int Player = 1;
    public static int Level1Boss = 2;
}

