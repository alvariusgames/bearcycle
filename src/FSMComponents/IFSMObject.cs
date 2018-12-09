
public interface IFSMObject<StateEnum> {
    StateEnum ActiveState {get;}
    int ActiveStatePriority {get;}
    void SetActiveState(StateEnum ActiveState, int priority);
    void ResetActiveState(StateEnum ActiveState);
    void UpdateState(float delta);
    void ReactStateless(float delta);
    void ReactToState(float delta);
}