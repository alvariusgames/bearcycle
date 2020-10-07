using Godot;
using System;

public class DropItemZone : KinematicBody2D, IConsumeable{
    [Export]
    public NodePath StreamBasePath {get; set;}
    public Sprite StreamBase;
    [Export]
    public NodePath StreamHighlightPath {get; set;}
    public Sprite StreamHighlight;
    [Export]
    public NodePath TopEmitterSpritePath {get; set;}
    public Sprite TopEmitter;
    public CollisionShape2D CollisionShape2D;
    public RectangleShape2D RectangleShape2D;
    // The particle effects are tuned for a rectangle of the below size
    // Since the DropItemZone instance requires the level designer to specify CollisionShape2D child,
    // Do some fancy math to make the particles project to the new size based on teh below ref size
    private float ARBITRARY_EMITTER_SPRITE_HEIGHT { get { return 150f * this.Scale.y;}}
    public override void _Ready(){
        this.StreamBase = (Sprite)this.GetNode(this.StreamBasePath);
        this.StreamHighlight = (Sprite)this.GetNode(this.StreamHighlightPath);
        this.TopEmitter = (Sprite)this.GetNode(this.TopEmitterSpritePath);
        foreach(Node child in this.GetChildren()){
            if(child is CollisionShape2D && ((CollisionShape2D)child).Shape is RectangleShape2D){
                this.CollisionShape2D = (CollisionShape2D)child;
                this.RectangleShape2D = (RectangleShape2D)this.CollisionShape2D.Shape;}}
        if(this.RectangleShape2D is null){
            throw new Exception("DropItemZone instance must have a CollisionShape2D child with a RectangleShape2D shape.");}
        
        this.adjustEmitterAndParticlesFromCollisionShapeScale();
    }

    private void adjustEmitterAndParticlesFromCollisionShapeScale(){
        var rectShapeY = this.RectangleShape2D.Extents.y / this.Scale.y;
        this.TopEmitter.Position = new Vector2(this.TopEmitter.Position.x,
            ARBITRARY_EMITTER_SPRITE_HEIGHT - rectShapeY);
        //var adjustment = this.RectangleShape2D.Extents.y / ARBITRARY_PARTICLE_RECT_SIZE_BASE.y;
        this.StreamBase.RegionRect = new Rect2(this.StreamBase.RegionRect.Position,
            new Vector2(this.StreamBase.RegionRect.Size.x, rectShapeY / this.StreamBase.Scale.y));
        this.StreamHighlight.RegionRect = new Rect2(this.StreamHighlight.RegionRect.Position,
            new Vector2(this.StreamHighlight.RegionRect.Size.x, rectShapeY / this.StreamHighlight.Scale.y));
    }

    public void Consume(Player player){
        if(player.AllHoldibles.Count != 0){
            SoundHandler.PlaySample<MyAudioStreamPlayer2D>(this,
                                                           "res://media/samples/items/phaser_dequip1.wav",
                                                           SkipIfAlreadyPlaying: true);
            foreach(var holdible in player.AllHoldibles.ToArray()){
                player.DropHoldable(holdible);}}}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
