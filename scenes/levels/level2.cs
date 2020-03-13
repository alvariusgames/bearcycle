using Godot;
using System;

public class level2 : LevelNode2D{
    public const String ConstTitle = "SUBURBIA";
    public override String Title { get { return ConstTitle;}}
    public const float ConstUnitOffset = 0.041f;
    public override float UnitOffset { get { return ConstUnitOffset;}}
 
    public override int LevelNum { get { return 2;}}
    public override String MusicPath { get { return"res://media/music/app_forest_1.ogg"; }}
}