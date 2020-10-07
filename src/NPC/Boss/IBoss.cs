using Godot;
using System;

public enum BossType { SWARM_OF_ENEMIES }
public interface IBoss {
    BossType BossType{get;}
    int NumHealth{get;set;}
    int MaxHealth{get;}
    Boolean IsDefeated{get;set;}
}
