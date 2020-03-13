using Godot;
using System;

public class level6 : LevelNode2D{
    public const String ConstTitle = "FINAL";
    public override String Title { get { return ConstTitle;}}
    public override int LevelNum { get { return 4;}}
    public const float ConstUnitOffset = 0.6787f;
    public override float UnitOffset { get { return ConstUnitOffset;}}
    public override String MusicPath { get { return"res://media/music/app_forest_1.ogg"; }}
}