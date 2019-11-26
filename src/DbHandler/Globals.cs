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
    public static Globals Default { get {
        return new Globals(){
            Locale = "en_US",
            ActiveGameSlotNum = "1"};}}}