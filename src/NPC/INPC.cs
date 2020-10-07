using Godot;
using System;

public interface INPC{
    void GetHitBy(Node node);
    bool ResetPlayerAttackWindowAfterGettingHit {get; set;}
    Node GetParent();
    void AddChild(Node node, bool legibleUniqueName = false);
    Vector2 Position {get; set;}
    Vector2 GlobalPosition {get; set;}
    SceneTree GetTree();
}

