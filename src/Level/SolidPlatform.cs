using Godot;
using System;

public class SolidPlatform : CollisionPolygon2D
{
    public override void _Ready(){
        for(int i=0; i<this.GetChildCount(); i++){
            var child = this.GetChild(i);
            if(child is Polygon2D){
                var scale = 1f;
                var pts = ((Polygon2D)child).GetPolygon();
                for(int j=0; j<pts.Length; j++){
                    pts[j] *= scale;
                }
                this.Polygon = pts;
                
                //Polygon2D visiblePolygon = (Polygon2D)GetChild(0);
                //visiblePolygon.Polygon = this.Polygon;
                return;}}}
}
