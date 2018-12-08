using Godot;
using System;

public abstract class FSMKinematicBody2D<StateEnum> : KinematicBody2D, IFSMObject<StateEnum> {
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

    public override void _PhysicsProcess(float delta) {
        this.UpdateState(delta);
        this.ReactStateless(delta);
        this.ReactToState(delta);
    }
}