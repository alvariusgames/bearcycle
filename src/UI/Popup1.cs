using Godot;
using System;

public class Popup1 : Sprite{
    private enum ActionState{YES, NO}
    private ActionState actionState;
    public Label Message;
    public HoverableTouchScreenButton YesButton;
    public HoverableTouchScreenButton NoButton;

    [Export]
    public Boolean DefaultValueYes{get; set;} = true;

    public override void _Ready(){
        this.ResetActionStateToDefault();
        foreach(Node child in this.GetChildren()){
            if(child is Label && child.Name.ToLower().Contains("message")){
                this.Message = (Label)child;}
            if(child is HoverableTouchScreenButton && child.Name.ToLower().Contains("yes")){
                this.YesButton = (HoverableTouchScreenButton)child;}
            if(child is HoverableTouchScreenButton && child.Name.ToLower().Contains("no")){
                this.NoButton = (HoverableTouchScreenButton)child;}}}


    private void ResetActionStateToDefault(){
        if(DefaultValueYes){
            this.actionState = ActionState.YES;}
        else{
            this.actionState = ActionState.NO;}
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta){
        if(!this.Visible){
            this.ResetActionStateToDefault();
            return;}
        if(Input.IsActionJustReleased("ui_left") || Input.IsActionJustReleased("ui_right")){
            SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/click_1.wav");}
        if(Input.IsActionJustReleased("ui_left") || this.YesButton.IsPressed()){
            this.actionState = ActionState.YES;}
        if(Input.IsActionJustReleased("ui_right") || this.NoButton.IsPressed()){
            this.actionState = ActionState.NO;}
        switch(this.actionState){
            case ActionState.YES:
                this.YesButton.SetGraphicToPressed();
                this.NoButton.SetGraphicToUnpressed();
                if(Input.IsActionJustReleased("ui_accept")){
                    this.YesButton.MimicTouchRelease();}
                break;
            case ActionState.NO:
                this.YesButton.SetGraphicToUnpressed();
                this.NoButton.SetGraphicToPressed();
                if(Input.IsActionJustReleased("ui_accept")){
                    this.NoButton.MimicTouchRelease();}
                break;
        }
  }
}
