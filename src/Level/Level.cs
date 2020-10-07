using Godot;
using System;
using System.Collections.Generic;

public class Level : ILevel{
    private int levelNum;
    public int LevelNum { get { return levelNum;} set { this.levelNum = value;}}
    private Node2D nodeInst;
    public Node2D NodeInst { get { return nodeInst;} set { this.nodeInst = value;}}
    private String nodePath;
    public String NodePath { get { return nodePath;} set { this.nodePath = value;}}
    private String title;
    public String Title { get { return title;} set { this.title = value;}}
    private int zone;
    public int Zone { get { return zone;} set { this.zone = value;}}
    private float unitOffset;
    public float UnitOffset { get { return unitOffset;} set { this.unitOffset = value;}}
    private Boolean spaceRock1Collected;
    public Boolean SpaceRock1Collected { get { return this.spaceRock1Collected;} set{ this.spaceRock1Collected = value;}}
    private Boolean spaceRock2Collected = false;
    public Boolean SpaceRock2Collected { get { return this.spaceRock2Collected;} set{ this.spaceRock2Collected = value;}}
    private Boolean spaceRock3Collected = false;
    public Boolean SpaceRock3Collected { get { return this.spaceRock3Collected;} set{ this.spaceRock3Collected = value;}}
    public String MusicPath {get; set;}
    public String AmbiencePath {get; set;}
    public NodePath BossFightManagerPath {get; set;}
    public BossFightManager BossFightManager {get; set;}
    public Godot.Collections.Array<NodePath> TrackablePaths {get; set;}
    public List<ITrackable> Trackables {get; set;}
    public EndLevel EndLevel {get; set;}

    public static Level AssembleLevelFrom(String title, int zone, int levelNum, float unitOffset, String nodePath, LevelStatsRecord levelStatsRecord){
        if(levelStatsRecord != null){
            return new Level{
                LevelNum = levelNum,
                Title = title,
                Zone = zone,
                UnitOffset = unitOffset,
                NodePath = nodePath,
                NodeInst = null,
                spaceRock1Collected = levelStatsRecord.SpaceRock1Collected,
                spaceRock2Collected = levelStatsRecord.SpaceRock2Collected,
                spaceRock3Collected = levelStatsRecord.SpaceRock3Collected
            };}
        else {
            return new Level{
                LevelNum = levelNum,
                Title = title,
                Zone = zone,
                UnitOffset = unitOffset,
                NodePath = nodePath,
                NodeInst = null,
                spaceRock1Collected = LevelStatsRecord.Default.SpaceRock1Collected,
                spaceRock2Collected = LevelStatsRecord.Default.SpaceRock2Collected,
                spaceRock3Collected = LevelStatsRecord.Default.SpaceRock3Collected
            };
        }
    }

}
