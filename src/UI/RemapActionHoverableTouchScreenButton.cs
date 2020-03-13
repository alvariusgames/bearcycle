using Godot;
using System;

public class RemapActionHoverableTouchScreenButton : HoverableTouchScreenButton{
    private Boolean catchNextInputEventAndRemapAsAction = false;
    private Boolean ignoreNextUserHasSelected = false; //Needed for when mapping to ui_accept
    private float timer = 0f;
    private const float TIME_TO_RESET_TO_DEFAULT_SEC = 10f;
    public PlatformDepButton PlatformDepButton;
    public Label ActionNameLabel;
    public Node2D Popup;

    public override void _Ready(){
        base._Ready();
        foreach(Node child in this.GetChildren()){
            if(child is PlatformDepButton){
                this.PlatformDepButton = (PlatformDepButton)child;
                this.PlatformDepButton.Name = Name;
                this.PlatformDepButton._Ready();}
            if(child is Label){
                this.ActionNameLabel = (Label)child;
                if(this.Name.Equals("ui_attack")){
                    this.ActionNameLabel.Text = "UI_ATTACK_NAME";}
                if(this.Name.Equals("ui_forage")){
                    this.ActionNameLabel.Text = "UI_FORAGE_NAME";}
                if(this.Name.Equals("ui_use_item")){
                    this.ActionNameLabel.Text = "UI_ITEM_NAME";}}
            if(child.Name.ToLower().Contains("popup")){
                this.Popup = (Node2D)child;
                foreach(Node subChild in this.Popup.GetChildren()){
                    if(subChild is Label && subChild.Name.ToLower().Contains("ui_attack_name")){
                        ((Label)subChild).Text = this.ActionNameLabel.Text;}}}}
    }

    public override void MimicTouch(){
        if(!this.ignoreNextUserHasSelected){
            base.MimicTouch();
            this.togglePopup(true);}}

    public override void _Input(InputEvent @event){
        if(this.UserHasJustSelected()){
            if(this.ignoreNextUserHasSelected){
                this.togglePopup(false);
                this.ignoreNextUserHasSelected = false;
                return;}
            this.catchNextInputEventAndRemapAsAction = true;}
        if(this.catchNextInputEventAndRemapAsAction){
            SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                new string[]{"res://media/samples/ui/accept_1.wav"});

            if(@event is InputEventKey){
                DbHandler.OverrideControlAction(this.Name,
                                                Controllers.Keyboard.Id,
                                                ((InputEventKey)@event).Scancode);
                this.catchNextInputEventAndRemapAsAction = false;
                this.PlatformDepButton._Ready();
                this.togglePopup(false);
                if(this.isEventEquivalentTo_ui_accept(@event)){
                    this.ignoreNextUserHasSelected = true;}}
            if(@event is InputEventJoypadButton){
                DbHandler.OverrideControlAction(this.Name,
                                                Controllers.JoyPad.Id,
                                                (uint)((InputEventJoypadButton)@event).ButtonIndex);
                this.catchNextInputEventAndRemapAsAction = false;
                this.PlatformDepButton._Ready();
                this.togglePopup(false);
                if(this.isEventEquivalentTo_ui_accept(@event)){
                    this.ignoreNextUserHasSelected = true;}}
            if(@event is InputEventScreenTouch){
                // User probably wants to get out
                this.togglePopup(false);
                this.catchNextInputEventAndRemapAsAction = false;}}}

    private void togglePopup(Boolean visible){
        if(visible){
            this.Popup.Visible = true;}
            //this.PlatformDepButton.Visible = false;}
        else{
            this.Popup.Visible = false;}}
            //this.PlatformDepButton.Visible = true;}}

    private Boolean isEventEquivalentTo_ui_accept(InputEvent @event){
        foreach(var action in InputMap.GetActionList("ui_accept")){
            if(action is InputEventKey && @event is InputEventKey){
                if(((InputEventKey)action).Scancode == ((InputEventKey)@event).Scancode){
                    return true;}}
            if(action is InputEventJoypadButton && @event is InputEventJoypadButton){
                if(((InputEventJoypadButton)action).ButtonIndex == ((InputEventJoypadButton)@event).ButtonIndex){
                    return true;}}}
        return false;}

}
