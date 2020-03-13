using Godot;
using System;

public class MakeMyParentHover : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private float currHeightHover = 0f;
    public float Amplitude = 5f;
    public float Period = 3f;
    public Boolean HoverVertical = true;
    public Boolean HoverHorizontal = false;
    public Boolean Reverse = false;
    public Boolean randomStartAmpl = true;
    public Boolean RandomStartAmpl { get { return this.randomStartAmpl;}
                                     set { this.randomStartAmpl = value;
                                           if(this.randomStartAmpl){
                                               var rand = new Random();
                                               this.i = (float)((-0.5f + rand.NextDouble()) * Math.PI);}
                                           else {
                                               this.i = 0f;}}}
    private Vector2 origParentGlobalPosition;
    private float i;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        this.origParentGlobalPosition = ((Node2D)this.GetParent()).GetPosition();
        this.RandomStartAmpl = this.RandomStartAmpl;
        if(this.Name.ToLower().Contains("vert")){
            this.HoverVertical = true;}
        if(this.Name.ToLower().Contains("notvert")){
            this.HoverVertical = false;}
        if(this.Name.ToLower().Contains("horiz")){
            this.HoverHorizontal = true;}
        if(this.Name.ToLower().Contains("nothoriz")){
            this.HoverHorizontal = false;}
    }

    public override void _Process(float delta){
        var parent = (Node2D)this.GetParent();
        i += delta;
        if(this.Reverse){
            this.currHeightHover = -(float)Math.Sin((double)i * Period) * Amplitude;}
        else{
            this.currHeightHover = (float)Math.Sin((double)i * Period) * Amplitude;}

        var newOffset = new Vector2(0f,0f);
        if(this.HoverVertical){
            newOffset += new Vector2((float)Math.Sin(parent.Rotation)*currHeightHover,
                                     (float)Math.Cos(parent.Rotation)*currHeightHover);}
        if(this.HoverHorizontal){
            newOffset += new Vector2((float)Math.Cos(parent.Rotation)*currHeightHover,
                                     (float)Math.Sin(parent.Rotation)*currHeightHover);}

        parent.Position = new Vector2(this.origParentGlobalPosition.x - newOffset.x,
                                            this.origParentGlobalPosition.y - newOffset.y); 
  }
}
