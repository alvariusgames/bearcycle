using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class FSMKinematicBody2D<StateEnum> : KinematicBody2D, IFSMObject<StateEnum> {
    public StateEnum ActiveState {get; private set;}
    public int ActiveStatePriority {get; private set;}

    private SortedList<int, StateEnum> prevActiveStates = new SortedList<int, StateEnum>();

    public void SetActiveState(StateEnum ActiveState, int priority){
        if(priority >= this.ActiveStatePriority){
            this.prevActiveStates[this.ActiveStatePriority] = this.ActiveState;
            this.ActiveState = ActiveState;
            this.ActiveStatePriority = priority;
        }
    }

    public void UnsetActiveState(int priority){
        if(priority == this.ActiveStatePriority){
            var keys = this.prevActiveStates.Keys.ToList();
            keys.Sort();
            var highestPriorityKey = keys[keys.Count - 1];
            this.ActiveState = this.prevActiveStates[highestPriorityKey];
            this.ActiveStatePriority = highestPriorityKey;}
        }

   public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);

    public override void _PhysicsProcess(float delta) {
        this.UpdateState(delta);
        this.ReactToState(delta);
        this.ReactStateless(delta);
    }
}