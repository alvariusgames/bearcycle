using Godot;
using System;

public enum RangerSpawnerState {WAITING_FOR_ACTIVATION, TRIGGER_ACTIVATION, ACTIVATION, SPAWNING, }

public class RangerSpawner : Spawner<RangerSpawnerState>{
    public override RangerSpawnerState InitialState {get; set;} = RangerSpawnerState.WAITING_FOR_ACTIVATION;

    public override void _Ready(){
        base._Ready();
        var masterTemplateRanger = (Ranger)this.MasterTemplateEntity;
        // Template should be the default size, which is male for humans
        masterTemplateRanger.MakeMaleSize();
    }

    protected override void _PreSpawn(Vector2 GlobalPosWhereEntityWillBeSpawned){
        this.DisplaySpawnSwirlAnimationAt(GlobalPosWhereEntityWillBeSpawned);
    }
    protected override void _PostSpawn(Node2D entity){
        entity.Rotation = 0;
    }


    public override void ReactStateless(float delta){}

    public override void ReactToState(float delta){}
    
    public override void UpdateState(float delta){}

}
