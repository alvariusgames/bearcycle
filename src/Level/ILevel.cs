using Godot;
using System;

public interface ILevel{
    Node2D NodeInst { get; }
    int LevelNum { get; }
    String Title { get;}
    Boolean SpaceRock1Collected {get; set;}
    Boolean SpaceRock2Collected { get; set;}
    Boolean SpaceRock3Collected {get; set;}

}
