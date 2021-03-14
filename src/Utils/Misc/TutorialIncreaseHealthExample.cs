using Godot;
using System;

public class TutorialIncreaseHealthExample : Node2D{
    private LevelFrame levelFrame;
    public override void _Process(float delta){
        base._Process(delta);
        var parent = this.GetParent();
        if(parent is null){
            return;}
        var grandparent = parent.GetParent();
        if(grandparent is null){
            return;}
        if(grandparent is PlayerStatsDisplayHandler){
            if(this.levelFrame is null){
                Node tmp = this;
                while(!(tmp is LevelFrame)){
                    tmp = tmp.GetParent();}
                this.levelFrame = (LevelFrame)tmp;}
            this.levelFrame.PlayerStatsDisplayHandler.totalCaloriesLabel.Visible = false;
            SoundHandler.StopAllSample();
            var player = this.levelFrame.Player;
            player.Strength += 20f * delta;
            //Pause process stuff makes the player process unreliable -- manually do the health transfer
            if(player.Strength > Player.STRENGTH_INCREASE_BOUNDARY * Player.MAX_STRENGTH){
                player.Health += 10f * delta;}
            if(player.Health > 0.85f * Player.MAX_HEALTH){
                player.Health = 0.85f * Player.MAX_HEALTH;}}
    }
}
