using Godot;
using System;

public class LevelStatsRecord
{
    public int Id { get; set; }
    public int TotalCalories { get; set; }
    public Boolean IsCompleted { get; set; }
    public Boolean SpaceRock1Collected {get; set;}
    public Boolean SpaceRock2Collected {get; set;}
    public Boolean SpaceRock3Collected {get; set;}
    public int NumTimesPlayed {get; set;}
    public String version { get; set;}

    public static LevelStatsRecord Default { get {
        return new LevelStatsRecord{
            Id = -1,
            TotalCalories = 0,
            IsCompleted = false,
            SpaceRock1Collected = false,
            SpaceRock2Collected = false,
            SpaceRock3Collected = false,
            NumTimesPlayed = 0,
            version = main.VERSION
        };
    }}}