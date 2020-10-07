using Godot;
using System;

public class KinematicPlatform : KinematicBody2D, IPlatform{
    [Export]
    public Boolean TransferMovementToPlayer {get; set;} = false;
    public Vector2 velocity = new Vector2(0,0);
    private Vector2 prevGlobalPosition = new Vector2(0,0);
    public override void _Ready(){
        this.prevGlobalPosition = this.GlobalPosition;
    }
    public override void _Process(float delta){
        if(this.TransferMovementToPlayer){
            this.velocity = (this.GlobalPosition - this.prevGlobalPosition) * delta * main.DELTA_NORMALIZER;
            this.prevGlobalPosition = this.GlobalPosition;}
    }

}
