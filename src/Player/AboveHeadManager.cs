using Godot;
using System;
using System.Collections.Generic;

public class AboveHeadManager : Node2D{
    public Player Player;
    public Sprite InteractablePromptMagGlass;
    public Sprite InteractablePromptMagBg;
    private float interactablePromptAlpha = 0f;
    private const float INTERACT_PROMPT_APPEAR_RATE = 5f;
    private const float FLOAT_DIST = -185f;
    private const float SIDEWAYS_SEPERATION = 65f;
    private List<Node2D> elementsAboveHead = new List<Node2D>();
    public override void _Ready(){
        //TODO: Evaluate if this would be cleaner with a FSM...
        this.Visible = true;
        this.Player = (Player)this.GetParent();
        if(main.PlatformType.Equals(PlatformType.DESKTOP)){
            this.Scale = new Vector2(0.75f, 0.75f);
            this.Scale = new Vector2(0.75f, 0.75f);}
        foreach(Node child in this.GetChildren()){
            if(child.Name.ToLower().Contains("nteractable")){
                this.InteractablePromptMagGlass = (Sprite)child.GetChild(1);
                this.InteractablePromptMagBg = (Sprite)child.GetChild(0);}}}
            

    public void AddAboveHead(Node2D element){
        this.elementsAboveHead.Add(element);
        element.GetParent().RemoveChild(element);
        this.AddChild(element);}

    public void RemoveAboveHead(Node2D element){
        this.elementsAboveHead.Remove(element);
        this.RemoveChild(element);}

    public void HackyClearAllAboveHead(){
        this.Visible = false;}

    public override void _Process(float delta){
        this.placeElementsAboveHead();
        this.handleInteractablePrompt(delta);
    }

    private void placeElementsAboveHead(){
        var pos = this.Player.ATV.GetDeFactoGlobalPosition();
        pos = new Vector2(pos.x, pos.y + FLOAT_DIST);
        this.GlobalPosition = pos;

        var drawInteractPrompt = this.interactablePromptAlpha > 0.05f;
        foreach(var child in this.elementsAboveHead){
            var sidewaysSeperation = 0f;
            if(drawInteractPrompt){
               sidewaysSeperation = SIDEWAYS_SEPERATION;}
            this.InteractablePromptMagBg.GlobalPosition = new Vector2(
                this.GlobalPosition.x + sidewaysSeperation,
                this.GlobalPosition.y);
            this.InteractablePromptMagGlass.GlobalPosition = new Vector2(
                this.GlobalPosition.x + sidewaysSeperation,
                this.GlobalPosition.y);
            child.GlobalPosition = new Vector2(
                this.GlobalPosition.x - sidewaysSeperation,
                this.GlobalPosition.y);
            //TODO: make this logic work with >2 elements above head
        }
        }

    public void MakeInteractablePromptTempInvisible(){
        this.interactablePromptAlpha = 0f;
        this.makeInteractablePrompt(0f);
        this.placeElementsAboveHead();}

    private void handleInteractablePrompt(float delta){
        if(this.Player.WholeBodyKinBody.IsOverInteractable){
            this.interactablePromptAlpha += delta * INTERACT_PROMPT_APPEAR_RATE;
            if(this.interactablePromptAlpha > 1f){
                this.interactablePromptAlpha = 1f;}
        } else {
            this.interactablePromptAlpha -= delta * INTERACT_PROMPT_APPEAR_RATE;
            if(this.interactablePromptAlpha < 0f){
                this.interactablePromptAlpha = 0f;}}
        this.makeInteractablePrompt(this.interactablePromptAlpha);}

    private void makeInteractablePrompt(float alpha){
        this.InteractablePromptMagGlass.SelfModulate = new Color(this.InteractablePromptMagGlass.SelfModulate.r,
                                                                 this.InteractablePromptMagGlass.SelfModulate.g,
                                                                 this.InteractablePromptMagGlass.SelfModulate.b,
                                                                 alpha);
        this.InteractablePromptMagBg.SelfModulate = new Color(this.InteractablePromptMagBg.SelfModulate.r,
                                                              this.InteractablePromptMagBg.SelfModulate.g,
                                                              this.InteractablePromptMagBg.SelfModulate.b,
                                                              alpha);

    }

}
