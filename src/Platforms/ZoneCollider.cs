using Godot;
using System;
using System.Collections.Generic;

public class ZoneCollider : KinematicBody2D{
    public List<int> ZoneCollisionMaskBits = new List<int>();
    public int priority = -1;
    public int[] allCollisionZones = {2, 9, 10};

    private Dictionary<int, int> zoneNumToCollisionLayer = new Dictionary<int, int> {
        {1, 2},  //L_PlatformZoneOne
        {2, 9}, //L_PlatformZoneTwo
        {3, 10} //L_PlatformZoneThree
    };

    public override void _Ready(){
       var name = this.Name.ToLower();
       if(name.Contains("all")){
           foreach(int i in allCollisionZones){
               this.ZoneCollisionMaskBits.Add(i);}}
       if(name.Contains("one") && !name.Contains("notone")){
           this.ZoneCollisionMaskBits.Add(zoneNumToCollisionLayer[1]);}
       if(name.Contains("two") && !name.Contains("nottwo")){
           this.ZoneCollisionMaskBits.Add(zoneNumToCollisionLayer[2]);}
       if(name.Contains("three") && !name.Contains("notthree")){
           this.ZoneCollisionMaskBits.Add(zoneNumToCollisionLayer[3]);}
       if(name.Contains("notone")){
           this.ZoneCollisionMaskBits.Remove(zoneNumToCollisionLayer[1]);}
       if(name.Contains("nottwo")){
           this.ZoneCollisionMaskBits.Remove(zoneNumToCollisionLayer[2]);}
       if(name.Contains("notthree")){
           this.ZoneCollisionMaskBits.Remove(zoneNumToCollisionLayer[3]);}
        foreach(var child in this.GetChildren()){
            if(child is CollisionPolygon2D){
                this.priority = Convert.ToInt32(((Node)child).Name);}}}}
