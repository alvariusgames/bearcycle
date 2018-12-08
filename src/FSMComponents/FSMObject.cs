using Godot;
using System;
using System.Collections.Generic;

public abstract class FSMObject<StateEnum> : IFSMObject<StateEnum> {
    public StateEnum ActiveState {get; private set;}
    public int ActiveStatePriority {get; private set;}
    private SortedList<int, StateEnum> prevActiveStates = new SortedList<int, StateEnum>();
    public void SetActiveState(StateEnum ActiveState, int priority){
        if(priority > ActiveStatePriority){
            this.prevActiveStates.Add(this.ActiveStatePriority, this.ActiveState);
            this.ActiveState = ActiveState;
            this.ActiveStatePriority = priority;
        }
    }

    public void UnsetActiveState(StateEnum ActiveState, int priority){
        if(priority >= ActiveStatePriority){
            var lastElementIndex = this.prevActiveStates.Count - 1;
            this.ActiveState = this.prevActiveStates[lastElementIndex];
            this.ActiveStatePriority = this.prevActiveStates.Keys[lastElementIndex];
            this.prevActiveStates.RemoveAt(lastElementIndex);
        }
    }

    public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);
}