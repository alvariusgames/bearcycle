using Godot;
using System;

public class PauseMenu : Node2D{
    private Boolean isPaused = false;
    private Boolean IsFreshlyPaused = false;
    private Boolean waitForUserInputToStartHovering = true;
    private HoverableTouchScreenButton HomeButton;
    private const int HOME_I = 0;
    private const int HOME_J = 0;
    private HoverableTouchScreenButton ResumeButton;
    private const int RES_I = 0;
    private const int RES_J = 1;

    public override void _Ready(){
        if(this.GetChild(0) is PlatformSpecificChildren){
            ((PlatformSpecificChildren)this.GetChild(0)).PopulateChildrenWithPlatformSpecificNodes(this);}
        foreach(Node child in this.GetChildren()){
            if(child is Sprite && child.Name.ToLower().Contains("banner")){
                foreach(Node subChild in child.GetChildren()){
                    if(subChild is HoverableTouchScreenButton && subChild.Name.ToLower().Contains("home")){
                        this.HomeButton = (HoverableTouchScreenButton)subChild;
                        this.interactablesGridRefs[HOME_I,HOME_J] = this.HomeButton;}
                    if(subChild is HoverableTouchScreenButton && subChild.Name.ToLower().Contains("resume")){
                        this.ResumeButton = (HoverableTouchScreenButton)subChild;
                        this.interactablesGridRefs[RES_I,RES_J] = this.ResumeButton;}
                }
            }
        }
    }

    private void Pause(){
        this.isPaused = true;
        this.GetTree().Paused = true;}
    
    private void UnPause(){
        this.isPaused = false;
        this.GetTree().Paused = false;}

    public void OpenPauseMenu(){
        this.Pause();
        this.Visible = true;
        this.IsFreshlyPaused = true;
        this.waitForUserInputToStartHovering = true;
        this.HomeButton.SetGraphicToUnpressed();
        this.ResumeButton.SetGraphicToUnpressed();
        SoundHandler.PauseAllStream();
        SoundHandler.StopAllSample();
        SoundHandler.PlayStream<MyAudioStreamPlayer>(this,
            new string[]{"res://media/music/short/pause_music1.ogg"},
            Loop: true);
    }

    public void ClosePauseMenuBackToGame(){
        this.UnPause();
        this.Visible = false;
        SoundHandler.StopStream(this, "res://media/music/short/pause_music1.ogg");
        SoundHandler.UnpauseAllStream();
        SoundHandler.StopAllSample();
    }

    private const int INITIAL_I = 0;
    private const int INTITIAL_J = 1;
    private int i = INITIAL_I;
    private int j = INTITIAL_J;

    private void navigate(int plusI, int plusJ){
        var potI = i + plusI;
        if(potI >= I_LENGTH){
            potI = I_LENGTH - 1;}
        if(potI <= 0){
            potI = 0;}
        var potJ = j + plusJ;
        if(potJ >= J_LENGTH){
            potJ = J_LENGTH - 1;}
        if(potJ <= 0){
            potJ = 0;}
        if(this.interactablesGrid[potI, potJ]){
            this.i = potI;
            this.j = potJ;
        } else {
            if(potJ > 0){
                //Try moving left if there's anything there
                potJ--;}
            if(this.interactablesGrid[potI, potJ]){
                this.i = potI;
                this.j = potJ;}}}

    private const int I_LENGTH = 3;
    private const int J_LENGTH = 2;
    private Boolean[,] interactablesGrid = new Boolean[I_LENGTH, J_LENGTH]
        //Maps what buttons exist in what placement on grid
        {{true, /*to home button*/ true /*resume play button*/},
        {true, /*change sample volume slider*/ false},
        {true, /*change stream volume slider*/ false}};

    private object[,] interactablesGridRefs = new object[I_LENGTH, J_LENGTH]
    {{null, null},
    {null, null},
    {null, null}};

    public override void _Process(float delta){
        if(isPaused){
            this.navigateAndUpdateIandJ(delta);
            this.updateIAndJFromTouch();
            this.checkInputForUnpausing(delta);
            this.updateGraphicsForSelectionInPauseMenu(delta);
            this.checkInputForSelectingPauseMenuItem(delta);
         } else {
             this.i = INITIAL_I;
             this.j = INTITIAL_J;
         }
    }

    private void navigateAndUpdateIandJ(float delta){
        if(this.waitForUserInputToStartHovering){
            if(Input.IsActionJustPressed("ui_down") || 
               Input.IsActionJustPressed("ui_up") ||
               Input.IsActionJustPressed("ui_right") ||
               Input.IsActionJustPressed("ui_left")){
                   this.waitForUserInputToStartHovering = false;}
        } else {
            if(Input.IsActionJustPressed("ui_down")){
                this.navigate(1, 0);}
            if(Input.IsActionJustPressed("ui_up")){
                this.navigate(-1, 0);}
            if(Input.IsActionJustPressed("ui_right")){
                this.navigate(0, 1);}
            if(Input.IsActionJustPressed("ui_left")){
                this.navigate(0, -1);}
            }
    }

    private void updateIAndJFromTouch(){
        if(this.HomeButton.UserHasJustSelected()){
            i =  HOME_I;
            j = HOME_J;}
        if(this.ResumeButton.UserHasJustSelected()){
            i = RES_I;
            j = RES_J;}}

    private void checkInputForUnpausing(float delta){
        if(Input.IsActionJustPressed("ui_pause")){
            if(IsFreshlyPaused){
                //hacky workaround to not immediately unpause after pause
                this.IsFreshlyPaused = false;
            } else {
                this.ClosePauseMenuBackToGame();}}
    }

    private void updateGraphicsForSelectionInPauseMenu(float delta){
        if(!this.waitForUserInputToStartHovering){
            var selectedItem = this.interactablesGridRefs[i,j];
            foreach(var hovTouchButonToCheck in new HoverableTouchScreenButton[]{this.HomeButton, this.ResumeButton})
            if(selectedItem == hovTouchButonToCheck){
                hovTouchButonToCheck.SetGraphicToPressed();}
            else {
                hovTouchButonToCheck.SetGraphicToUnpressed();}
        }
    }

    private void checkInputForSelectingPauseMenuItem(float delta){
            var selectedItem = this.interactablesGridRefs[i,j];
            var isSelected = Input.IsActionJustReleased("ui_accept");
            if(selectedItem is TouchScreenButton){
                isSelected = isSelected || ((HoverableTouchScreenButton)selectedItem).UserHasJustSelected();}
            if((selectedItem == this.HomeButton) && isSelected){
                this.ClosePauseMenuBackToGame();
                SoundHandler.StopAllStream();
                SoundHandler.StopAllSample();
                SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                                    ToSceneStr: "res://scenes/level_select/level_select.tscn",
                                    effect: SceneTransitionEffect.FADE_BLACK,
                                    numSeconds: 2f,
                                    FadeOutAudio: true);
            }
            else if((selectedItem == this.ResumeButton) && isSelected){
                this.ClosePauseMenuBackToGame();
            }
        }

}
