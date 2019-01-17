using Godot;
using System;

public sealed class PlayerStatsDisplayHandler : Node2D
{
    private Camera2D activeCamera;
    private Player activePlayer;
    private Label testLabel;
    public override void _Ready(){
           this.setPlayerAndCameraMembers();
           foreach(var child in this.GetChildren()){
               if(child is Label){
                this.testLabel = (Label)child;
               }
           }
        }
    private void setPlayerAndCameraMembers(){
           var player = this.tryGetPlayerFrom(this.GetTree().GetRoot());
            if(player != null){
                this.activePlayer = player;
                this.activeCamera = player.ATV.Bear.Camera2D;}}
    private Player tryGetPlayerFrom(Node node){
        GD.Print("Testing Node");
        GD.Print(node);
        if(node is Player){
            return (Player)node;}
        foreach(Node child in node.GetChildren()){
           var player = this.tryGetPlayerFrom(child);
           if(player is Player){
               return player;}}
        return null;}
   

        public void update(Player player){
            this.updatePlayerTotalCalories(player);
        }
            
        private void updatePlayerTotalCalories(Player player){
            GD.Print(this.activeCamera);
        }
     public override void _Process(float delta) {
        var cameraPosition = this.activeCamera.GetCameraScreenCenter();
        var visibleRectangle = this.activeCamera.GetViewport().GetVisibleRect();
        this.testLabel.SetGlobalPosition(cameraPosition); 
     }
 }
