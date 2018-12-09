using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class FSMNode2D<StateEnum> : Node2D, IFSMObject<StateEnum> {
    public StateEnum ActiveState {get; private set;}
    public int ActiveStatePriority {get; private set;}
    private Dictionary<int, StateEnum> prevActiveStates = new Dictionary<int, StateEnum>();

    public void SetActiveState(StateEnum ActiveState, int priority){
        if(priority >= ActiveStatePriority){
            this.prevActiveStates[this.ActiveStatePriority] = this.ActiveState;
            this.ActiveState = ActiveState;
            this.ActiveStatePriority = priority;
        }
    }

    public void UnsetActiveState(int priority){
        if(priority >= ActiveStatePriority){
            var keys = this.prevActiveStates.Keys.ToList();
            keys.Sort();
            var highestPriorityKey = keys[keys.Count - 1];
            this.ActiveState = this.prevActiveStates[highestPriorityKey];
            this.ActiveStatePriority = highestPriorityKey;}
        if(this.prevActiveStates.ContainsKey(priority)){
            this.prevActiveStates.Remove(priority);}
    }

   public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);

    public override void _Process(float delta) {
        this.UpdateState(delta);
        this.ReactStateless(delta);
        this.ReactToState(delta);
    }
}