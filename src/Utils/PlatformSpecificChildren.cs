using Godot;
using System;

public class PlatformSpecificChildren : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    protected Godot.Collections.Array GetPlatformSpecificChildren(){
        foreach(Node2D child in this.GetChildren()){
            if(child.Name.ToLower().Contains("mobile") && main.PlatformType == PlatformType.MOBILE){
                child.Visible = true;
                return child.GetChildren();}
            if(child.Name.ToLower().Contains("desktop") && main.PlatformType == PlatformType.DESKTOP){
                child.Visible = true;
                return child.GetChildren();}
            if(child.Name.ToLower().Contains("android") && main.Platform == Platform.ANDROID){
                child.Visible = true;
                return child.GetChildren();}}
        throw new Exception("Could not find any nodes for this platform!");
    }

    public void PopulateChildrenWithPlatformSpecificNodes(Node2D Node){
        foreach(Node child in this.GetPlatformSpecificChildren()){
            child.GetParent().RemoveChild(child);
            Node.AddChild(child);}
        // Delete all other nodes
        foreach(Node2D child in this.GetChildren()){
            this.RemoveChild(child);
            child.QueueFree();}
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
