using Godot;
using System;

public class ScrollableOptionButton : OptionButton{
    private PopupMenu popup;
    private Boolean wasJustDragging = false;
    public Boolean PopupIsVisible { get { 
        return this.popup.Visible;}}

    public override void _Ready(){
        this.popup = this.GetPopup();
        this.popup.HideOnItemSelection = false;
        this.popup.HideOnCheckableItemSelection = false;
        this.popup.Connect("gui_input", this, nameof(this.OnPopupGuiInput));
    
        //NOTE: it is assumed IDs are numerical in order, which isn't always the case
    }

    public void  OnPopupGuiInput(InputEvent @event){
        this.popup.HideOnItemSelection = false;
        this.popup.HideOnCheckableItemSelection = false;
        if(@event is InputEventScreenDrag){
            var dragEvent = (InputEventScreenDrag)@event;
            this.popup.RectPosition += new Vector2(0, dragEvent.Relative.y);
            //If we're dragging, set this flag to not release popup until we're done
            this.wasJustDragging = true;}
        if(@event is InputEventMouseButton){
            if(!this.wasJustDragging){
                //If we weren't just dragging, release the popup for normal input
                this.popup.HideOnItemSelection = true;
                this.popup.HideOnCheckableItemSelection = true;}
            this.wasJustDragging = false;}
    }

    private Boolean runOnce = true;

    public override void _Process(float delta){
        if(main.PlatformType.Equals(PlatformType.MOBILE)){
            return;}
        float step = this.popup.RectSize.y / this.GetItemCount();
        if(this.PopupIsVisible && this.runOnce){
            this.runOnce = false;
            var selectedElYFromHalf = ((this.GetItemCount() / 2) - this.GetSelectedId()) * step;
            this.popup.RectPosition = new Vector2(this.popup.RectPosition.x, selectedElYFromHalf);
        }
        if(this.Visible == true){
            if(Input.IsActionJustPressed("ui_up") && this.GetSelectedId() > 0){
                if(this.GetIndex() + 1 <= this.GetItemCount())
                this.popup.RectPosition += new Vector2(0, step);
            }
            if(Input.IsActionJustPressed("ui_down") && this.GetSelectedId() < this.GetItemCount()-1){
                this.popup.RectPosition += new Vector2(0, -step);

            }
        }}
}
