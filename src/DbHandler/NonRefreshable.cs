using Godot;
using System;

public class NonRefreshable : INonRefreshable{
    public int Id { get; set; }
    public int LevelNum {get; set;}
    public int UUID {get; set;}
    public static NonRefreshable Default{ get {
        return new NonRefreshable(){
            Id = -1,
            LevelNum = -1,
            UUID = 1};
    }}
}
