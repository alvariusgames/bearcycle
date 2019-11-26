using Godot;
using System;
using System.Collections.Generic;

public class LevelPortalChain : Node2D{
    public List<LevelPortal> LevelPortals = new List<LevelPortal>();
    public LevelSelect LevelSelect;

    public override void _Ready(){
        if(this.GetParent() is LevelSelect){
            this.LevelSelect = (LevelSelect)this.GetParent();}
        foreach(Node2D child in this.GetChildren()){
            if(child is LevelPortal){
                this.LevelPortals.Add((LevelPortal)child);}}}


}
