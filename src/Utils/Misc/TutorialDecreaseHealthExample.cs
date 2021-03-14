using Godot;
using System;
public class TutorialDecreaseHealthExample : Node2D{
    private LevelFrame levelFrame;
    private Player player;
    private Boolean hasResetHealthAndStrength = false;
    public override void _Process(float delta){
        base._Process(delta);

        var parent = this.GetParent();
        if(!hasResetHealthAndStrength && parent is null){
            this.player.Health = 0.85f * Player.MAX_HEALTH;
            this.player.Strength = 0.67f * Player.MAX_STRENGTH;
            this.levelFrame.PlayerStatsDisplayHandler.totalCaloriesLabel.Visible = true;
            this.hasResetHealthAndStrength = true;}
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
            this.player = this.levelFrame.Player;
            this.player.Strength -= 40f * delta;
            //Pause process stuff makes the player process unreliable -- manually do the health transfer
            if(this.player.Strength < Player.STRENGTH_DECREASE_BOUNDARY * Player.MAX_STRENGTH){
                this.player.Health -= 10f * delta;}
            if(this.player.Health < 0.4f * Player.MAX_HEALTH){
                this.player.Health = 0.4f * Player.MAX_HEALTH;}}
    }
}
