using Godot;
using System;

public class level4 : LevelNode2D{
    public const String ConstTitle = "MOON";
    public override String Title { get { return ConstTitle;}}
    public const float ConstUnitOffset = 0.238f;
    public override float UnitOffset { get { return ConstUnitOffset;}}
    public override int LevelNum { get { return 4;}}
    public override String MusicPath { get { return"res://media/music/app_forest_1.ogg"; }}
}