using Godot;
using System;

public interface ILevel{
    String NodePath { get;}
    Node2D NodeInst { get; }
    int LevelNum { get; }
    int Zone {get; }
    String Title { get;}
    float UnitOffset { get;}
    Boolean SpaceRock1Collected {get; set;}
    Boolean SpaceRock2Collected { get; set;}
    Boolean SpaceRock3Collected {get; set;}

}
