using Godot;
using System;

public class SlotArea : Node2D{
    [Export]
    public int SlotNum = 1;
    private enum SelectedAction{PLAY_SLOT, DELETE_SLOT}
    private SelectedAction selectedAction = SelectedAction.PLAY_SLOT;
    public HoverableTouchScreenButton PlaySlotButton;
    public HoverableTouchScreenButton DeleteSlotButton;
    private Boolean isHovered = false;
    private Label NewLabel;
    private Container HighestLevelUnlockedContainer;
    private Sprite HighestLevelUnlockedSprite;
    private Node2D StatsArea;
    private Label NumCalories;
    private Label NumLives;
    private Label NumSpaceRocks;
    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child.Name.ToLower().Contains("playslot")){
                this.PlaySlotButton = (HoverableTouchScreenButton)child;}
            if(child.Name.ToLower().Contains("deleteslot")){
                this.DeleteSlotButton = (HoverableTouchScreenButton)child;}
            if(child.Name.ToLower().Contains("highestlevel")){
                this.NewLabel = (Label)child.GetChild(0);
                this.HighestLevelUnlockedContainer = (Container)child.GetChild(1);
                this.HighestLevelUnlockedSprite = (Sprite)this.HighestLevelUnlockedContainer.GetChild(0);}
            if(child.Name.ToLower().Contains("statsarea")){
                this.StatsArea = (Node2D)child;
                this.NumCalories = (Label)child.GetChild(0);
                this.NumLives = (Label)child.GetChild(1);
                this.NumSpaceRocks = (Label)child.GetChild(2);}}
        this.populateWithSlotInfo();
        }

    private void populateWithSlotInfo(){
        if(DbHandler.DoesSlotExist(this.SlotNum)){
            var slotSnapshot = DbHandler.ReadSnapshotOfSlot(this.SlotNum);
            var sprite = this.getSpriteForHighestLevel(slotSnapshot);
            this.DrawHighestLevelUnlockedSprite(sprite);
            this.populateLabelsWithText(slotSnapshot);
            this.NewLabel.Visible = false;
            this.HighestLevelUnlockedSprite.Visible = true;
            this.StatsArea.Visible = true;
            this.DeleteSlotButton.Visible = true;}
        else{
            this.NewLabel.Visible = true;
            this.HighestLevelUnlockedSprite.Visible = false;
            this.StatsArea.Visible = false;
            this.DeleteSlotButton.Visible = false;
        }
    }

    private Sprite getSpriteForHighestLevel(SlotSnapshot slotSnapshot){
        var highestLevel = slotSnapshot.Slot.HighestLevelNumUnlocked;
        var sprite = new Sprite();
        switch(highestLevel){
            case 1:
                sprite.Texture = (Texture)GD.Load("res://media/UI/level_select/icons/app_forest1.png");
                break;
            case 2:
                sprite.Texture = (Texture)GD.Load("res://media/UI/level_select/icons/suburbia_standin.png");
                break;
            default:
                sprite.Texture = (Texture)GD.Load("res://media/UI/level_select/icons/tutorial.png");
                break;}
        return sprite;
    }

    private void DrawHighestLevelUnlockedSprite(Sprite levelSprite){
        //Get the size of the parent display container, scale sprite to ti
        var targetSize = this.HighestLevelUnlockedContainer.GetRect().Size;
        var defactorRect = PlayerStatsDisplayHandler.GetDefactoRect(levelSprite);
        var widthScale = targetSize.x / PlayerStatsDisplayHandler.GetDefactoRect(levelSprite).Size.x;
        var heightScale = targetSize.y / PlayerStatsDisplayHandler.GetDefactoRect(levelSprite).Size.y;
        //For oblong non-square sizes, always pick the smaller scale
        var targetScale = widthScale < heightScale ? widthScale : heightScale;
        this.HighestLevelUnlockedSprite.Scale = new Vector2(targetScale, targetScale);
        
        foreach(Node child in this.HighestLevelUnlockedSprite.GetChildren()){
            this.HighestLevelUnlockedSprite.RemoveChild(child);}
        if(levelSprite.Texture != null){
            this.HighestLevelUnlockedSprite.Texture = levelSprite.Texture;}
        else{
            levelSprite.GetParent().RemoveChild(levelSprite);
            this.HighestLevelUnlockedSprite.Texture = null;
            this.HighestLevelUnlockedSprite.AddChild(levelSprite);}

        this.HighestLevelUnlockedSprite.Visible = true;
        //Center the newly scaled sprite in the center of the container
        var spritePos = new Vector2(
            this.HighestLevelUnlockedContainer.RectPosition.x + targetSize.x / 2f,
            this.HighestLevelUnlockedContainer.RectPosition.y + targetSize.y / 2f);
        this.HighestLevelUnlockedSprite.Position = spritePos;}

    private void populateLabelsWithText(SlotSnapshot slotSnapshot){
        var totalNumCalories = 0;
        var totalSpaceRocks = 0;
        foreach(var levelStatsRecord in slotSnapshot.LevelStatsRecords){
            totalNumCalories += levelStatsRecord.TotalCalories;
            if(levelStatsRecord.SpaceRock1Collected){
                totalSpaceRocks++;}
            if(levelStatsRecord.SpaceRock2Collected){
                totalSpaceRocks++;}
            if(levelStatsRecord.SpaceRock3Collected){
                totalSpaceRocks++;}}
        this.NumCalories.Text = totalNumCalories.ToString() + " kcal";
        this.NumSpaceRocks.Text = totalSpaceRocks.ToString() + " / " + SpaceRock.NUM_SPACE_ROCKS_IN_WHOLE_GAME.ToString();
        this.NumLives.Text = "x " + slotSnapshot.Slot.NumLives.ToString();
    }

    public void MimicTouch(){
        switch(this.selectedAction){
            case SelectedAction.PLAY_SLOT:
                this.PlaySlotButton.MimicTouch();
                break;
            case SelectedAction.DELETE_SLOT:
                this.DeleteSlotButton.MimicTouch();
                break;}}

    public void MimicTouchRelease(){
        switch(this.selectedAction){
            case SelectedAction.PLAY_SLOT:
                this.PlaySlotButton.MimicTouchRelease();
                break;
            case SelectedAction.DELETE_SLOT:
                this.DeleteSlotButton.MimicTouchRelease();
                break;
        }
 
    }

    public void SetToHovered(){
        this.isHovered = true;
    }

    public void SetToUnhovered(){
        this.selectedAction = SelectedAction.PLAY_SLOT;
        this.DeleteSlotButton.SetGraphicToUnpressed();
        this.PlaySlotButton.SetGraphicToUnpressed();
        this.isHovered = false;
    }

    public override void _Process(float delta){
        //Update graphics so its intuitive
        var playIsPressed = this.PlaySlotButton.IsPressed();
        var deleteIsPressed = this.DeleteSlotButton.IsPressed();
        if((playIsPressed || deleteIsPressed) && this.GetParent().GetParent() is HoverControlBoxContainer){
            var container = (HoverControlBoxContainer)this.GetParent().GetParent();
            container.HoveredItem = this;}

        //Update selected action
        //Mouse/Touch section
        if(deleteIsPressed){
            this.isHovered = true;
            this.selectedAction = SelectedAction.DELETE_SLOT;}
        else if(playIsPressed){
            this.isHovered = true;
            this.selectedAction = SelectedAction.PLAY_SLOT;}
        //Keyboard section
        if(!this.isHovered){
            return;}
        if(this.selectedAction.Equals(SelectedAction.PLAY_SLOT) && Input.IsActionJustReleased("ui_right")){
            SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/click_1.wav");
            this.selectedAction = SelectedAction.DELETE_SLOT;}
        else if(this.selectedAction.Equals(SelectedAction.DELETE_SLOT) && Input.IsActionJustReleased("ui_left")){
            SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/click_1.wav");
            this.selectedAction = SelectedAction.PLAY_SLOT;}

        //Make sure correct state is set if delete is disabled (new game)
        if(this.DeleteSlotButton.Visible == false){
            this.selectedAction = SelectedAction.PLAY_SLOT;}

        //Act upon selected action
        switch(this.selectedAction){
            case SelectedAction.PLAY_SLOT:
                this.PlaySlotButton.SetGraphicToPressed();
                this.DeleteSlotButton.SetGraphicToUnpressed();
               break;
            case SelectedAction.DELETE_SLOT:
                this.PlaySlotButton.SetGraphicToUnpressed();
                this.DeleteSlotButton.SetGraphicToPressed();
                break;
        }
    }
}
