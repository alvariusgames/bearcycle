using Godot;
using System;


public class BossFightLevelFrameHandler : Node2D {
    public LevelFrame LevelFrame;
    public PlayerStatsDisplayHandler PlayerStatsHandler;
    public Sprite BaseHolder;
    public Sprite Avatar;
    public BossFightManager ActiveBossFightManager { get {
        if(!(this.LevelFrame is null) && 
           !(this.LevelFrame.Level is null) && 
           !(this.LevelFrame.Level.BossFightManager is null)){
               return this.LevelFrame.Level.BossFightManager;
        } else {
            return null;
        }
    }}

    public TextureProgress HealthProgressBar;
    public Label DiscreteUnitCounter;

    public override void _Ready(){
        this.Visible = false;
        if(this.GetChild(0) is PlatformSpecificChildren){
            ((PlatformSpecificChildren)this.GetChild(0)).PopulateChildrenWithPlatformSpecificNodes(this);}
        this.LevelFrame = (LevelFrame)this.GetParent();
        this.PlayerStatsHandler = this.LevelFrame.PlayerStatsDisplayHandler;
        foreach(Node child in this.GetChildren()){
            if(child is TextureProgress){
                this.HealthProgressBar = (TextureProgress)child;
                this.HealthProgressBar.MinValue = 0;}
            if(child is Label){
                this.DiscreteUnitCounter = (Label)child;}
            if(child is Sprite && child.Name.ToLower().Contains("base")){
                this.BaseHolder = (Sprite)child;}
            if(child is Sprite && child.Name.ToLower().Contains("avatar")){
                this.Avatar = (Sprite)child;
            }

        }
        if(main.PlatformType.Equals(PlatformType.MOBILE)){
            foreach(CanvasItem item in new CanvasItem[]{this.HealthProgressBar, 
                                                        this.BaseHolder,
                                                        this.Avatar}){
                item.Modulate = new Color(item.Modulate.r, item.Modulate.g, item.Modulate.b, 0.75f);}}
    }

    private void modulateHealthBar(){
        var lowerHealthBoundary = 0.2f * (float)this.HealthProgressBar.MaxValue;
        var midHealthBoundary = 0.5f * (float)this.HealthProgressBar.MaxValue;
        var upperHealthBoundary = 0.8f * (float)this.HealthProgressBar.MaxValue;
        var green = new Color(PlayerStatsDisplayHandler.HEALTH_BAR_GREEN_HEX);
        var red = new Color(PlayerStatsDisplayHandler.HEALTH_BAR_RED_HEX);
        var orange = new Color(PlayerStatsDisplayHandler.HEALTH_BAR_ORANGE_HEX);

        var displayHealth = (float)this.HealthProgressBar.Value;

        if(displayHealth < midHealthBoundary){
            var mix = (displayHealth - lowerHealthBoundary) / (midHealthBoundary - lowerHealthBoundary);
            mix = Math.Max(0, mix);

            this.HealthProgressBar.Modulate = this.mixTwoColors(
                orange,
                red,
                mix);
        } else {
            var mix = (displayHealth - midHealthBoundary) / (upperHealthBoundary - midHealthBoundary);
            mix = Math.Min(1, mix);
            this.HealthProgressBar.Modulate = this.mixTwoColors(
                green,
                orange,
                mix
            );
        }
    }

    public override void _Process(float delta){
        if(this.ActiveBossFightManager is null){
            return;}
        switch(this.ActiveBossFightManager.ActiveState){
            case BossFightManagerState.FIGHT:
                if(this.Avatar.Texture is null){
                    this.Avatar.Texture = this.ActiveBossFightManager.BossFightUIAvatar;}
                this.Visible = true;
                this.PlayerStatsHandler.totalCaloriesLabel.Visible = false;
                if(main.PlatformType.Equals(PlatformType.MOBILE)){
                    this.PlayerStatsHandler.lastFoodEatenContainer.Visible = false;}
                this.HealthProgressBar.MaxValue = 
                    this.ActiveBossFightManager.NumFightUnitsTotal;
                this.HealthProgressBar.Value = 
                    this.ActiveBossFightManager.NumFightUnitsTotal -
                    this.ActiveBossFightManager.NumFightUnitsCompleted;
                this.modulateHealthBar();
                if(this.ActiveBossFightManager.DisplayDiscreteFightUnits){
                    this.DiscreteUnitCounter.Text = String.Format(
                        "{0} / {1}", 
                        this.ActiveBossFightManager.NumFightUnitsCompleted,
                        this.ActiveBossFightManager.NumFightUnitsTotal);
                } else {
                    this.DiscreteUnitCounter.Visible = false;
                }
                break;
            case BossFightManagerState.NOT_ACTIVE:
            case BossFightManagerState.END_FIGHT:
            case BossFightManagerState.DEFEATED:
                this.Visible = false;
                this.PlayerStatsHandler.totalCaloriesLabel.Visible = true;
                if(main.PlatformType.Equals(PlatformType.MOBILE)){
                    this.PlayerStatsHandler.lastFoodEatenContainer.Visible = true;}
                break;
        }
    }

}