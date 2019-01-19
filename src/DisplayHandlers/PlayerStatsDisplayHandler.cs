using Godot;
using System;

public enum PlayerStatsDisplayHandlerState {DEFAULT, TRIGGER_SHOW_LAST_FOOD,
                                            END_SHOW_LAST_FOOD,
                                            SHOW_LAST_FOOD}
public class PlayerStatsDisplayHandler : FSMNode2D<PlayerStatsDisplayHandlerState>{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    private Player activePlayer;
    private TextureProgress healthProgressBar;
    private Label totalCaloriesLabel;
    private IFood lastFoodDisplayed;
    private Label lastFoodEatenDisplayLabel;
    private Sprite lastFoodEatenDisplaySprite;

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

    public override void _Ready(){
        this.setPlayerAndCameraMembers();
        foreach(Node child in this.GetChildren()){
            if(child is TextureProgress){
                this.healthProgressBar = (TextureProgress)child;}
            if((child is Label) && (child.GetName().ToLower().Contains("total"))){
                this.totalCaloriesLabel= (Label)child;}
            if(child is Container){
                foreach(Node subChild in child.GetChildren()){
                    if(subChild is Label){
                        this.lastFoodEatenDisplayLabel = (Label)subChild;
                    }
                    if(subChild is Sprite){
                        this.lastFoodEatenDisplaySprite = (Sprite)subChild;
                    }
                }
            }}}

    public override void ReactStateless(float delta)
    {
        this.healthProgressBar.Value = this.activePlayer.CurrentHealth;
        this.totalCaloriesLabel.Text = "Calories: " + this.activePlayer.TotalCalories.ToString();
    }

    public override void UpdateState(float delta){
        if(this.activePlayer.lastFoodEaten != this.lastFoodDisplayed){
            GD.Print("Update state...");
            this.ResetActiveState(PlayerStatsDisplayHandlerState.TRIGGER_SHOW_LAST_FOOD);
        }
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case PlayerStatsDisplayHandlerState.TRIGGER_SHOW_LAST_FOOD:
                GD.Print("Triggering showing of last food");
                this.lastFoodDisplayed = this.activePlayer.lastFoodEaten;
                this.SetActiveState(PlayerStatsDisplayHandlerState.SHOW_LAST_FOOD, 200);
                this.ResetActiveStateAfter(PlayerStatsDisplayHandlerState.END_SHOW_LAST_FOOD, 2.5f);
                this.lastFoodEatenDisplayLabel.Text = this.activePlayer.lastFoodEaten.Name;
                this.lastFoodEatenDisplaySprite.Texture = this.activePlayer.lastFoodEaten.Sprite.Texture;
                this.lastFoodEatenDisplayLabel.Visible = true;
                this.lastFoodEatenDisplaySprite.Visible = true;
                break;
            case PlayerStatsDisplayHandlerState.SHOW_LAST_FOOD:
                this.lastFoodEatenDisplaySprite.Rotate(0.04f);
                break;
            case PlayerStatsDisplayHandlerState.END_SHOW_LAST_FOOD:
                this.lastFoodEatenDisplayLabel.Visible = false;
                this.lastFoodEatenDisplaySprite.Visible = false;
                this.ResetActiveState(PlayerStatsDisplayHandlerState.DEFAULT);
                break;
            case PlayerStatsDisplayHandlerState.DEFAULT:
                break;
        }
    }

}
