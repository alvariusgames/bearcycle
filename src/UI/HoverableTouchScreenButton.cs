using Godot;
using System;

public class HoverableTouchScreenButton : TouchScreenButton{

//  // Called every frame. 'delta' is the elapsed time since the previous frame.

    private bool wasPreviouslyPressed = false;
    private bool isCurrentlyPressed = false;
    private bool userProgramaticPressOverride = false;
    private Texture _Pressed;
    private Texture _Normal;

    public override void _Ready(){
        this._Pressed = (Texture)this.Pressed.Duplicate();
        this._Normal = (Texture)this.Normal.Duplicate();
    }

    public override void _Process(float delta){
        this.wasPreviouslyPressed = this.isCurrentlyPressed;
        this.isCurrentlyPressed = this.IsPressed();
        if(this.isCurrentlyPressed && this.GetParent().GetParent() is HoverControlBoxContainer){
            var container = (HoverControlBoxContainer)this.GetParent().GetParent();
            container.HoveredItem = this;
        }
    }

    public bool UserHasJustSelected(){
        if(this.userProgramaticPressOverride){
           this.userProgramaticPressOverride = false;
           return true; 
        } else {
            return this.wasPreviouslyPressed && !this.isCurrentlyPressed; }
    }

    public void MimicTouch(){
        this.SetGraphicToPressed();
    }

    public void MimicTouchRelease(){
        this.SetGraphicToUnpressed();
        this.userProgramaticPressOverride = true;
    }

    public void SetGraphicToPressed(){
        this.SetTexture(this._Pressed);
    }

    public Boolean GraphicIsPressed(){
        return this.GetTexture() == this._Pressed;
    }

    public void SetGraphicToUnpressed(){
        this.SetTexture(this._Normal);
    }

    public Boolean GraphicIsUnpressed(){
        return this.GetTexture() == this._Normal;
    }

}
