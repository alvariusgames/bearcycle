using Godot;
using System;

public enum ParentCollisionShapeType {POLYGON, CAPSULE, CIRCLE, RECTANGLE}
public class DisabilityVisibleNotifier : VisibilityNotifier2D{
    private ParentCollisionShapeType ParentCollisionShapeType;
    private CollisionPolygon2D parentCollisionPolygon;
    private CollisionShape2D parentCollisionShape;
    public override void _Ready(){
        base._Ready();
        var parent = this.GetParent();
        if(parent is CollisionPolygon2D){
            this.parentCollisionPolygon = (CollisionPolygon2D)parent;
            this.ParentCollisionShapeType = ParentCollisionShapeType.POLYGON;
            this.Rect = this.calculateBoundingBox(this.parentCollisionPolygon.Polygon);
        } else if(parent is CollisionShape2D){
            this.parentCollisionShape = (CollisionShape2D)parent;
            var colShape = this.parentCollisionShape;
            if(colShape.Shape is CircleShape2D){
                this.ParentCollisionShapeType = ParentCollisionShapeType.CIRCLE;
                var circle = (CircleShape2D)(colShape.Shape);
                this.Rect = new Rect2(this.Rect.Position,
                                      new Vector2(circle.Radius * 2, circle.Radius * 2));}
            else if(colShape.Shape is RectangleShape2D){
                this.ParentCollisionShapeType = ParentCollisionShapeType.RECTANGLE;
                var rect = (RectangleShape2D)colShape.Shape;
                this.Rect = new Rect2(this.Rect.Position, rect.Extents);}
            else if(colShape.Shape is CapsuleShape2D){
                this.ParentCollisionShapeType = ParentCollisionShapeType.CAPSULE;
                var caps = (CapsuleShape2D)colShape.Shape;
                this.Rect = new Rect2(this.Rect.Position,
                    new Vector2(caps.Radius*2, caps.Radius*2));}
        }
    }

    private Rect2 calculateBoundingBox(Vector2[] polygon){
        //Takes all points in the polygon and gets the Rect2 bounding box
        var min = this.Rect.Position;
        var max = this.Rect.Position + this.Rect.Size;
        foreach(var pt in polygon){
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
        this.parentCollisionPolygon.Disabled = true;
        return;
        base._Process(delta);
        if(this.GetParent().Name.Contains("display")){
            //GD.Print(this.parentCollisionPolygon.Disabled);
        }
        if(this.ParentCollisionShapeType.Equals(ParentCollisionShapeType.POLYGON)){
            this.parentCollisionPolygon.Disabled = !this.IsOnScreen();}
        else{
            this.parentCollisionShape.Disabled = !this.IsOnScreen();}
    }

}
