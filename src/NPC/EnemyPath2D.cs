using Godot;
using System;
using System.Collections.Generic;

public class EnemyPath2D : Path2D{
    public IEnumerable<float> CurrentEnemyOffsets(Node excluding = null){
        foreach(var child in this.GetChildren()){
            if(child is EnemyPathFollow2D &&
               !child.Equals(excluding)){
                yield return ((EnemyPathFollow2D)child).Offset;}}
    }

    public EnemyPathFollow2D PlaceEnemyPathFollow2D(){
        //Places an EnemyPathFollow2D on this Path, avoiding collisions
        var maxNumAttempts = 10;
        EnemyPathFollow2D pathFollow2D = null;
        while(maxNumAttempts > 0){
            pathFollow2D = new EnemyPathFollow2D();
            this.AddChild(pathFollow2D);
            pathFollow2D.UnitOffset = (float)(new Random()).NextDouble();
            if(pathFollow2D.CanAdvance(0) ||
               maxNumAttempts == 1){
                maxNumAttempts = 0;}
            else{
                this.RemoveChild(pathFollow2D);
                maxNumAttempts--;}}
        return pathFollow2D;
    }

}