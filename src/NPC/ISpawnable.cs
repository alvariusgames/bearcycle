using Godot;

public interface ISpawnable {
    bool IsDestroyed {get; set;}
    void PostDuplicate(Node2D masterTemplate);
}
