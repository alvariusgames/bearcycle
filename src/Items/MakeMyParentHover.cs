using Godot;
using System;

public class MakeMyParentHover : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private float currHeightHover = 0f;
    private const float AMPL_MULT = 5f;
    private const float PERIOD_MULT = 3f;
    private Vector2 origParentGlobalPosition;
    private float i;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        this.origParentGlobalPosition = ((Node2D)this.GetParent()).GetGlobalPosition();
        var rand = new Random();
        this.i += (float)(rand.NextDouble() * Math.PI * 10f); //TODO: find way to make the floats start out at random different times...
    }

    public override void _Process(float delta){
        var parent = (Node2D)this.GetParent();
        i += delta;
        this.currHeightHover = (float)Math.Sin((double)i * PERIOD_MULT) * AMPL_MULT;

        parent.SetGlobalPosition(new Vector2(this.origParentGlobalPosition.x,
                                           this.origParentGlobalPosition.y + currHeightHover));      
  }
}
