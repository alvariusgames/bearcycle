using Godot;
using System;

public interface IInteractable {
    int InteractPriority { get;}
    void InteractWith(Player player,float delta);
}
