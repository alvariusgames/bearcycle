using Godot;
using System;

public class level1 : LevelNode2D{
    public const String ConstTitle = "Forest";
    public override String Title { get { return ConstTitle;}}
    public override int LevelNum { get { return 1;}}
    public override String MusicPath { get { return"res://media/music/app_forest_1.ogg"; }}
}