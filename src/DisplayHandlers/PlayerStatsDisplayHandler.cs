using Godot;
using System;

public enum PlayerStatsDisplayHandlerState {DEFAULT, TRIGGER_SHOW_LAST_FOOD,
                                            END_SHOW_LAST_FOOD,
                                            SHOW_LAST_FOOD}
public class PlayerStatsDisplayHandler : FSMNode2D<PlayerStatsDisplayHandlerState>{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public override PlayerStatsDisplayHandlerState InitialState {
        get { return PlayerStatsDisplayHandlerState.DEFAULT;}}
    private Player activePlayer;
    private TextureProgress healthProgressBar;
    private Label totalCaloriesLabel;
    private Label accellLabel;
    private IFood lastFoodDisplayed;
    private Label lastFoodEatenDisplayLabel;
    private Container lastFoodEatenContainer;
    private Sprite lastFoodEatenDisplaySprite;

    public override void _Ready(){
        this.ResetActiveState(this.InitialState);
        this.setPlayerAndCameraMembers();
        foreach(Node child in this.GetChildren()){
            if(child is TextureProgress){
                this.healthProgressBar = (TextureProgress)child;}
            if((child is Label) && (child.GetName().ToLower().Contains("total"))){
                this.totalCaloriesLabel= (Label)child;}
             if((child is Label) && (child.GetName().ToLower().Contains("accell"))){
                this.accellLabel= (Label)child;}
            if(child is Container){
                this.lastFoodEatenContainer = (Container)child;
                foreach(Node subChild in child.GetChildren()){
                    if(subChild is Label){
                        this.lastFoodEatenDisplayLabel = (Label)subChild;
                    }
                    if(subChild is Sprite){
                        this.lastFoodEatenDisplaySprite = (Sprite)subChild;
                    }
                }
            }}}

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


    public override void ReactStateless(float delta)
    {
        this.healthProgressBar.Value = this.activePlayer.CurrentHealth;
        this.totalCaloriesLabel.Text = "Calories: " + this.activePlayer.TotalCalories.ToString();
        this.accellLabel.Text = "Speed: " + (int)this.activePlayer.ATV.GetAccellOfTwoWheels();
    }

    public override void UpdateState(float delta){
        if(this.activePlayer.lastFoodEaten != this.lastFoodDisplayed){
            this.ForceClearAllTimers();
            this.ResetActiveState(PlayerStatsDisplayHandlerState.TRIGGER_SHOW_LAST_FOOD);
        }
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case PlayerStatsDisplayHandlerState.TRIGGER_SHOW_LAST_FOOD:
                this.lastFoodDisplayed = this.activePlayer.lastFoodEaten;
                this.SetActiveState(PlayerStatsDisplayHandlerState.SHOW_LAST_FOOD, 200);
                this.SetActiveStateAfter(PlayerStatsDisplayHandlerState.END_SHOW_LAST_FOOD, 300, 2.5f);
                this.setUpLastFoodSprite(delta);
                this.setUpLastFoodLabel(delta);
                break;
            case PlayerStatsDisplayHandlerState.SHOW_LAST_FOOD:
                this.lastFoodEatenDisplaySprite.Rotate(0.04f);
                break;
            case PlayerStatsDisplayHandlerState.END_SHOW_LAST_FOOD:
                this.lastFoodEatenDisplayLabel.Visible = false;
                this.lastFoodEatenDisplaySprite.Visible = false;
                this.SetActiveState(PlayerStatsDisplayHandlerState.DEFAULT, 400);
                break;
            case PlayerStatsDisplayHandlerState.DEFAULT:
                break;
            default:
                throw new Exception("Invalid player handler state");
        }
    }

    private void setUpLastFoodSprite(float delta){
        //Get the size of the parent display container, scale sprite to ti
        var targetSize = this.lastFoodEatenContainer.GetRect().Size;
        var texture = this.activePlayer.lastFoodEaten.Sprite.Texture;
        var widthScale = targetSize.x / texture.GetWidth();
        var heightScale = targetSize.y / texture.GetHeight();
        //For oblong non-square sizes, always pick the smaller scale
        var targetScale = widthScale < heightScale ? widthScale : heightScale;
        this.lastFoodEatenDisplaySprite.Scale = new Vector2(targetScale, targetScale);
        this.lastFoodEatenDisplaySprite.Texture = texture;
        this.lastFoodEatenDisplaySprite.Visible = true;
        //Center the newly scaled sprite in the center of the container
        var spritePos = new Vector2(
            this.lastFoodEatenContainer.RectPosition.x + targetSize.x / 2f,
            this.lastFoodEatenContainer.RectPosition.y + targetSize.y / 2f);
        this.lastFoodEatenDisplaySprite.SetGlobalPosition(spritePos);}

    private void setUpLastFoodLabel(float delta){
        this.lastFoodEatenDisplayLabel.Visible = true;
        this.lastFoodEatenDisplayLabel.Text = this.activePlayer.lastFoodEaten.GetDisplayableName();
}}
