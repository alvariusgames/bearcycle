using Godot;
using System;
using System.Collections.Generic;

// Use https://docs.microsoft.com/en-us/cpp/c-runtime-library/language-strings?view=vs-2019
public class Globals{
    public const int SINGLETON_GLOBALS_ID = 311; // do not change me
    public int Id { get{ return SINGLETON_GLOBALS_ID; } set{}}

    public String GameVersion { get {return main.VERSION;} set {}}
    public String Locale {get; set;}
    public String ActiveGameSlotNum {get; set;}
    public Boolean PerformanceMode {get; set;}
    public float StreamVolumeLinearUnits {get; set;}
    public float SampleVolumeLinearUnits {get; set;}
    public uint AttackControlOverrideIndex {get; set;}
    public uint AttackControlOverrideController {get; set;}

    public uint ForageControlOverrideIndex {get; set;}
    public uint ForageControlOverrideController {get; set;}

    public uint UseItemControlOverrideIndex {get; set;}
    public uint UseItemControlOverrideController {get; set;}

    public static String NO_LOCALE_SET_ARBITRARY_STRING = "NO_LOCALE_SET_318";

    public static Globals Default { get {
        return new Globals(){
            Locale = NO_LOCALE_SET_ARBITRARY_STRING,
            ActiveGameSlotNum = "1",
            PerformanceMode = false,
            StreamVolumeLinearUnits = SoundHandler.DEFAULT_STREAM_VOLUME_LINEAR,
            SampleVolumeLinearUnits = SoundHandler.DEFAULT_SAMPLE_VOLUME_LINEAR,
            AttackControlOverrideIndex = Controllers.Keyboard.DefaultAttackIndex,
            AttackControlOverrideController = Controllers.Keyboard.Id,
            ForageControlOverrideIndex = Controllers.Keyboard.DefaultForageIndex,
            ForageControlOverrideController = Controllers.Keyboard.Id,
            UseItemControlOverrideIndex = Controllers.Keyboard.DefaultUseItemIndex,
            UseItemControlOverrideController = Controllers.Keyboard.Id,};}}}