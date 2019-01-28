using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class FSMKinematicBody2D<StateEnum> : KinematicBody2D, IFSMObject<StateEnum> {
    public abstract StateEnum InitialState {get;}
    public StateEnum ActiveState {get; private set;}
    public int ActiveStatePriority {get; private set;}
    public void SetActiveState(StateEnum ActiveState, int priority){
        if(priority >= ActiveStatePriority){
            this.ActiveState = ActiveState;
            this.ActiveStatePriority = priority;}}

    public void ResetActiveState(StateEnum ActiveState){
        this.ActiveStatePriority = 0;
        this.ActiveState = ActiveState;}

    private class TimerAttrs{
        public float secondsCount;
        public float numSecondsToTriggerStateChange;
        public StateEnum stateToSetTo;
        public int priorityToSetTo;
        public Boolean callResetState;
        public TimerAttrs(float numSecondsToTriggerStateChange,
                          StateEnum stateToSetTo,
                          int priorityToSetTo,
                          bool callResetState){
            this.secondsCount = 0f;
            this.numSecondsToTriggerStateChange = numSecondsToTriggerStateChange;
            this.stateToSetTo = stateToSetTo;
            this.priorityToSetTo = priorityToSetTo;
            this.callResetState = callResetState;}}
    private List<TimerAttrs> timersSet = new List<TimerAttrs>();

    public void SetActiveStateAfter(StateEnum ActiveState, int priority, float numSeconds){
        this.timersSet.Add(new TimerAttrs(numSeconds, ActiveState, priority, false));}
    
    public void ResetActiveStateAfter(StateEnum ActiveState, float numSeconds){
        this.timersSet.Add(new TimerAttrs(numSeconds, ActiveState, -1, true));}
    public void ForceClearAllTimers(){
        this.timersSet = new List<TimerAttrs>();}

    private void handleTimers(float delta){
        for(var i=0; i<this.timersSet.Count(); i++){
            var timerAttr = this.timersSet.ElementAt(i);
            timerAttr.secondsCount = timerAttr.secondsCount + delta;
            if(timerAttr.secondsCount >= timerAttr.numSecondsToTriggerStateChange){
                if(timerAttr.callResetState){
                    this.ResetActiveState(timerAttr.stateToSetTo);
                } else {
                    this.SetActiveState(timerAttr.stateToSetTo, timerAttr.priorityToSetTo);
                }
                this.timersSet.Remove(timerAttr);}}}

    public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);

    public override void _Process(float delta) {
        this.UpdateState(delta);
        this.ReactStateless(delta);
        this.handleTimers(delta);
        this.ReactToState(delta);}}