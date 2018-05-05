using Godot;
using System;

public class PlatformCollision : CollisionPolygon2D
{
    public override void _Ready()
    {
        Polygon2D visiblePolygon = (Polygon2D)GetChild(0);
        visiblePolygon.Polygon = this.Polygon;
    }
}
