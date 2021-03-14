using Godot;
using System;
using System.Collections.Generic;

public enum CutoutPlayer {BLACK_BEAR, GRIZZLY_BEAR}

public class CutOut : Node2D{
    [Export]
    public Boolean Minimized {get; set;} = true;

    [Export]
    public CutoutPlayer CutoutPlayer {get; set;} = CutoutPlayer.BLACK_BEAR;

    public Texture DialogueAvatar {get { 
        return (Texture)GD.Load(
        System.IO.Path.Combine(this.cutoutPlayerToPath[this.CutoutPlayer],
            "avatar.png"));}}

    public String DialogueName {get {
        return this.cutoutPlayerToDialogueName[this.CutoutPlayer];
    }}

    private Dictionary<CutoutPlayer, String> cutoutPlayerToPath = new Dictionary<CutoutPlayer, string>(){
        {CutoutPlayer.BLACK_BEAR, "res://media/sprites/player/cutout_templates/black_bear/"},
        {CutoutPlayer.GRIZZLY_BEAR, "res://media/sprites/player/cutout_templates/grizzly_bear/"}
    };

    private Dictionary<CutoutPlayer, String> cutoutPlayerToDialogueName { get {
        return new Dictionary<CutoutPlayer, string>(){
            {CutoutPlayer.BLACK_BEAR, this.Tr("UI_BLACK_DNAME")},
            {CutoutPlayer.GRIZZLY_BEAR, this.Tr("UI_GRIZZLY_DNAME")}};}}

    public override void _Ready(){
        this.ApplyCutoutPlayerTextureToAllChildrenSprite();
        this.ApplyMinimizedFlagToAllChildrenSprite();
    }

    public void ApplyMinimizedFlagToAllChildrenSprite(){
        this.applyMinimizedFlagToAllChildrenSpriteRecurs(this);
    }

    private void applyMinimizedFlagToAllChildrenSpriteRecurs(Node n){
        if(n is Sprite && ((Sprite)n).Texture != null){
            if(n.Name.ToLower().Contains("full")){
                ((Node2D)n).Visible = !this.Minimized;}
            if(n.Name.ToLower().Contains("min")){
                ((Node2D)n).Visible = this.Minimized;}
        }
        foreach(Node child in n.GetChildren()){
            this.applyMinimizedFlagToAllChildrenSpriteRecurs(child);
        }
    }

    public void ApplyCutoutPlayerTextureToAllChildrenSprite(){
        this.overrideTexturePathsRecurs(this);
    }

    private void overrideTexturePathsRecurs(Node n){
        if(n is Sprite && ((Sprite)n).Texture != null){
           ((Sprite)n).Texture = this.getSelectedTextureForCutoutPlayer((Sprite)n);}
        foreach(Node child in n.GetChildren()){
            this.overrideTexturePathsRecurs(child);}
    }

    private Texture getSelectedTextureForCutoutPlayer(Sprite s){
        var cutoutPlayerPathDir = this.cutoutPlayerToPath[this.CutoutPlayer];
        if(s.Texture.GetPath().Contains(cutoutPlayerPathDir) || s.Name.ToLower().Contains("atv")){
            //The existing texture is what the cutout specifies -- ignore and return the arg
            return s.Texture;}
        else{
            var filename = System.IO.Path.GetFileName(s.Texture.GetPath());
            var newPath = System.IO.Path.Combine(cutoutPlayerPathDir, filename);
            return (Texture)GD.Load(newPath);}
    }

}
