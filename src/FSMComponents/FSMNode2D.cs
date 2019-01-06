using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class FSMNode2D<StateEnum> : Node2D, IFSMObject<StateEnum> {
    public StateEnum ActiveState {get; private set;}
    public int ActiveStatePriority {get; private set;}
    public void SetActiveState(StateEnum ActiveState, int priority){
        if(priority >= ActiveStatePriority){
            this.ActiveState = ActiveState;
            this.ActiveStatePriority = priority;}}

    public void ResetActiveState(StateEnum ActiveState){
        this.ActiveStatePriority = 0;
        this.ActiveState = ActiveState;}

    private struct TimerAttrs{
        public float secondsCount;
        public float numSecondsToTriggerStateChange;
        public StateEnum stateToSetTo;
        public int priorityToSetTo;
        public TimerAttrs(float numSecondsToTriggerStateChange,
                          StateEnum stateToSetTo,
                          int priorityToSetTo){
            this.secondsCount = 0f;
            this.numSecondsToTriggerStateChange = numSecondsToTriggerStateChange;
            this.stateToSetTo = stateToSetTo;
            this.priorityToSetTo = priorityToSetTo;}}
    private List<TimerAttrs> timersSet = new List<TimerAttrs>();

    public void SetActiveStateAfter(StateEnum ActiveState, int priority, float numSeconds){
        this.timersSet.Add(new TimerAttrs(numSeconds, ActiveState, priority));}

    private void handleTimers(float delta){
        for(var i=0; i<this.timersSet.Count(); i++){
            var timerAttr = this.timersSet[i];
            timerAttr.secondsCount += delta;
            if(timerAttr.secondsCount >= timerAttr.numSecondsToTriggerStateChange){
                this.SetActiveState(timerAttr.stateToSetTo, timerAttr.priorityToSetTo);
                this.timersSet.Remove(timerAttr);}}}

    public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);

    public override void _Process(float delta) {
        this.UpdateState(delta);
        this.ReactStateless(delta);
        this.handleTimers(delta);
        this.ReactToState(delta);}}