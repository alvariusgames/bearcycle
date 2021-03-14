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
        get { return PlayerStatsDisplayHandlerState.DEFAULT;}set{}}
    public Player activePlayer;
    public LevelFrameBannerBase LevelFrameBannerBase;
    [Export]
    public NodePath LevelFrameBannerBasePath {get; set;}
    private Sprite speedometerMeter;
    [Export]
    public NodePath speedometerMeterPath {get; set;}
    private TextureProgress healthProgressBar;
    [Export]
    public NodePath healthProgressBarPath {get; set;}
    public const string HEALTH_BAR_RED_HEX = "ff3d38";
    public const string HEALTH_BAR_GREEN_HEX = "308122";
    public const string HEALTH_BAR_ORANGE_HEX = "ffb940";
    [Export]
    public NodePath gainHealthParticlesPath {get; set;}
    private Particles2D gainHealthParticles {get; set;}
    private TextureProgress strengthProgressBar;
    [Export]
    public NodePath strengthProgressBarPath {get; set;}
    private TextureProgress holdableProgressBar;
    [Export]
    public NodePath holdableProgressBarPath {get; set;}
    public Label totalCaloriesLabel;
    [Export]
    public NodePath totalCaloriesLabelPath {get; set;}
    private Label fpsLabel;
    [Export]
    public NodePath fpsLabelPath {get; set;}
    private Label livesLabel;
    [Export]
    public NodePath livesLabelPath {get; set;}
    private IFood lastFoodDisplayed;
    private Label lastFoodEatenDisplayLabel;
    [Export]
    public NodePath lastFoodEatenDisplayLabelPath {get; set;}
    public Container lastFoodEatenContainer;
    [Export]
    public NodePath lastFoodEatenContainerPath {get; set;}
    private Sprite lastFoodEatenDisplaySprite;
    [Export]
    public NodePath lastFoodEatenDisplaySpritePath {get; set;}
    private AnimatedSprite LastFoodEatenBorder;
    [Export]
    public NodePath LastFoodEatenBorderPath {get; set;}
    private int interactableFlashPromptCounter = 0;
    private Sprite HoldableIcon;
    [Export]
    public NodePath HoldableIconPath {get; set;}
    private TextureProgress NumActionCallsGraphic1To6;
    [Export]
    public NodePath NumActionCallsGraphic1To6Path {get; set;}
    private Node2D TouchScreenButtons;
    [Export]
    public NodePath TouchScreenButtonsPath {get; set;}
    private TouchScreenButton UiUseItemTouchScreenButton;
    [Export]
    public NodePath UiUseItemTouchScreenButtonPath {get; set;}
    [Export]
    public NodePath RightOfHudLocationPath {get; set;}
    public Node2D RightOfHudLocation;

    public override void _Ready(){
        this.ResetActiveState(this.InitialState);
        this.setPlayerAndCameraMembers();
        try{
        this.speedometerMeter = this.GetNode<Sprite>(this.speedometerMeterPath);
        this.HoldableIcon = this.GetNode<Sprite>(this.HoldableIconPath);
        this.NumActionCallsGraphic1To6 = this.GetNode<TextureProgress>(this.NumActionCallsGraphic1To6Path);
        this.healthProgressBar = this.GetNode<TextureProgress>(this.healthProgressBarPath);
        this.gainHealthParticles = this.GetNode<Particles2D>(this.gainHealthParticlesPath);
        this.strengthProgressBar = this.GetNode<TextureProgress>(this.strengthProgressBarPath);
        this.holdableProgressBar = this.GetNode<TextureProgress>(this.holdableProgressBarPath);
        this.totalCaloriesLabel = this.GetNode<Label>(this.totalCaloriesLabelPath);
        this.fpsLabel = this.GetNode<Label>(this.fpsLabelPath);
        this.livesLabel = this.GetNode<Label>(this.livesLabelPath);
        this.LevelFrameBannerBase = this.GetNode<LevelFrameBannerBase>(this.LevelFrameBannerBasePath);
        this.lastFoodEatenContainer = this.GetNode<Container>(this.lastFoodEatenContainerPath);
        this.lastFoodEatenDisplayLabel = this.GetNode<Label>(this.lastFoodEatenDisplayLabelPath);
        this.lastFoodEatenDisplaySprite = this.GetNode<Sprite>(this.lastFoodEatenDisplaySpritePath);
        this.LastFoodEatenBorder = this.GetNode<AnimatedSprite>(this.LastFoodEatenBorderPath);
        this.RightOfHudLocation = this.GetNode<Node2D>(this.RightOfHudLocationPath);
        if(this.TouchScreenButtonsPath != null){
            this.TouchScreenButtons = this.GetNode<Node2D>(this.TouchScreenButtonsPath);}
        if(this.UiUseItemTouchScreenButtonPath != null){
            this.UiUseItemTouchScreenButton = this.GetNode<TouchScreenButton>(this.UiUseItemTouchScreenButtonPath);}
        this.makeTransparentOnMobile();} catch{}
    }


    private void makeTransparentOnMobile(){
        if(main.PlatformType.Equals(PlatformType.MOBILE)){
            foreach(CanvasItem child in this.GetChildren()){
                if(child != this.lastFoodEatenContainer){
                    child.Modulate = new Color(child.Modulate.r, child.Modulate.g, child.Modulate.b, 0.75f);
                    if(child is LevelFrameBannerBase){
                        ((LevelFrameBannerBase)child).InitialModulate = child.Modulate;}}}}}

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

    public override void _Process(float delta)
    {
    }

    public override void _PhysicsProcess(float delta)
    {
    }

    public override void ReactStateless(float delta){
        var minStrengthToDisplay = 0f * Player.MAX_STRENGTH;
        this.strengthProgressBar.Value = Math.Max(
            this.activePlayer.Strength,
            minStrengthToDisplay);
        this.healthProgressBar.Value = this.activePlayer.Health;

        this.totalCaloriesLabel.Text = this.Tr("UI_CALORIES") + ": " + this.activePlayer.TotalCalories.ToString();
        if(main.IsDebug){
            this.fpsLabel.Visible = true;
            this.fpsLabel.Text = "FPS: " + (1 / delta).ToString("0");}
        this.livesLabel.Text = "x " + this.activePlayer.NumLives;
        this.rotateSpeedometer();
        this.handleHealthWarningFlashPrompt(delta);
        this.handleHoldableIcon(delta);
        this.handleStrengthModulateAndParticles(delta);
        if(this.TouchScreenButtons != null){
            if(this.activePlayer.ActiveHoldable == null){
                this.UiUseItemTouchScreenButton.Visible = false;}
            else{
                this.UiUseItemTouchScreenButton.Visible = true;}}
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
    private float HitSeqRedTimerSec = 0f;
    private void handleHealthWarningFlashPrompt(float delta){
       if(this.activePlayer.IsInHealthDanger){
           this.LevelFrameBannerBase.BlinkRed = true;}
       else if(this.activePlayer.ATV.Bear.HitSeqTriggeredThisFrame ||
               this.HitSeqRedTimerSec > 0f && this.HitSeqRedTimerSec < DamageShakeHandler.NUM_SEC_TO_SHAKE){
           this.LevelFrameBannerBase.ModulateToRedNow();
           this.HitSeqRedTimerSec += delta;
       }
       else {
           this.LevelFrameBannerBase.BlinkRed = false;
           this.LevelFrameBannerBase.ResetToDefaultColor();
           this.HitSeqRedTimerSec = 0f;}}

    private float holdableIconFlashTimer = -10f;

    private void handleHoldableIcon(float delta){
        this.HoldableIcon.Texture = this.activePlayer.HoldableOrClawAttackUIDisplayTexture;
        var numActionCallsToDraw = 0;
        foreach(var holdable in this.activePlayer.AllHoldibles){
            numActionCallsToDraw += holdable.NumActionCallsLeft;}
        this.NumActionCallsGraphic1To6.Visible = false;
        this.holdableProgressBar.Visible = false;
        if(this.activePlayer.ActiveHoldable != null && this.activePlayer.ActiveHoldable.DisplayInProgressBar){
            this.drawActionCallsInProgressBar((float)numActionCallsToDraw / (float)this.activePlayer.ActiveHoldable.NumActionCallsToDepleted);}
        else if(numActionCallsToDraw != 0){
            this.draw1To6ActionCall(numActionCallsToDraw);}
        
        var holdableIconUnder = (Node2D)this.HoldableIcon.GetChild(0);
        if(this.activePlayer.JustPickedUpHoldable){
            this.holdableIconFlashTimer = (float)Math.PI;}
        if(this.holdableIconFlashTimer > -5f){
            this.holdableIconFlashTimer -= delta;

            var x = 0.5f + 0.5f * (float)Math.Sin(this.holdableIconFlashTimer * 7.5f);
            holdableIconUnder.Modulate = new Color(1f,1f,1f,x);
            if(this.holdableIconFlashTimer < 0f){
                this.holdableIconFlashTimer = -10f;}
        } 

        if(this.activePlayer.ActiveHoldable == null){
           holdableIconUnder.Modulate = new Color(1f,1f,1f,0f);}
        else if(this.holdableIconFlashTimer < 0f){
            holdableIconUnder.Modulate = new Color(1f,1f,1f,0.5f);}
    }

    private void drawActionCallsInProgressBar(float percentFull){
        this.holdableProgressBar.Visible = true;
        this.holdableProgressBar.Value = percentFull * this.holdableProgressBar.MaxValue;
    }

    private void draw1To6ActionCall(int NumActionCallsLeft){
        this.NumActionCallsGraphic1To6.Visible = true;
        this.NumActionCallsGraphic1To6.Value = NumActionCallsLeft;
    }
    private float strengthBarModulateCounter = 0.2666667f;
    private void handleStrengthModulateAndParticles(float delta){
        var strength = this.activePlayer.Strength;

        var displayHealth = this.activePlayer.Health;
        var increaseStrengthBoundary = Player.MAX_STRENGTH * Player.STRENGTH_INCREASE_BOUNDARY;
        var decreaseStrengthBoundary = Player.MAX_STRENGTH * Player.STRENGTH_DECREASE_BOUNDARY;
        var lowerHealthBoundary = 0.2f * Player.MAX_HEALTH;
        var midHealthBoundary = 0.5f * Player.MAX_HEALTH;
        var upperHealthBoundary = 0.8f * Player.MAX_HEALTH;
        var green = new Color(HEALTH_BAR_GREEN_HEX);
        var red = new Color(HEALTH_BAR_RED_HEX);
        var orange = new Color(HEALTH_BAR_ORANGE_HEX);
       
        //flash strength meter if in decreasing strength mode
        var sMod = this.strengthProgressBar.Modulate;
        if((strength < decreaseStrengthBoundary) && (displayHealth>0f)){
            this.strengthBarModulateCounter += delta;
            var sinCounter =  (0.75f + ((float)Math.Sin(this.strengthBarModulateCounter * 6f) * 0.25f));
            this.strengthProgressBar.Modulate = new Color(
                sMod.r,sMod.g, sMod.b,
                sinCounter);
        } else {
            this.strengthProgressBar.Modulate = new Color(
                sMod.r, sMod.g, sMod.g,
                1f);
            this.strengthBarModulateCounter = 0.2666667f; //Makes for smooth transitions
        }

        //Modulate the health bar red for low, orange for mid, green for high
        if(displayHealth < midHealthBoundary){
            var mix = (displayHealth - lowerHealthBoundary) / (midHealthBoundary - lowerHealthBoundary);
            mix = Math.Max(0, mix);

            this.healthProgressBar.Modulate = this.mixTwoColors(
                orange,
                red,
                mix);
        } else {
            var mix = (displayHealth - midHealthBoundary) / (upperHealthBoundary - midHealthBoundary);
            mix = Math.Min(1, mix);
            this.healthProgressBar.Modulate = this.mixTwoColors(
                green,
                orange,
                mix
            );
        }

         //handle the strength transfer particle animations       
        if((strength > increaseStrengthBoundary) && (displayHealth < Player.MAX_HEALTH)){
            this.gainHealthParticles.Emitting = true;
            var partMaterial = (ParticlesMaterial)this.gainHealthParticles.ProcessMaterial;
            var gradTexture = (GradientTexture)partMaterial.ColorRamp;
            gradTexture.Gradient.SetColor(1, this.healthProgressBar.Modulate);
        }else{
            this.gainHealthParticles.Emitting = false;}
    }


    public override void UpdateState(float delta){
        if(false && this.activePlayer.lastFoodEaten != this.lastFoodDisplayed){
            //disabled triggering showing of last food from user feedback
            //TODO: remove me
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

    public static Rect2 GetDefactoRect(Sprite sprite){
        foreach(Node child in sprite.GetChildren()){
            if(child is CollisionShape2D && ((CollisionShape2D)child).Shape is RectangleShape2D && child.Name.ToLower().Contains("boundingbox")){
                return new Rect2(sprite.GetPosition(), ((RectangleShape2D)((CollisionShape2D)child).Shape).Extents);
            }
        }
        return _GetDefactoRect(sprite, sprite.GetRect());
    }

    private static Rect2 _GetDefactoRect(Sprite sprite, Rect2 ongoingRect){
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
        var defactorRect = PlayerStatsDisplayHandler.GetDefactoRect(foodDisplaySprite);
        var widthScale = targetSize.x / PlayerStatsDisplayHandler.GetDefactoRect(foodDisplaySprite).Size.x;
        var heightScale = targetSize.y / PlayerStatsDisplayHandler.GetDefactoRect(foodDisplaySprite).Size.y;
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
