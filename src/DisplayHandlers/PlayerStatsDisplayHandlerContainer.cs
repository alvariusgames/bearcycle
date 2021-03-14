using Godot;
using System;

public class PlayerStatsDisplayHandlerContainer : Node2D{

    public PlayerStatsDisplayHandler PlayerStatsDisplayHandler;

    public override void _Ready(){
        if(this.GetChild(0) is PlatformSpecificChildren){
            ((PlatformSpecificChildren)this.GetChild(0)).PopulateChildrenWithPlatformSpecificNodes(this);}
        foreach(var child in this.GetChildren()){
            this.PlayerStatsDisplayHandler = (PlayerStatsDisplayHandler)child;}}
}
