using Godot;
using System;

public class Viewport99 : Viewport
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        foreach(var child in this.GetParent().GetChildren()){
            if(child is Sprite){
                ((Sprite)child).Texture = this.GetTexture();
            }}}

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
