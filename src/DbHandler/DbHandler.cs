using Godot;
using System;
using LiteDB;
using System.Linq;
public static class DbHandler{
    private static String GlobalsDbPath = OS.GetUserDataDir() + "/globals.db";
   private static void GlobalsInitializationCheck(){
        var file = new File();
        if(!file.FileExists(GlobalsDbPath)){
            GD.Print("Globals database does not exist, making one...");
            using(var db = new LiteDatabase(GlobalsDbPath)){
                var col = db.GetCollection<Globals>("globals");
                col.Insert(Globals.Default);}}}

    public static Globals Globals { 
        get {
            GlobalsInitializationCheck();
            using(var db = new LiteDatabase(GlobalsDbPath)){
                var col = db.GetCollection<Globals>("globals");
                col = db.GetCollection<Globals>("globals");
                return col.Find(x => x.Id == Globals.SINGLETON_GLOBALS_ID).First();
                }
            }
        set {
            GlobalsInitializationCheck();
            using(var db = new LiteDatabase(GlobalsDbPath)){
                 var col = db.GetCollection<Globals>("globals");
                 col = db.GetCollection<Globals>("globals");
                 col.Update(value);
            }
        }
    }

    private static String ActiveSlotDbPath { get {
        return OS.GetUserDataDir() + "/slot" + DbHandler.Globals.ActiveGameSlotNum + ".db"; }}
 
    private static void ActiveSlotInitializationCheck(){
        var file = new File();
        if(!file.FileExists(ActiveSlotDbPath)){
            GD.Print("Slot database does not exist, making one...");
            using(var db = new LiteDatabase(ActiveSlotDbPath)){
                var col = db.GetCollection<Slot>("slot");
                col.Insert(Slot.Default);}}}


    public static Slot ActiveSlot { 
        get {
            ActiveSlotInitializationCheck();
            using(var db = new LiteDatabase(ActiveSlotDbPath)){
                var col = db.GetCollection<Slot>("slot");
                return col.Find(x => x.Id == Slot.SINGLETON_GLOBALS_ID).First();
                }
            }
        set {
            ActiveSlotInitializationCheck();
            using(var db = new LiteDatabase(ActiveSlotDbPath)){
                 var col = db.GetCollection<Slot>("slot");
                 col = db.GetCollection<Slot>("slot");
                 col.Update(value);
            }
        }
    }

    public static LevelStatsRecord GetLevelStatsRecordFor(int levelNum){
        ActiveSlotInitializationCheck();
        using(var db = new LiteDatabase(ActiveSlotDbPath)){
                var col = db.GetCollection<LevelStatsRecord>("levelstatsrecord");
                return col.FindById(levelNum);
        }
    }

    public static void SaveLevelStatsRecord(
        int levelNum, int TotalCalories, Boolean IsCompleted,
        Boolean spaceRock1Collected, Boolean spaceRock2Collected, 
        Boolean spaceRock3Collected, Boolean incrementNumTimesPlayed = true){

        ActiveSlotInitializationCheck();
        using(var db = new LiteDatabase(ActiveSlotDbPath)){
                var col = db.GetCollection<LevelStatsRecord>("levelstatsrecord");
                var existingRecord = col.FindById(levelNum);
                var recordToWrite = new LevelStatsRecord(){
                    Id = levelNum,
                    TotalCalories = TotalCalories,
                    IsCompleted = IsCompleted,
                    SpaceRock1Collected = spaceRock1Collected,
                    SpaceRock2Collected = spaceRock2Collected,
                    SpaceRock3Collected = spaceRock3Collected,
                    NumTimesPlayed = 1};
                if(existingRecord != null){
                    if(existingRecord.TotalCalories > TotalCalories){
                        recordToWrite.TotalCalories = existingRecord.TotalCalories;
                    } else {
                        recordToWrite.TotalCalories = TotalCalories;}
                    if(existingRecord.SpaceRock1Collected){
                        recordToWrite.SpaceRock1Collected = true;}
                    if(existingRecord.SpaceRock2Collected){
                        recordToWrite.SpaceRock2Collected = true;}
                    if(existingRecord.SpaceRock3Collected){
                        recordToWrite.SpaceRock3Collected = true;}
                    if(incrementNumTimesPlayed){
                        recordToWrite.NumTimesPlayed = existingRecord.NumTimesPlayed + 1;}
                // Update the DB
                    col.Update(recordToWrite);}
                else {
                    col.Insert(recordToWrite);
                }
       }
    }

    public static void DeleteGlobalsAndActiveGameSlotDbs(){
        DeleteActiveGameSlotDatabase();
        DeleteGlobalsDatabase();
    }

    private static void DeleteActiveGameSlotDatabase(){
        var dir = new Directory();
        dir.Remove(ActiveSlotDbPath);
    }

    private static void DeleteGlobalsDatabase(){
        var dir = new Directory();
        dir.Remove(GlobalsDbPath);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

