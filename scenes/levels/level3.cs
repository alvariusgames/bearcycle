using Godot;
using System;

public class level3 : LevelNode2D{
    public const String ConstTitle = "GOVERNMENT";
    public override String Title { get { return ConstTitle;}}
    public const float ConstUnitOffset = 0.066f;
    public override float UnitOffset { get { return ConstUnitOffset;}}
    public override int LevelNum { get { return 3;}}
    public override String MusicPath { get { return"res://media/music/app_forest_1.ogg"; }}
}