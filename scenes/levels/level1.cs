using Godot;
using System;

public class level1 : LevelNode2D{
    public const String ConstTitle = "FOREST";
    public override String Title { get { return ConstTitle;}}
    public const float ConstUnitOffset = 0.009f;
    public override float UnitOffset { get { return ConstUnitOffset;}}
    public override int LevelNum { get { return 1;}}
    public override String MusicPath { get { return"res://media/music/app_forest_1.ogg"; }}
    public override String AmbiencePath { get { return "res://media/music/misc/forest_ambience_1.ogg";}}
}