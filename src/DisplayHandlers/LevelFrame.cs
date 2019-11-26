using Godot;
using System;

public class LevelFrame : Node{

    private PauseMenu PauseMenu;
    public Viewport Viewport;

    public override void _Ready(){
        foreach(Node child in this.GetChildren()){
            if(child is PauseMenu){
                this.PauseMenu = (PauseMenu)child;}
            if(child is ViewportContainer){
                this.Viewport = (Viewport)(child.GetChild(0));
            }}}

    public override void _Process(float delta){
        if(Input.IsActionJustPressed("ui_pause")){
            this.PauseMenu.OpenPauseMenu();}}}
