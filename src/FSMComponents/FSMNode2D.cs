using Godot;
using System;

public abstract class FSMNode2D<StateEnum> : Node2D, IFSMObject<StateEnum> {
    private FSMObject<StateEnum> FSMObject;
    public StateEnum ActiveState {get; private set;}
    public int ActiveStatePriority {get; private set;}
    public void SetActiveState(StateEnum ActiveState, int priority) {
        FSMObject.SetActiveState(ActiveState, priority);}
    public void UnsetActiveState(StateEnum ActiveState, int priority){
        FSMObject.UnsetActiveState(ActiveState, priority);}
    public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);

    public override void _Process(float delta) {
        this.UpdateState(delta);
        this.ReactStateless(delta);
        this.ReactToState(delta);
    }
}