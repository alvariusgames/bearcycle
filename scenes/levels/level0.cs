using Godot;
using System;

public class level0 : LevelNode2D{
    public const String ConstTitle = "TUTORIAL";
    public override String Title { get { return ConstTitle;}}
    public const float ConstUnitOffset = 0f;
    public override float UnitOffset { get { return ConstUnitOffset;}}
    public override int LevelNum { get { return 0;}}
    public override String MusicPath { get { return "res://media/music/misc/forest_ambience_1.ogg"; }}

    public Boolean callOnce = true;

    public override void _Process(float delta){
        base._Process(delta);
        if(callOnce){
            this.Player.CurrentHealth = 0.67f;
            this.callOnce = false;}
        if(this.Player.CurrentHealth < 0.5f * Player.MAX_HEALTH){
            this.Player.CurrentHealth = 0.5f * Player.MAX_HEALTH;} //Ya can't die in a tutorial!
    }

}