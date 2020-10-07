using Godot;
using System;

public enum BounceParentOpacityFormulas { 
    RAW_SINE_WAVE, // 'Always on'
    QUARTIC_OPACITY_MULTIPLIER //Used by AboveHeadManager (can't remember why)
    }
public class BounceParentsOpacity : Node2D{
    [Export]
    public float startOpac {get; set; } = 0f;
    [Export]
    public float stopOpac {get; set; } = 1f;
    [Export]
    public BounceParentOpacityFormulas Formula {get; set;} = 
        BounceParentOpacityFormulas.RAW_SINE_WAVE;
    private float amplitude = 1f;
    private Node2D parent;
    private const float MULT = 3f;
    private float i = 0f;
    public override void _Ready(){
        this.parent = (Node2D)this.GetParent();
        this.amplitude = this.stopOpac - this.startOpac;}

    public override void _Process(float delta){
        i += delta;
        if(!this.Visible){
            this.i = 0f;}
        var newAlpha = (this.startOpac + (this.amplitude * 0.5f)) + ((float)Math.Sin(i * MULT) * this.amplitude  * 0.5f);
        if(this.Formula.Equals(BounceParentOpacityFormulas.QUARTIC_OPACITY_MULTIPLIER)){
            newAlpha *= (float)Math.Pow(this.parent.SelfModulate.a, 4);}
        this.parent.SelfModulate = new Color(this.parent.SelfModulate.r,
                                             this.parent.SelfModulate.g,
                                             this.parent.SelfModulate.b,
                                             newAlpha);
    }
}
