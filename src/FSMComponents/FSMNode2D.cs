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

    public void ResetActiveState(StateEnum ActiveState){
        this.ActiveStatePriority = 0;
        this.ActiveState = ActiveState;
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