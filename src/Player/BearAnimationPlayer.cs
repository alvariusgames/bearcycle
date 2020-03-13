using Godot;
using System;
using System.Linq;

public class BearAnimationPlayer : AnimationPlayer
{
    public Bear Bear;
    public Sprite RootSprite;

    public override void _Ready(){
        this.Bear = (Bear)this.GetParent().GetParent();
        foreach(Node cutoutChild in this.GetParent().GetChildren()){
            if(cutoutChild is Sprite && cutoutChild.Name.ToLower().Contains("root")){
                this.RootSprite = (Sprite)cutoutChild;}}
    }

    public void AdvancedPlay(string animation, Boolean skipIfAlreadyPlaying = false, string thenPlay = "idleBounce1"){
        if(skipIfAlreadyPlaying && this.AreInterchangeable(animation, this.CurrentAnimation) && this.IsPlaying()){
            return;}
        this.RootSprite.Visible = true;
        float seekPosition = 0f;
        if(this.IsPlaying() &&
           this.AreInSameGroups(animation, this.CurrentAnimation)){
                seekPosition = this.Bear.AnimationPlayer.CurrentAnimationPosition;}
        this.Play(animation);
        this.ClearQueue();
        this.Queue(thenPlay);
        if(seekPosition != 0f){
            this.Bear.AnimationPlayer.Seek(seekPosition);}
        }

    public void AdvancedPlay(string[] animations, Boolean skipIfAlreadyPlaying = false, float afterSec = 0){
        Random random = new Random();
        var animation = animations[random.Next(0, 
                                   animations.Length)];
        this.AdvancedPlay(animation, skipIfAlreadyPlaying);
    }

    public Boolean AreInterchangeable(String bearAnimName1, String BearAnimName2){
        return bearAnimName1.RemoveTrailingNumbers() == BearAnimName2.RemoveTrailingNumbers();
    }

    public Boolean AreInSameGroups(String bearAnimName1, String BearAnimName2){
        var groups = new String[2][];
        groups[0] = new String[] { "attack1", "attack_backwards1", "attack_down1", "attack_up1"};
        groups[1] = new String[] {"idle", "idleBounce1"};
        foreach(var group in groups){
            if(group.Contains(bearAnimName1) && group.Contains(BearAnimName2)){
                return true;}}
        return false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
