using Godot;
using System;

public class CollisionPolygonParentOfBorderedPolygon : CollisionPolygon2D{
    [Export]
    public Boolean DisableWhenNotVisible {get; set;} = true;
    private Polygon2D polygon2D;
    private VisibilityNotifier2D visibilityNotifier2D;
    public override void _Ready(){
        for(int i=0; i<this.GetChildCount(); i++){
            var child = this.GetChild(i);
            if(child is Polygon2D){
                this.polygon2D = (Polygon2D)child;
                var pts = ((Polygon2D)child).GetPolygon();
                this.Polygon = pts;}
        }
        this.visibilityNotifier2D = new VisibilityNotifier2D();
        this.AddChild(this.visibilityNotifier2D);
        this.visibilityNotifier2D.Rect = this.calculateBoundingBox();

    }

    private Rect2 calculateBoundingBox(){
        //Takes all points in the polygon and gets the Rect2 bounding box
        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(float.MinValue, float.MinValue);
        foreach(var pt in this.Polygon){
            if(pt.x < min.x){
                min = new Vector2(pt.x, min.y);}
            if(pt.y < min.y){
                min = new Vector2(min.x, pt.y);}
            if(pt.x > max.x){
                max = new Vector2(pt.x, max.y);}
            if(pt.y > max.y){
                max = new Vector2(max.x, pt.y);}
        }
        return new Rect2(min, max-min);
    }

    public override void _Process(float delta){
        base._Process(delta);
        if(!(this.visibilityNotifier2D is null) && this.DisableWhenNotVisible){
            this.Disabled = !this.visibilityNotifier2D.IsOnScreen();}
    }
}