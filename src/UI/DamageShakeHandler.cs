using Godot;
using System;
using System.Collections.Generic;

public enum DamageShakeHandlerState { NOT_SHAKING, TRIGGER_HIT1, HIT1_PROC}
public class DamageShakeHandler : FSMNode2D<DamageShakeHandlerState>{
    [Export]
    public Godot.Collections.Array<NodePath> NodesToShakePaths {get; set;} = new Godot.Collections.Array<NodePath>();
    private List<Node2D> nodesToShake = new List<Node2D>();
    private Dictionary<Node2D, Vector2> ShakableNodesToInitialPosition = new Dictionary<Node2D, Vector2>();
    public List<Node2D> NodesToShake { get {
        if(this.nodesToShake.Count == 0){
            foreach(var nodepath in this.NodesToShakePaths){
                var node = this.GetNode<Node2D>(nodepath);
                this.nodesToShake.Add(node);}}
        return this.nodesToShake;}}

    [Export] 
    public NodePath PlayerStatsDisplayHandlerPath {get; set;}

    private PlayerStatsDisplayHandler playerStatsDisplayHandler;
    public PlayerStatsDisplayHandler PlayerStatsDisplayHandler { get {
        var parent = this.GetParent();
        while(this.playerStatsDisplayHandler == null){
            if(parent is PlayerStatsDisplayHandler){
                this.playerStatsDisplayHandler = (PlayerStatsDisplayHandler)parent;}
            parent = parent.GetParent();
        }
        return this.playerStatsDisplayHandler;}}
    public Player Player { get {
        return this.PlayerStatsDisplayHandler.activePlayer;}}

    [Export]
    public float Period {get; set;} = 30f;

    [Export]
    public float Amplitude {get; set;} = 15f;

    [Export]
    public float NumSecondsToShake {get; set;} = 0.3f;
    public override DamageShakeHandlerState InitialState { get { return DamageShakeHandlerState.NOT_SHAKING; } set {}}

    public static float NUM_SEC_TO_SHAKE = 0.3f;

    public override void _Ready(){
        base._Ready();
        DamageShakeHandler.NUM_SEC_TO_SHAKE = this.NumSecondsToShake;
        foreach(var node in this.NodesToShake){
            this.ShakableNodesToInitialPosition[node] = node.Position;}
    }

    public override void ReactStateless(float delta){
    }

    public override void UpdateState(float delta){
        if(this.Player.ATV.Bear.HitSeqTriggeredThisFrame){
            this.ResetActiveState(DamageShakeHandlerState.TRIGGER_HIT1);
        }
    }

    private float i = 0f;
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case DamageShakeHandlerState.NOT_SHAKING:
                foreach(var node in this.NodesToShake){
                    node.Position = this.ShakableNodesToInitialPosition[node];}
                break;
            case DamageShakeHandlerState.TRIGGER_HIT1:
                this.i = 0f;
                this.SetActiveState(DamageShakeHandlerState.HIT1_PROC, 100);
                this.SetActiveStateAfter(DamageShakeHandlerState.NOT_SHAKING, 100, this.NumSecondsToShake);
                break;
            case DamageShakeHandlerState.HIT1_PROC:
                this.i += delta;
                foreach(var node in this.NodesToShake){
                    var currPosOffset = -(float)Math.Sin((double)this.i * this.Period) * this.Amplitude;
                    var initialPos = this.ShakableNodesToInitialPosition[node];
                    node.Position = new Vector2(initialPos.x + currPosOffset, initialPos.y);
                    }
                break;
        }        
    }

}
