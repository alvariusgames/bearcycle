using Godot;
using System;

public class PlayerStatsDisplayHandler : Node
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    private Player activePlayer;
    private Label healthLabel;

    private void setPlayerAndCameraMembers(){                                   
           var player = this.tryGetPlayerFrom(this.GetTree().GetRoot());        
            if(player != null){                                                 
                this.activePlayer = player;}}                                   
    private Player tryGetPlayerFrom(Node node){                                 
        if(node is Player){                                                     
            return (Player)node;}                                               
        foreach(Node child in node.GetChildren()){                              
           var player = this.tryGetPlayerFrom(child);                           
           if(player is Player){                                                
               return player;}}                                                 
        return null;}  

    public override void _Ready()
    {
        this.setPlayerAndCameraMembers();
        foreach(Node child in this.GetChildren()){
            if(child is Label){
                this.healthLabel = (Label)child;
            }
        }
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    public override void _Process(float delta)
    {
        //GD.Print(this.activePlayer);
        this.healthLabel.Text = this.activePlayer.currentHealth.ToString("0");
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
    }
}
