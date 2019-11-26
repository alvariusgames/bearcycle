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
    private LevelFrameBannerBase baseHolder;
    private Sprite speedometerMeter;
    private TextureProgress healthProgressBar;
    private Label totalCaloriesLabel;
    private Label accellLabel;
    private IFood lastFoodDisplayed;
    private Label lastFoodEatenDisplayLabel;
    private Container lastFoodEatenContainer;
    private Sprite lastFoodEatenDisplaySprite;
    private AnimatedSprite InteractableFlashPrompt;
    private AnimatedSprite HealthWarningFlashPrompt;
    private AnimatedSprite LastFoodEatenBorder;
    private int interactableFlashPromptCounter = 0;
    private Sprite HoldableIcon;
    private Sprite NumActionCallsGraphic1To6;
    private Color InitialInteractableFlashPromptModulate;

    public override void _Ready(){
        if(this.GetChild(0) is PlatformSpecificChildren){
            ((PlatformSpecificChildren)this.GetChild(0)).PopulateChildrenWithPlatformSpecificNodes(this);}
        this.ResetActiveState(this.InitialState);
        this.setPlayerAndCameraMembers();
        foreach(Node child in this.GetChildren()){
            if(child is Sprite && child.Name.ToLower().Contains("speedometermeter")){
                this.speedometerMeter = (Sprite)child;}
            if(child is AnimatedSprite && child.Name.Contains("nteractable")){
                this.InteractableFlashPrompt = (AnimatedSprite)child;
                this.InitialInteractableFlashPromptModulate = this.InteractableFlashPrompt.Modulate;}
            if(child is AnimatedSprite && child.Name.ToLower().Contains("healthwarningflashprompt")){
                this.HealthWarningFlashPrompt = (AnimatedSprite)child;}
            if(child is Sprite && child.Name.Contains("oldable")){
                this.HoldableIcon = (Sprite)child;}
            if(child is Sprite && child.Name.Contains("ActionCalls")){
                if(child.Name.Contains("1To6")){
                    this.NumActionCallsGraphic1To6 = (Sprite)child;}}
            if(child is TextureProgress){
                this.healthProgressBar = (TextureProgress)child;}
            if((child is Label) && (child.GetName().ToLower().Contains("total"))){
                this.totalCaloriesLabel= (Label)child;
                this.totalCaloriesLabel.Text = (String)Strings.UI["Calories"];}
            if((child is Label) && (child.GetName().ToLower().Contains("accell"))){
                this.accellLabel= (Label)child;}
            if(child is Sprite && child.Name.ToLower().Contains("baseholder")){
                this.baseHolder = (LevelFrameBannerBase)child;}
            if(child is Container){
                this.lastFoodEatenContainer = (Container)child;
                foreach(Node subChild in child.GetChildren()){
                    if(subChild is Node && subChild.Name.ToLower().Contains("labelcontainer")){
                        this.lastFoodEatenDisplayLabel = (Label)subChild.GetChild(0);}
                    if(subChild is Sprite){
                        this.lastFoodEatenDisplaySprite = (Sprite)subChild;}
                    if(subChild is AnimatedSprite && subChild.Name.ToLower().Contains("foodeatenborder")){
                        this.LastFoodEatenBorder = (AnimatedSprite)subChild;}}
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


    public override void ReactStateless(float delta){
        this.healthProgressBar.Value = this.activePlayer.CurrentHealth;
        this.totalCaloriesLabel.Text = Strings.UI["Calories"] + ": " + this.activePlayer.TotalCalories.ToString();
        this.accellLabel.Text = "FPS: " + (1 / delta).ToString("0");
        this.rotateSpeedometer();
        this.handleHealthWarningFlashPrompt(delta);
        this.handleInteractableFlashPrompt(delta);
        this.handleHoldableIcon(delta);
    }
    private void rotateSpeedometer(){
        var lowerBoundDeg = 160;
        var upperBoundDeg = 40;
        var range = upperBoundDeg + 180 + (180 - lowerBoundDeg);
        var arbitraryNormalizer = 0.65f;
        var weight = 0.15f;
        var percFull = Math.Abs(this.activePlayer.ATV.FrontWheel.forwardAccell) / Wheel.MAX_FORWARD_ACCEL;
        var degrees = (lowerBoundDeg + (percFull * range)) * arbitraryNormalizer;
        this.speedometerMeter.RotationDegrees = (this.speedometerMeter.RotationDegrees + weight * degrees) / (weight + 1);
    }

    private void handleHealthWarningFlashPrompt(float delta){
       if(this.activePlayer.IsInHealthDanger){
           this.HealthWarningFlashPrompt.Visible = true;
           this.baseHolder.BlinkRed = true;}
       else {
           this.HealthWarningFlashPrompt.Visible = false;
           this.baseHolder.BlinkRed = false;
           this.baseHolder.ResetToDefaultColor();}}

    private void handleInteractableFlashPrompt(float delta){
        if(this.activePlayer.WholeBodyKinBody.IsOverInteractable){
            this.HealthWarningFlashPrompt.Visible = false;
            if(this.interactableFlashPromptCounter < 100){
                this.interactableFlashPromptCounter = 100;}
            if(this.interactableFlashPromptCounter < 200){
                this.InteractableFlashPrompt.Modulate = new Color(
                    this.InitialInteractableFlashPromptModulate.r, 
                    this.InitialInteractableFlashPromptModulate.g, 
                    this.InitialInteractableFlashPromptModulate.b,
                    this.interactableFlashPromptCounter / 255f);
                this.interactableFlashPromptCounter += 10;}
        } else {
            if(this.interactableFlashPromptCounter > 100){
                this.interactableFlashPromptCounter = 100;
            }
            if(this.interactableFlashPromptCounter < 0){
                this.interactableFlashPromptCounter = 0;
            }
            this.interactableFlashPromptCounter -= 10;
                this.InteractableFlashPrompt.Modulate = new Color(
                    this.InitialInteractableFlashPromptModulate.r, 
                    this.InitialInteractableFlashPromptModulate.g, 
                    this.InitialInteractableFlashPromptModulate.b,
                    this.interactableFlashPromptCounter / 255f);
        }
    }

    private void handleHoldableIcon(float delta){
        this.HoldableIcon.Texture = this.activePlayer.ActiveHoldable.UIDisplayIcon;
        if(this.activePlayer.ActiveHoldable.NumActionCallsToDepleted <= 6){
            this.draw1To6ActionCallLeftGraphic(this.activePlayer.ActiveHoldable.NumActionCallsLeft);
        } else {
            this.draw1To6ActionCallLeftGraphic(0);
        }
    }

    private void draw1To6ActionCallLeftGraphic(int NumActionCallsLeft){
        const int ICON_PIXEL_WIDTH = 130;
        const int ICON_PIXEL_HEIGHT = 100;
        const int MAX_ICON_WIDTH = ICON_PIXEL_WIDTH * 6;
        var offsetWidth = 0f - ((MAX_ICON_WIDTH - (NumActionCallsLeft * ICON_PIXEL_WIDTH)) / 2f); 
        var regionWidth = NumActionCallsLeft * ICON_PIXEL_WIDTH;
        this.NumActionCallsGraphic1To6.Offset = new Vector2(offsetWidth, 0f);
        this.NumActionCallsGraphic1To6.RegionRect = new Rect2(
            new Vector2(0,0), new Vector2(regionWidth, ICON_PIXEL_HEIGHT));
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
                this.LastFoodEatenBorder.Visible = true;
                break;
            case PlayerStatsDisplayHandlerState.SHOW_LAST_FOOD:
                this.lastFoodEatenDisplaySprite.Rotate(0.04f * delta * main.DELTA_NORMALIZER);
                break;
            case PlayerStatsDisplayHandlerState.END_SHOW_LAST_FOOD:
                this.lastFoodEatenDisplayLabel.Visible = false;
                this.lastFoodEatenDisplaySprite.Visible = false;
                this.LastFoodEatenBorder.Visible = false;
                this.SetActiveState(PlayerStatsDisplayHandlerState.DEFAULT, 400);
                break;
            case PlayerStatsDisplayHandlerState.DEFAULT:
                break;
            default:
                throw new Exception("Invalid player handler state");
        }
    }

    private Rect2 GetDefactoRect(Sprite sprite){
        foreach(Node child in sprite.GetChildren()){
            if(child is CollisionShape2D && ((CollisionShape2D)child).Shape is RectangleShape2D && child.Name.ToLower().Contains("boundingbox")){
                return new Rect2(sprite.GetPosition(), ((RectangleShape2D)((CollisionShape2D)child).Shape).Extents);
            }
        }
        return _GetDefactoRect(sprite, sprite.GetRect());
    }

    private Rect2 _GetDefactoRect(Sprite sprite, Rect2 ongoingRect){
        var rect = sprite.GetRect();
        var xmin = rect.Position.x;
        var ymin = rect.Position.y;
        var xmax = xmin + rect.Size.x;
        var ymax = ymin + rect.Size.y;

        var ongoingxmin = ongoingRect.Position.x;
        var ongoingymin = ongoingRect.Position.y;
        var ongoingxmax = ongoingxmin + ongoingRect.Size.x;
        var ongoingymax = ongoingymin + ongoingRect.Size.y;

        if(xmin < ongoingxmin){
            ongoingRect.Position = new Vector2(xmin, ongoingRect.Position.y);
        }
        if(ymin < ongoingymin){
            ongoingRect.Position = new Vector2(ongoingRect.Position.x, ymin);
        }
        if(xmax > ongoingxmax){
            var newSizeX = xmax + (xmax - ongoingxmax); 
            ongoingRect.Size = new Vector2(newSizeX, ongoingRect.Size.y);
        }
        if(ymax > ongoingymax){
            var newSizeY = ymax + (ymax - ongoingymax);
            ongoingRect.Size = new Vector2(ongoingRect.Size.x, newSizeY);
        }

        foreach(Node child in sprite.GetChildren()){
            if(child is Sprite){
                ongoingRect = _GetDefactoRect((Sprite)child, ongoingRect);
            }
        }
        return ongoingRect;
    }

    private void setUpLastFoodSprite(float delta){
        //Get the size of the parent display container, scale sprite to ti
        var targetSize = this.lastFoodEatenContainer.GetRect().Size;
        var foodDisplaySprite = this.activePlayer.lastFoodEaten.FoodDisplaySprite;
        var defactorRect = this.GetDefactoRect(foodDisplaySprite);
        var widthScale = targetSize.x / this.GetDefactoRect(foodDisplaySprite).Size.x;
        var heightScale = targetSize.y / this.GetDefactoRect(foodDisplaySprite).Size.y;
        //For oblong non-square sizes, always pick the smaller scale
        var targetScale = widthScale < heightScale ? widthScale : heightScale;
        this.lastFoodEatenDisplaySprite.Scale = new Vector2(targetScale, targetScale);
        
        foreach(Node child in this.lastFoodEatenDisplaySprite.GetChildren()){
            this.lastFoodEatenDisplaySprite.RemoveChild(child);}
        if(foodDisplaySprite.Texture != null){
            this.lastFoodEatenDisplaySprite.Texture = foodDisplaySprite.Texture;}
        else{
            foodDisplaySprite.GetParent().RemoveChild(foodDisplaySprite);
            this.lastFoodEatenDisplaySprite.Texture = null;
            this.lastFoodEatenDisplaySprite.AddChild(foodDisplaySprite);}

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
