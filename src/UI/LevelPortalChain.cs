using Godot;
using System;
using System.Collections.Generic;

public class LevelPortalChain : Node2D{
    public List<LevelPortal> LevelPortals = new List<LevelPortal>();
    private int lastUnlockedLevel = 1;
    public void UnlockLevelPortalsUpTo(int levelNum){
        this.lastUnlockedLevel = levelNum;}
    public IEnumerable<LevelPortal> UnlockedLevelPortals{ get {
        for(int i=0;i<=lastUnlockedLevel; i++){
            yield return this.LevelPortals[i];
        }
    }}
    public LevelSelectView LevelSelectView;
    public LevelSelect LevelSelect;

    public override void _Ready(){
        this.LevelSelectView = (LevelSelectView)this.GetParent();
        this.LevelSelect = (LevelSelect)this.LevelSelectView.GetParent();
        foreach(Node2D child in this.GetChildren()){
            if(child is LevelPortal){
                this.LevelPortals.Add((LevelPortal)child);}}}


}
