using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Level1RangerFight : BossFightManager{
    [Export]
    public NodePath FirstRangerPath {get; set;}
    public Ranger FirstRanger;
    [Export]
    Godot.Collections.Array<NodePath> OtherRangersPaths;
    private List<Ranger> otherRangers;
    public List<Ranger> OtherRangers { get {
        if(this.otherRangers is null){
            this.otherRangers = new List<Ranger>();
            foreach(var npath in this.OtherRangersPaths){
                this.otherRangers.Add(this.GetNode<Ranger>(npath));}}
        return this.otherRangers;}}
    public int InitialOtherRangersCount { get {
        return this.OtherRangersPaths.Count();
    }}
    public int NumOtherRangersDestroyed = 0;

    [Export]
    public NodePath Spawner1Path {get; set;}
    private ISpawner spawner1;
    public ISpawner Spawner1 { get { 
        if(this.spawner1 is null){
            this.spawner1 = this.GetNode<ISpawner>(this.Spawner1Path);}
        return this.spawner1;}}

    [Export]
    public NodePath Spawner2Path {get; set;}
    private ISpawner spawner2;
    public ISpawner Spawner2 { get {
        if(this.spawner2 is null){
            this.spawner2 = this.GetNode<ISpawner>(this.Spawner2Path);}
        return this.spawner2;}}

    [Export]
    public NodePath LeftBoundaryPath {get; set;}
    private StaticBody2D leftBoundary;
    public StaticBody2D LeftBoundary { 
        get { 
            if(this.leftBoundary is null){
                this.leftBoundary = this.GetNode<StaticBody2D>(this.LeftBoundaryPath);}
            return this.leftBoundary;}
        set {
            if(this.leftBoundary is null){
                this.leftBoundary = this.GetNode<StaticBody2D>(this.LeftBoundaryPath);}
            this.leftBoundary = value;
        }}
    private uint InitialLeftRightMask = 0;
    private uint InitialLeftRightLayer = 0;

    [Export]
    public NodePath RightBoundaryPath {get; set;}
    private StaticBody2D rightBoundary;
    public StaticBody2D RightBoundary { 
        get { 
            if(this.rightBoundary is null){
                this.rightBoundary = this.GetNode<StaticBody2D>(this.RightBoundaryPath);}
            return this.rightBoundary;}
        set {
            if(this.rightBoundary is null){
                this.rightBoundary = this.GetNode<StaticBody2D>(this.RightBoundaryPath);}
            this.rightBoundary = value;
        }}

    public override float NumFightUnitsCompleted {get; set;} = 0f;
    public override float NumFightUnitsTotal {get; set; } = 1f;
    private List<Node2D> emptyList = new List<Node2D>();
    private List<Node2D> endLevelList;
    public List<Node2D> EndLevelList { get {
        if(this.endLevelList == null){
            this.endLevelList = new List<Node2D>(){this.Level.EndLevel};}
        return this.endLevelList;
    }}
    public override IEnumerable<Node2D> NodesToTrack { get {
        if(this.ActiveState.Equals(BossFightManagerState.FIGHT)){
            return this.Spawner1.CurrentlySpawnedEntities.
                   Concat(this.Spawner2.CurrentlySpawnedEntities).
                   Concat(this.OtherRangers);}
        if(this.ActiveState.Equals(BossFightManagerState.DEFEATED)){
            return this.EndLevelList;
        } else {
            return this.emptyList;            
        }
    }}
    public override bool ShouldTrackNodesNow { get {
        return (this.ActiveState.Equals(BossFightManagerState.FIGHT)) || 
               (this.ActiveState.Equals(BossFightManagerState.DEFEATED));
    }}

    public override void _Ready(){
        base._Ready();
        this.NumFightUnitsTotal = (float)(this.Spawner1.TotalNumEntities + 
                                          this.Spawner2.TotalNumEntities + 
                                          this.OtherRangers.Count);
        this.NumFightUnitsCompleted = 0f;
        this.InitialLeftRightMask = this.LeftBoundary.CollisionMask;
        this.InitialLeftRightLayer = this.LeftBoundary.CollisionLayer;
        this.LeftBoundary.Visible = false;
        this.LeftBoundary.CollisionMask = 0;
        this.LeftBoundary.CollisionLayer = 0;
        this.RightBoundary.Visible = true;
        this.FirstRanger = (Ranger)this.GetNode(this.FirstRangerPath);
        this.FirstRanger.SetActiveStateAfter(PursuingEnemyState.LOCKED, 800, 2f);}
    public override Boolean IsFightOver(float delta){
        return this.Spawner1.Depleted && this.Spawner2.Depleted && 
               this.OtherRangers.Count() <= 0;
    }

    public override void StartOfFight(float delta){
        this.LeftBoundary.Visible = true;
        this.LeftBoundary.CollisionMask = this.InitialLeftRightMask;
        this.LeftBoundary.CollisionLayer = this.InitialLeftRightLayer;
        this.RightBoundary.Visible = true;
        this.RightBoundary.CollisionMask = this.InitialLeftRightMask;
        this.RightBoundary.CollisionLayer = this.InitialLeftRightLayer;
        this.Spawner1.Activate();
        this.Spawner2.Activate();
        this.FirstRanger.SetActiveState(PursuingEnemyState.MOVING_TOWARD_PLAYER_GROUND, 800);
        this.FirstRanger.SetActiveStateAfter(PursuingEnemyState.ATTACKING, 800, 2f);
        this.FirstRanger.ResetActiveStateAfter(PursuingEnemyState.ATTACKING, 4f);
    }

    public override void _FightProcess(float delta){
        this.manageOtherRangersAfterDestroyed(delta);
        this.NumOtherRangersDestroyed = this.InitialOtherRangersCount - 
            this.OtherRangers.Count();
        this.NumFightUnitsCompleted = 
            (float)(this.spawner1.DestroyedEntitiesCount +
                    this.spawner2.DestroyedEntitiesCount +
                    this.NumOtherRangersDestroyed);
    }

    private void manageOtherRangersAfterDestroyed(float delta){
        foreach(var ranger in this.OtherRangers.ToArray()){
            if(ranger.IsDestroyed){
                this.OtherRangers.Remove(ranger);}}}

    public override void EndOfFight(float delta){
        this.RightBoundary.Visible = false;
        this.RightBoundary.CollisionMask = 0;
        this.RightBoundary.CollisionLayer = 0;
    }

}
