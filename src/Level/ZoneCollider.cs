using Godot;
using System;
using System.Collections.Generic;

public class ZoneCollider : KinematicBody2D{
    public List<int> ZoneCollisionMaskBits = new List<int>();
    [Export]
    public int priority = 100;
    public static int[] AllCollisionZones = {2, 9, 10};

    [Export]
    Boolean ZoneOne = true;
    [Export]
    Boolean ZoneTwo = true;
    [Export]
    Boolean ZoneThree = true;

    public static Dictionary<int, int> ZoneNumToCollisionLayer = new Dictionary<int, int> {
        {1, 2},  //L_PlatformZoneOne
        {2, 9}, //L_PlatformZoneTwo
        {3, 10} //L_PlatformZoneThree
    };

    public override void _Ready(){
        if(this.ZoneOne){
            this.ZoneCollisionMaskBits.Add(ZoneNumToCollisionLayer[1]);}
        if(this.ZoneTwo){
            this.ZoneCollisionMaskBits.Add(ZoneNumToCollisionLayer[2]);}
        if(this.ZoneThree){
            this.ZoneCollisionMaskBits.Add(ZoneNumToCollisionLayer[3]);}}
}