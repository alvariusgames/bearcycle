using Godot;
using System.Collections.Generic;
using System;
using System.Linq;

public interface ISpawner {
    int TotalNumEntities {get; set;}
    int DestroyedEntitiesCount {get; set;}
    void Activate();
    Boolean Depleted {get;}
    void Spawn(int count = 1);
    List<Node2D> CurrentlySpawnedEntities {get; set;}
}

public abstract class Spawner<T> : FSMNode2D<T>, ISpawner{
    public Node2D MasterTemplateEntity {get; set;}
    [Export]
    public NodePath MasterTemplateEntityPath {get; set;}
    [Export]
    public int TotalNumEntities { get; set;} = 0;
    public List<Node2D> CurrentlySpawnedEntities {get; set;} = new List<Node2D>();
    private int NumEntitiesSpawned = 0;
    private int stageThisNumEntitiesToSpawnNow = 0;
    private float toSpawnNowCountdownTimer = -1f;
    public int DestroyedEntitiesCount {get; set; } =  0;
    public List<Node2D> SpawnSpots = new List<Node2D>();
    [Export]
    Godot.Collections.Array<NodePath> SpawnSpotsPaths {get; set;}
    private Node2D activeSpawnSpot;
    [Export]
    public int MaxNumEntitiesSpawnedAtOnceInScene { get; set; } = 3;
    private float SpawnDelaySec {get; set;} = 0.5f;
    [Export]
    public float BwSpawnDeadSpaceSec {get; set;} = 2f;
    public bool Depleted { get { 
        return this.DestroyedEntitiesCount >= this.TotalNumEntities;}}
    public override void _Ready(){
        this.MasterTemplateEntity = (Node2D)this.GetNode(this.MasterTemplateEntityPath);
        foreach(NodePath npath in this.SpawnSpotsPaths){
            this.SpawnSpots.Add(this.GetNode<Node2D>(npath));}
    }

    public override void _Process(float delta){
        this._SpawnerBookkeepingProcess(delta);
        base._Process(delta);}

    public void Activate(){
        this.Spawn(this.MaxNumEntitiesSpawnedAtOnceInScene);
    }

    public void Spawn(int count = 1){
        if(this.NumEntitiesSpawned + count > this.TotalNumEntities){
            // Prevent spawning more entities than you're supposed to
            return;}
        this.NumEntitiesSpawned += count;
        this.stageThisNumEntitiesToSpawnNow += count;
    }
    private void _SpawnerBookkeepingProcess(float delta){
        this._TrackDestroyedEntitiesProcess(delta);
        this._SpawnNewEntitiesProcess(delta);
        this._OffscreenSwirlAnimationHandlerProcess(delta);
    }

    private void _TrackDestroyedEntitiesProcess(float delta){
        foreach(Node2D entity in this.CurrentlySpawnedEntities.ToArray()){
            if(((ISpawnable)entity).IsDestroyed){
                this.DestroyedEntitiesCount++;
                this.CurrentlySpawnedEntities.Remove(entity);
                entity.GetParent().RemoveChild(entity);
                if(this.DestroyedEntitiesCount < this.TotalNumEntities){
                    this.Spawn();
                }}}}

    private void _SpawnNewEntitiesProcess(float delta){
        if(this.toSpawnNowCountdownTimer <= 0f && this.stageThisNumEntitiesToSpawnNow > 0){
            // Someone's called StageSpawnNewEntity() -- begin spawning process
            this.toSpawnNowCountdownTimer = this.BwSpawnDeadSpaceSec + 
                                            (2 * this.SpawnDelaySec);
            this.stageThisNumEntitiesToSpawnNow--;
            this.activeSpawnSpot = this.SpawnSpots.PickRandom();
            this._PreSpawn(this.activeSpawnSpot.GlobalPosition);}
        else if(this.toSpawnNowCountdownTimer > 0f){
            this.toSpawnNowCountdownTimer -= delta;
            var timer = this.toSpawnNowCountdownTimer;
            if((timer > this.BwSpawnDeadSpaceSec) && 
               (timer < this.BwSpawnDeadSpaceSec + this.SpawnDelaySec)){
                this.toSpawnNowCountdownTimer = this.BwSpawnDeadSpaceSec;
                // Actually spawn the entity
                var entity = (Node2D)this.MasterTemplateEntity.Duplicate();
                this.CurrentlySpawnedEntities.Add(entity);
                this.AddChild(entity);
                entity.GlobalPosition = this.activeSpawnSpot.GlobalPosition;
                //entity._Ready();
                ((ISpawnable)entity).PostDuplicate(this.MasterTemplateEntity);
                this._PostSpawn(entity);}}}

    private Dictionary<Node2D, float> swirlAnimationsTimers = new Dictionary<Node2D, float>();

    private void _OffscreenSwirlAnimationHandlerProcess(float delta){
        // Animations offscreen should be auto removed after ~1 sec,
        // otherwise it will display later and looks wrong
        foreach(var key in this.swirlAnimationsTimers.Keys.ToList()){
            this.swirlAnimationsTimers[key] += delta;
            if(this.swirlAnimationsTimers[key] > SWIRL_ANIMATION_SEC){
                key.GetParent().RemoveChild(key);
                this.swirlAnimationsTimers.Remove(key);
            }
        }
    }

    public const float SWIRL_ANIMATION_SEC = 1.2f;

    public void DisplaySpawnSwirlAnimationAt(Vector2 globalPos){
        var explosion = (Particles2D)(GD.Load("res://scenes/misc/effects/spawnswirl1.tscn") as PackedScene).Instance();
        explosion.Emitting = true;
        explosion.OneShot = true;
        this.AddChild(explosion);
        explosion.ZIndex = this.ZIndex + 20;
        explosion.GlobalPosition = globalPos;
        this.swirlAnimationsTimers.Add(explosion, 0);
    }

    protected virtual void _PreSpawn(Vector2 GlobalPosWhereEntityWillBeSpawned){
        //Called right when Spawn() is called -- ~0.5 sec before actually spawning

    }

    protected virtual void _PostSpawn(Node2D entity){
        //Called right after is actually spawned

    }

}
