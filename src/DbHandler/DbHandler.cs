using Godot;
using System;
using LiteDB;
using System.Collections.Generic;
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

    public static SlotSnapshot ReadSnapshotOfSlot(int slotNum){
        //WARNING: this is a dangerous method that will cause bad behavior if slot doesnt exist
        //Use `DbHandler.ActiveSlot` instead (This method only used by main screen UI)
        var slotDbPath = DbHandler.assembleSlotDbPath(slotNum);
        if(!DbHandler.DoesSlotExist(slotNum)){
            throw new Exception("Slot doesn't exist and trying to read, exiting...");}
        using(var db = new LiteDatabase(slotDbPath)){
            var col = db.GetCollection<Slot>("slot");
            var slot =  col.Find(x => x.Id == Slot.SINGLETON_GLOBALS_ID).First();
            var levelStatsRecords = new List<LevelStatsRecord>();
            for(int levelNum = 1; levelNum <= slot.HighestLevelNumUnlocked; levelNum++){
                levelStatsRecords.Add(DbHandler.getLevelStatsRecordFor(slotDbPath, levelNum));}
            return new SlotSnapshot(){Slot = slot,
                                      LevelStatsRecords = levelStatsRecords.ToArray()};
            }}


    public static Boolean DoesSlotExist(int SlotNum){
        var dir = new Directory();
        return dir.FileExists(DbHandler.assembleSlotDbPath(SlotNum));
    }

    private static String assembleSlotDbPath(int SlotNum){
        return OS.GetUserDataDir() + "/slot" + SlotNum.ToString() + ".db";
    }

    private static String ActiveSlotDbPath { get {
        return DbHandler.assembleSlotDbPath(DbHandler.Globals.ActiveGameSlotNum);}}
 
    private static void ActiveSlotInitializationCheck(){
        if(!System.IO.File.Exists(ActiveSlotDbPath)){
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

    public static void SetCurrentLevelHoveringOver(int levelNum){
        var slot = DbHandler.ActiveSlot;
        slot.currentLevelNumHoveringOver = levelNum;
        DbHandler.ActiveSlot = slot;
    }
    public static void SetHighestLevelUnlocked(int levelNum){
        var slot = DbHandler.ActiveSlot;
        slot.HighestLevelNumUnlocked = levelNum;
        DbHandler.ActiveSlot = slot;
    }

    public static Boolean IsLevelUnlocked(int levelNum){
        return levelNum <= DbHandler.ActiveSlot.HighestLevelNumUnlocked;
    }

    public static LevelStatsRecord GetLevelStatsRecordFor(int levelNum){
        ActiveSlotInitializationCheck();
        return DbHandler.getLevelStatsRecordFor(ActiveSlotDbPath, levelNum);
    }

    private static LevelStatsRecord getLevelStatsRecordFor(String slotDbPath, int levelNum){
        if(levelNum == 0){
            // You can't store id of 0 in database, and we don't really care about storing
            // stats for the tutorial level -- so bypass the Db and return Default
            return LevelStatsRecord.Default;}
        using(var db = new LiteDatabase(slotDbPath)){
                var col = db.GetCollection<LevelStatsRecord>("levelstatsrecord");
                var output = col.FindById(levelNum);
                if(output == null){
                    GD.Print("LevelStatsRecord for " + levelNum + " doesn't exist, making one...");
                    DbHandler.SaveLevelStatsRecord(levelNum,
                                                   LevelStatsRecord.Default.TotalCalories,
                                                   LevelStatsRecord.Default.IsCompleted,
                                                   LevelStatsRecord.Default.SpaceRock1Collected,
                                                   LevelStatsRecord.Default.SpaceRock2Collected,
                                                   LevelStatsRecord.Default.SpaceRock3Collected,
                                                   false);
                    return col.FindById(levelNum);}
                else {
                    return output;}
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

    public static Dictionary<int, INonRefreshable> GetNonRefreshablesFor(int levelNum){
        if(levelNum == 0){
            new Dictionary<int, INonRefreshable>();}
        ActiveSlotInitializationCheck();
        using(var db = new LiteDatabase(ActiveSlotDbPath)){
            var col = db.GetCollection<NonRefreshable>("nonrefreshable");
            var dbNonRefreshables = col.Find(x => x.LevelNum == levelNum);
            var output = new Dictionary<int, INonRefreshable>();
            foreach(var dbNonRefreshable in dbNonRefreshables){
                output[dbNonRefreshable.UUID] = dbNonRefreshable;}
            return output;}
    }

    public static void SaveNonRefreshable(INonRefreshable n){
        using(var db = new LiteDatabase(ActiveSlotDbPath)){
            var col = db.GetCollection<NonRefreshable>("nonrefreshable");
            var nonRefreshable = new NonRefreshable(){
                Id = n.UUID,
                UUID = n.UUID,
                LevelNum = n.LevelNum}; 
            col.Insert(nonRefreshable);}
    }

    public static void ClearAllNonRefreshables(){
        using(var db = new LiteDatabase(ActiveSlotDbPath)){
            var col = db.GetCollection<NonRefreshable>("nonrefreshable");
            col.Delete(x => true);}
    }


    public static void DeleteActiveGameSlotDatabase(){
        var dir = new Directory();
        dir.Remove(ActiveSlotDbPath);
    }

    public static void DeleteGlobalsDatabase(){
        var dir = new Directory();
        dir.Remove(GlobalsDbPath);
    }

    public static void ResetInputMapToDefault(){
        DbHandler.OverrideControlAction("ui_attack",
                                        Controllers.Keyboard.Id,
                                        Controllers.Keyboard.DefaultAttackIndex);
        DbHandler.OverrideControlAction("ui_forage",
                                        Controllers.Keyboard.Id,
                                        Controllers.Keyboard.DefaultForageIndex);
         DbHandler.OverrideControlAction("ui_use_item",
                                        Controllers.Keyboard.Id,
                                        Controllers.Keyboard.DefaultUseItemIndex);
        InputMap.LoadFromGlobals(); //Not the same as our `Globals`
    }

    public static void ApplyOverrideControlsToInputMap(){
        InputMap.LoadFromGlobals(); //Different from our `Globals`
        InputEventKey ekey;
        InputEventJoypadButton ejoy;

        if(DbHandler.Globals.AttackControlOverrideController.Equals(Controllers.Keyboard.Id)){
            ekey = new InputEventKey();
            ekey.Scancode = DbHandler.Globals.AttackControlOverrideIndex;
            writeActionAndEventToInputMap("ui_attack", ekey);}
        else if(DbHandler.Globals.AttackControlOverrideController.Equals(Controllers.JoyPad.Id)){
            ejoy = new InputEventJoypadButton();
            ejoy.ButtonIndex = (int)DbHandler.Globals.AttackControlOverrideIndex;
            writeActionAndEventToInputMap("ui_attack", ejoy);}

        if(DbHandler.Globals.ForageControlOverrideController.Equals(Controllers.Keyboard.Id)){
            ekey = new InputEventKey();
            ekey.Scancode = DbHandler.Globals.ForageControlOverrideIndex;
            writeActionAndEventToInputMap("ui_forage", ekey);}
        else if(DbHandler.Globals.ForageControlOverrideController.Equals(Controllers.JoyPad.Id)){
            ejoy = new InputEventJoypadButton();
            ejoy.ButtonIndex = (int)DbHandler.Globals.ForageControlOverrideIndex;
            writeActionAndEventToInputMap("ui_forage", ejoy);}

        if(DbHandler.Globals.UseItemControlOverrideController.Equals(Controllers.Keyboard.Id)){
            ekey = new InputEventKey();
            ekey.Scancode = DbHandler.Globals.UseItemControlOverrideIndex;
            writeActionAndEventToInputMap("ui_use_item", ekey);}
        else if(DbHandler.Globals.UseItemControlOverrideController.Equals(Controllers.JoyPad.Id)){
            ejoy = new InputEventJoypadButton();
            ejoy.ButtonIndex = (int)DbHandler.Globals.UseItemControlOverrideIndex;
            writeActionAndEventToInputMap("ui_use_item", ejoy);}
    }

    private static void writeActionAndEventToInputMap(String action, InputEvent @event){
        foreach(var otherAction in new String[]{"ui_attack", "ui_forage", "ui_use_item"}){
            if(InputMap.ActionHasEvent(otherAction, @event)){
                InputMap.ActionEraseEvent(otherAction, @event);}}
        InputMap.ActionAddEvent(action, @event);}

    public static void OverrideControlAction(String action,
                                             uint controllerId,
                                             uint index){
        var globals = DbHandler.Globals;
        if(action.Equals("ui_attack")){
            globals.AttackControlOverrideController = controllerId;
            globals.AttackControlOverrideIndex = index;}
        if(action.Equals("ui_forage")){
            globals.ForageControlOverrideController = controllerId;
            globals.ForageControlOverrideIndex = index;}
        if(action.Equals("ui_use_item")){
            globals.UseItemControlOverrideController = controllerId;
            globals.UseItemControlOverrideIndex = index;}
        DbHandler.Globals = globals;
        DbHandler.ApplyOverrideControlsToInputMap();}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

