using Godot;
using System;

public class LevelFrameBannerBase : Sprite {
    public Boolean BlinkRed = true;
    public Boolean PlaySoundWhenBlinking = true;
    private int i;
    public Color InitialModulate;
    public Color InitialSelfModulate;

    public override void _Ready(){
        this.InitialModulate = this.Modulate;
        this.InitialSelfModulate = this.SelfModulate;}

    public void ResetToDefaultColor(){
        this.Modulate = this.InitialModulate;
        this.SelfModulate = this.InitialSelfModulate;}

    public void ModulateToRedNow(){
        this.Modulate = new Color(0.9f,0f,0f);
        this.SelfModulate = new Color(0.9f, 0.9f, 0.9f);
    }

    public override void _Process(float delta){
        if(BlinkRed){
            i+=(int)(10f*delta*main.DELTA_NORMALIZER);
            var mod = this.Modulate;
            if(i<=255){
                mod.r8 = i;
                mod.b8 = 255 - mod.r8;
                this.Modulate = mod;}
            if(i<270 && i>240){
                if(this.PlaySoundWhenBlinking){
                SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                    new string[]{"res://media/samples/player/low_health_alarm.wav"},
                    VolumeMultiplier: 0.1f,
                    SkipIfAlreadyPlaying: true);}}
            if(i>255 && i<=512){
                mod.r8 = 512-i;
                mod.b8 = 255 - mod.r8;
                this.Modulate = mod;}
            else if(i>512){
                i = 0;}
            this.SelfModulate = new Color(0.5f + mod.r, 0.5f + mod.r, 0.5f + mod.r, this.InitialModulate.a);
        } else {
            i=0;}}}
