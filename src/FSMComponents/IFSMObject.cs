using System;
using System.Collections.Generic;
using System.Linq;

public interface IFSMObject<StateEnum> {
    StateEnum InitialState{get; set;}
    StateEnum ActiveState {get; set;}
    int ActiveStatePriority {get; set;}
    List<TimerAttrs<StateEnum>> timersSet {get; set;}
    Boolean runOnce {get; set;}
    void UpdateState(float delta);
    void ReactStateless(float delta);
    void ReactToState(float delta);
}

public class TimerAttrs<StateEnum>{
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


public static class IFSMObjectExtensions{
    //Slightly complex workaround for multiple inheritance needed -- All `IFSMObject` inheriting classes will
    //get this record keeping logic to manage state, while being able to specify their 1 own Godot-specific parent class
    public static void SetActiveState<StateEnum>(this IFSMObject<StateEnum> fsmObj, StateEnum ActiveState, int priority){
        if(priority >= fsmObj.ActiveStatePriority){
            fsmObj.ActiveState = ActiveState;
            fsmObj.ActiveStatePriority = priority;}}

    public static void ResetActiveState<StateEnum>(this IFSMObject<StateEnum> fsmObj, StateEnum ActiveState){
        fsmObj.ActiveStatePriority = 0;
        fsmObj.ActiveState = ActiveState;}

    public static void SetActiveStateAfter<StateEnum>(this IFSMObject<StateEnum> fsmObj, StateEnum ActiveState, int priority, float numSeconds){
        fsmObj.timersSet.Add(new TimerAttrs<StateEnum>(numSeconds, ActiveState, priority, false));}
    public static void ResetActiveStateAfter<StateEnum>(this IFSMObject<StateEnum> fsmObj, StateEnum ActiveState, float numSeconds){
        fsmObj.timersSet.Add(new TimerAttrs<StateEnum>(numSeconds, ActiveState, -1, true));}
    public static void ForceClearAllTimers<StateEnum>(this IFSMObject<StateEnum> fsmObj){
        fsmObj.timersSet = new List<TimerAttrs<StateEnum>>();}

    public static void handleTimers<StateEnum>(this IFSMObject<StateEnum> fsmObj, float delta){
        for(var i=0; i<fsmObj.timersSet.Count(); i++){
            var timerAttr = fsmObj.timersSet.ElementAt(i);
            timerAttr.secondsCount = timerAttr.secondsCount + delta;
            if(timerAttr.secondsCount >= timerAttr.numSecondsToTriggerStateChange){
                if(timerAttr.callResetState){
                    fsmObj.ResetActiveState(timerAttr.stateToSetTo);
                } else {
                    fsmObj.SetActiveState(timerAttr.stateToSetTo, timerAttr.priorityToSetTo);
                }
                fsmObj.timersSet.Remove(timerAttr);}}}
    public static void FSMProcess<StateEnum>(this IFSMObject<StateEnum> fsmObj, float delta) {
        if(fsmObj.runOnce){
            fsmObj.ResetActiveState(fsmObj.InitialState);
            fsmObj.runOnce = false;}
        fsmObj.UpdateState(delta);
        fsmObj.ReactStateless(delta);
        fsmObj.handleTimers(delta);
        fsmObj.ReactToState(delta);}


}
