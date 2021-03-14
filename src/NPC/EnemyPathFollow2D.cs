using Godot;
using System;

public class EnemyPathFollow2D : PathFollow2D{
    public EnemyPath2D EnemyPath2D;
    private float prevOffset = 0f;

    public override void _Ready(){
        base._Ready();
        this.EnemyPath2D = (EnemyPath2D)this.GetParent();
        this.Loop = false;
    }

    public Boolean CanAdvance(float step, float buffer = 120f){
        var output = true;
        var potentialOffset = this.Offset + step;
        foreach(var otherEnemyOffset in this.EnemyPath2D.CurrentEnemyOffsets(excluding: this)){
            var potentialDiff = Math.Abs(potentialOffset - otherEnemyOffset);
            var currentDiff = Math.Abs(this.Offset - otherEnemyOffset);
            output = output && (potentialDiff > buffer);}
        return output;
    }

    public void Advance(float step){
        this.prevOffset = this.Offset;
        this.Offset += step;
   }

    public Boolean IsAtEndOfPath = false;

    public override void _Process(float delta){
        base._Process(delta);
        this.IsAtEndOfPath = prevOffset == this.Offset;

    }

}