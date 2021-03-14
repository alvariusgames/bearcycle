using Godot;
using System;
using System.Collections.Generic;

public interface ITimestepInterpolatable {
    Vector2 GlobalPosition {get; set;}
    float GlobalRotation {get; set;}
    float PrevGlobalRotation {get; set;}
    Vector2 PrevGlobalPosition {get; set;}
    Godot.Collections.Array<NodePath> DisplayNode2DPaths {get; set;}
    List<Node2D> DisplayNode2Ds {get;}
    Dictionary<Node2D, Vector2> DisplayNodeOffsets {get;set;}
}

public static class TimestepInterpolatorExtensions{
    public static void _InterpolateReady(this ITimestepInterpolatable interpObj){
        foreach(var node2D in interpObj.DisplayNode2Ds){
            interpObj.DisplayNodeOffsets[node2D] = 
                node2D.GlobalPosition - interpObj.GlobalPosition;
        }
    }

    public static void _InterpolateProcess(this ITimestepInterpolatable interpObj, float delta){
        if(!((Node)interpObj).IsInsideTree()){
            return;}
        var fraction = Engine.GetPhysicsInterpolationFraction();
        var rotation = interpObj.GlobalRotation;
        if(Math.Abs(interpObj.PrevGlobalRotation - interpObj.GlobalRotation) < 1f){
            rotation = (interpObj.PrevGlobalRotation * fraction) + 
                       (interpObj.GlobalRotation * (1-fraction));
        }
        foreach(var node2D in interpObj.DisplayNode2Ds){
            var offset = interpObj.DisplayNodeOffsets[node2D].Rotated(rotation);
            node2D.GlobalPosition = interpObj.PrevGlobalPosition.LinearInterpolate(
                interpObj.GlobalPosition, fraction) + offset;
        }
        interpObj.PrevGlobalPosition = interpObj.GlobalPosition;
        interpObj.PrevGlobalRotation = interpObj.GlobalRotation;
    }
}