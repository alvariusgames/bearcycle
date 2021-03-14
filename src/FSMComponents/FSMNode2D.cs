using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class FSMNode2D<StateEnum> : Node2D, IFSMObject<StateEnum>, ITimestepInterpolatable{
    [Export]
    public Godot.Collections.Array<NodePath> DisplayNode2DPaths {get; set;}
    private List<Node2D> displayNode2Ds = new List<Node2D>();
    public List<Node2D> DisplayNode2Ds { get {
        if(!(this.DisplayNode2DPaths is null)){
            this.displayNode2Ds = new List<Node2D>();
            foreach(var nodePath in this.DisplayNode2DPaths){
                this.displayNode2Ds.Add(this.GetNode<Node2D>(nodePath));}}
        return this.displayNode2Ds;
    }}

    public Vector2 PrevGlobalPosition {get; set;}
    public float PrevGlobalRotation {get; set;}
    public Dictionary<Node2D, Vector2> DisplayNodeOffsets {get; set;} = new Dictionary<Node2D, Vector2>();

    [Export]
    public FSMProcessType ProcessType {get;set;} = FSMProcessType.PROCESS;
    public abstract StateEnum InitialState {get; set;}
    public StateEnum ActiveState {get; set;}
    public int ActiveStatePriority {get; set;}
    public List<TimerAttrs<StateEnum>> timersSet {get; set; } = new List<TimerAttrs<StateEnum>>();
    public Boolean runOnce {get; set;} = true;
    public abstract void UpdateState(float delta);
    public abstract void ReactStateless(float delta);
    public abstract void ReactToState(float delta);
    public override void _Ready(){
        base._Ready();
        this._InterpolateReady();
    }
    public override void _Process(float delta){
        base._Process(delta);
        if(this.ProcessType.Equals(FSMProcessType.PROCESS)){
            this.FSMProcess<StateEnum>(delta);
            this._InterpolateProcess(delta);}}
    public override void _PhysicsProcess(float delta){
        base._PhysicsProcess(delta);
        if(this.ProcessType.Equals(FSMProcessType.PHYSICS_PROCESS)){
            this.FSMProcess<StateEnum>(delta);
            this._InterpolateProcess(delta);}
    }
} 