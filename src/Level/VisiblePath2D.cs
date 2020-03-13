using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class VisiblePath2D : Path2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public float PercentOfCurveToDraw = 1f;
    public int DottedSpace = 0;
    public int DottedLength = 0;
    public Color Color = new Color(1f, 1f, 1f);
    public int Width = 6;
    
    //0f // Tutorial
    //0.008f // Forest
    //0.042f //Suburbia
    //0.067f //Government
    //0.24f // Moon Base
    //0.43f //Space ship
    //0.998f //Bonus Ice
    //0.684f //Final Level

    public const int NUM_CHUNKS_NONDOTTED = 10;

    public override void _Draw(){
        var pts = this.Curve.GetBakedPoints();
        var lastIndexToTake = (int)(pts.Length * this.PercentOfCurveToDraw);
        var ptsToDraw = pts.Skip(0).Take(lastIndexToTake).ToArray();

        if(this.DottedLength == 0 && this.DottedSpace == 0){
            foreach(var chunkPts in ptsToDraw.Split(ptsToDraw.Count() / NUM_CHUNKS_NONDOTTED)){
                this.DrawPolyline(chunkPts.ToArray(), this.Color, this.Width);}
        } else {
            var startDrawI = this.DottedLength;
            var stopDrawI = this.DottedLength + this.DottedSpace;
            var drawPoint = true;
            var i = 0;
            foreach(var chunkPts in ptsToDraw.Split(this.DottedLength)){
                if(i % 2 == 0){
                    this.DrawPolyline(chunkPts.ToArray(), new Color(this.Color.r, this.Color.g, this.Color.b, 1f), this.Width);
                    this.DrawPolyline(chunkPts.ToArray(), new Color(this.Color.r, this.Color.g, this.Color.b, 0.5f), this.Width+1);}
                i++;}}}
}
