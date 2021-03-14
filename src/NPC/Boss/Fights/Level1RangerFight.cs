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
    public Godot.Collections.Array<NodePath> SpawnersPaths {get; set;}

    private List<ISpawner> spawners = null;

    public List<ISpawner> Spawners { get { 
        if(this.spawners is null){
            this.spawners = new List<ISpawner>();
            foreach(var spawnerPath in this.SpawnersPaths){
                this.spawners.Add(this.GetNode<ISpawner>(spawnerPath));}}
        return this.spawners;
    }}

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
            var entities = new List<Node2D>();
            entities = entities.Concat(this.OtherRangers).ToList();
            foreach(var spawner in this.Spawners){
                entities = entities.Concat(spawner.CurrentlySpawnedEntities).ToList();}
            return entities;}
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
        this.NumFightUnitsTotal = (float)(this.OtherRangers.Count);
        foreach(var spawner in this.Spawners){
            this.NumFightUnitsTotal += spawner.TotalNumEntities;}
        this.NumFightUnitsCompleted = 0f;
        this.InitialLeftRightMask = this.LeftBoundary.CollisionMask;
        this.InitialLeftRightLayer = this.LeftBoundary.CollisionLayer;
        this.LeftBoundary.Visible = false;
        this.LeftBoundary.CollisionMask = 0;
        this.LeftBoundary.CollisionLayer = 0;
        this.RightBoundary.Visible = true;
        this.FirstRanger = (Ranger)this.GetNode(this.FirstRangerPath);
        this.FirstRanger.SetActiveState(PursuingEnemyState.LOCKED, 800);
        this.FirstRanger.SetActiveStateAfter(PursuingEnemyState.LOCKED, 800, 0.1f);
        this.FirstRanger.SetActiveStateAfter(PursuingEnemyState.LOCKED, 800, 2f);}

    public override void OnActivation(){}

    public override Boolean FightIsLoadedProcess(){
        this.FreeUnreachable.FreeUnreachableNodesProcess();
        return this.FreeUnreachable.AllAreFree;
    }

    public override Boolean IsFightOver(float delta){
        return this.OtherRangers.Count() <= 0 &&
               this.Spawners.All(x => x.Depleted);
    }

    public override void StartOfFight(float delta){
        this.LeftBoundary.Visible = true;
        this.LeftBoundary.CollisionMask = this.InitialLeftRightMask;
        this.LeftBoundary.CollisionLayer = this.InitialLeftRightLayer;
        this.RightBoundary.Visible = true;
        this.RightBoundary.CollisionMask = this.InitialLeftRightMask;
        this.RightBoundary.CollisionLayer = this.InitialLeftRightLayer;
        this.moveFirstRangerToFirstPath2D();
        this.startSpawnerReverseTimer = 2f;
    }

    private void moveFirstRangerToFirstPath2D(){
        EnemyPath2D firstEnemyPath2D = null;
        foreach(var child in this.Spawners[0].GetChildren()){
            if(child is EnemyPath2D){
                firstEnemyPath2D = (EnemyPath2D)child;}}
        var enemyPathFollow2D = firstEnemyPath2D.PlaceEnemyPathFollow2D();
        this.FirstRanger.GetParent().RemoveChild(this.FirstRanger);
        enemyPathFollow2D.AddChild(this.FirstRanger);
        enemyPathFollow2D.UnitOffset = 0.6f; //harcoded to ranger 1 starting point
        this.FirstRanger.PursuitEnemyPattern = PursuitEnemyPattern.FOLLOWING_PATH2D;
        this.FirstRanger.Position = new Vector2(0,0);
        this.FirstRanger.ResetActiveState(PursuingEnemyState.ATTACKING);
    }

    public override void _FightProcess(float delta){
        this.startSpawnerBookkeeping(delta);
        this.manageOtherRangersAfterDestroyed(delta);
        this.NumOtherRangersDestroyed = this.InitialOtherRangersCount - 
            this.OtherRangers.Count();
        this.NumFightUnitsCompleted = 
            (float)(this.NumOtherRangersDestroyed);
        foreach(var spawner in this.Spawners){
            this.NumFightUnitsCompleted += spawner.DestroyedEntitiesCount;}
    }
    private float startSpawnerReverseTimer = -1f;
    private void startSpawnerBookkeeping(float delta){
        this.startSpawnerReverseTimer -= delta;
        if(this.startSpawnerReverseTimer < 0f && this.startSpawnerReverseTimer > -5f){
            foreach(var spawner in this.Spawners){
                spawner.Activate();}
            this.startSpawnerReverseTimer = -10f;} 
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
