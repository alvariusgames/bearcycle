using Godot;
using System;

public abstract class ManualEvocableFSMNode2D<StateEnum> : FSMNode2D<StateEnum>
{
    // Exists for the use case where you want ot control when this Node's `Process` is called
    // (Needed for a bug fix of needing the ATV node to be called AFTER both of it's wheel children)
    // node were called.
    public override void _Process(float delta){
        //Don't do anything here, since we want this to be manually evoked
    }

    public void ManualProcess(float delta){
        this.UpdateState(delta);
        this.ReactStateless(delta);
        this.handleTimers(delta);
        this.ReactToState(delta);}}
