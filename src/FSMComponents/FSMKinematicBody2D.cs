using Godot;
using System;
using System.Collections.Generic;
public abstract class FSMKinematicBody2D<StateEnum> : KinematicBody2D, IFSMObject<StateEnum> {
    public abstract StateEnum InitialState {get; set;}
    public StateEnum ActiveState {get; set;}
    public int ActiveStatePriority {get; set;}
    public List<TimerAttrs<StateEnum>> timersSet {get; set; } = new List<TimerAttrs<StateEnum>>();
    public Boolean runOnce {get; set;} = true;
    public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);
    public override void _Process(float delta){
        this.FSMProcess<StateEnum>(delta);
    }
} 
