using Godot;
using System;

public class HSliderVolumeControl : HSlider{
    [Export]
    public Boolean SetToSamplesVolume = false;
    [Export]
    public Boolean SetToStreamVolume = false;

    public void ValueChanged(float value){
        var linearSoundUnits = value;
        if(this.SetToSamplesVolume){
            SoundHandler.SetSampleVolumeLinearUnits(linearSoundUnits);
            SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/click_1.wav");}
        if(this.SetToStreamVolume){
            SoundHandler.SetStreamVolumeLinearUnits(linearSoundUnits, updateExistingStreams: true);}
    }

}
