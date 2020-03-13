using Godot;
using System;

public class PlatformDepButton : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public PlatformSpecificChildren PlatformSpecificChildren;
    public Node2D KeyIcon;
    public Label KeyLabel;
    public Node2D JoyPadIcon;
    public Label JoypadLabel;
    public Sprite touchScreenIcon;

    // Called when the node enters the scene tree for the first time.

    public override void _Input(InputEvent @event){
        if(this.KeyIcon == null || this.JoyPadIcon == null){
            return;}
        if(@event is InputEventJoypadButton){
            this.JoyPadIcon.Visible = true;
            this.KeyIcon.Visible = false;
            if(this.touchScreenIcon != null){
                this.touchScreenIcon.Visible = false;}}
        if(@event is InputEventKey){
            this.JoyPadIcon.Visible = false;
            this.KeyIcon.Visible = true;
            if(this.touchScreenIcon != null){
                this.touchScreenIcon.Visible = false;}}
        if(@event is InputEventScreenTouch && this.touchScreenIcon != null){
            this.JoyPadIcon.Visible = false;
            this.KeyIcon.Visible = false;
            if(this.touchScreenIcon != null){
                this.touchScreenIcon.Visible = true;}}}

    public override void _Ready(){
        if(this.GetChild(0) is PlatformSpecificChildren){
            this.PlatformSpecificChildren = (PlatformSpecificChildren)this.GetChild(0);
            this.PlatformSpecificChildren.PopulateChildrenWithPlatformSpecificNodes(this);}
        foreach(Node child in this.GetChildren()){
            if(child is Sprite){
                this.touchScreenIcon = (Sprite)child;
                if(this.Name.Equals("ui_attack")){
                    this.touchScreenIcon.Texture = (Texture)GD.Load("res://media/UI/touch_screen_buttons/attack_unclick.png");}
                if(this.Name.Equals("ui_forage")){
                    this.touchScreenIcon.Texture = (Texture)GD.Load("res://media/UI/touch_screen_buttons/forage_unclick.png");}
                if(this.Name.Equals("ui_use_item")){
                    this.touchScreenIcon.Texture = (Texture)GD.Load("res://media/UI/touch_screen_buttons/use_item_unclick.png");}}
            if(child.Name.ToLower().Contains("key")){
                this.KeyIcon = (Node2D)child;
                this.KeyLabel = (Label)this.KeyIcon.GetChild(0);
                if(this.Name.Equals("ui_attack")){
                    var index = Controllers.Keyboard.DefaultAttackIndex;
                    if(DbHandler.Globals.AttackControlOverrideController.Equals(Controllers.Keyboard.Id)){
                        index = DbHandler.Globals.AttackControlOverrideIndex;}
                    this.KeyLabel.Text = Controllers.Keyboard.GetNameFor(index);
                    if(this.KeyLabel.Text == "Control"){
                        this.KeyLabel.Text = "Ctrl";}
                    if(this.KeyLabel.Text == "Space"){
                        ((Node2D)this.KeyIcon.GetChild(1)).Scale = new Vector2(1, 0.5f);}
                    else{
                         ((Node2D)this.KeyIcon.GetChild(1)).Scale = new Vector2(0.5f, 0.5f);}}
                if(this.Name.Equals("ui_forage")){
                    var index = Controllers.Keyboard.DefaultForageIndex;
                    if(DbHandler.Globals.ForageControlOverrideController.Equals(Controllers.Keyboard.Id)){
                        index = DbHandler.Globals.ForageControlOverrideIndex;}
                    this.KeyLabel.Text = Controllers.Keyboard.GetNameFor(index);
                    if(this.KeyLabel.Text == "Control"){
                        this.KeyLabel.Text = "Ctrl";}
                    if(this.KeyLabel.Text == "Space"){
                        ((Node2D)this.KeyIcon.GetChild(1)).Scale = new Vector2(1, 0.5f);}
                    else{
                         ((Node2D)this.KeyIcon.GetChild(1)).Scale = new Vector2(0.5f, 0.5f);}}
                if(this.Name.Equals("ui_use_item")){
                    var index = Controllers.Keyboard.DefaultUseItemIndex;
                    if(DbHandler.Globals.UseItemControlOverrideController.Equals(Controllers.Keyboard.Id)){
                        index = DbHandler.Globals.UseItemControlOverrideIndex;}
                    this.KeyLabel.Text = Controllers.Keyboard.GetNameFor(index);
                    if(this.KeyLabel.Text == "Control"){
                        this.KeyLabel.Text = "Ctrl";}
                    if(this.KeyLabel.Text == "Space"){
                        ((Node2D)this.KeyIcon.GetChild(1)).Scale = new Vector2(1, 0.5f);}
                    else{
                         ((Node2D)this.KeyIcon.GetChild(1)).Scale = new Vector2(0.5f, 0.5f);}}}
            if(child.Name.ToLower().Contains("joypad")){
                this.JoyPadIcon = (Node2D)child;
                this.JoypadLabel = (Label)this.JoyPadIcon.GetChild(0);
                if(this.Name.Equals("ui_attack")){
                    var index = Controllers.JoyPad.DefaultAttackIndex;
                    if(DbHandler.Globals.AttackControlOverrideController.Equals(Controllers.JoyPad.Id)){
                        index = DbHandler.Globals.AttackControlOverrideIndex;}
                    this.JoypadLabel.Text = Controllers.JoyPad.GetNameFor(index);}
               if(this.Name.Equals("ui_forage")){
                    var index = Controllers.JoyPad.DefaultForageIndex;
                    if(DbHandler.Globals.ForageControlOverrideController.Equals(Controllers.JoyPad.Id)){
                        index = DbHandler.Globals.ForageControlOverrideIndex;}
                    this.JoypadLabel.Text = Controllers.JoyPad.GetNameFor(index);}
                if(this.Name.Equals("ui_use_item")){
                    var index = Controllers.JoyPad.DefaultUseItemIndex;
                    if(DbHandler.Globals.UseItemControlOverrideController.Equals(Controllers.JoyPad.Id)){
                        index = DbHandler.Globals.UseItemControlOverrideIndex;}
                    this.JoypadLabel.Text = Controllers.JoyPad.GetNameFor(index);}
            }
        }

        var fakeInputEvent = new InputEventKey();
        this._Input(fakeInputEvent);

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
