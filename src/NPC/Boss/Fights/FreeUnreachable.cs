using Godot;
using System;
using System.Collections.Generic;

public class FreeUnreachable : Node2D{
    [Export]
    public Godot.Collections.Array<NodePath> NodesToFree {get; set;}
    private Queue<Node2D> queueNodesToFree = new Queue<Node2D>();
    private Node2D currentNodeFreeing;

    public Boolean AllAreFree { get {
        return this.queueNodesToFree.Count == 0;
    }}

    public override void _Ready(){
        foreach(var nodePath in this.NodesToFree){
            this.queueNodesToFree.Enqueue(this.GetNode<Node2D>(nodePath));}
    }
    public void FreeUnreachableNodesAtOnce(){
        while(!this.AllAreFree){
            var node = this.queueNodesToFree.Dequeue();
            if(!(node.GetParent() is null)){
                node.QueueFree();}
        }
    }
    public void FreeUnreachableNodesProcess(){
        if(this.AllAreFree){
            return;}
        if(this.currentNodeFreeing is null){
            this.currentNodeFreeing = this.queueNodesToFree.Dequeue();}
        else{
            this.currentNodeFreeing.QueueFree();
            this.currentNodeFreeing = null;}
    }
}
