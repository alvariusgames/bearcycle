using Godot;
using System;

public class level5 : LevelNode2D{
    public const String ConstTitle = "SPACE_SHIP";
    public override String Title { get { return ConstTitle;}}
    public override int LevelNum { get { return 5;}}
    public const float ConstUnitOffset = 0.428f;
    public override float UnitOffset { get { return ConstUnitOffset;}}
    public override String MusicPath { get { return"res://media/music/app_forest_1.ogg"; }}
}