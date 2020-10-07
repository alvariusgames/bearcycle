using Godot;
using System;
using LiteDB;

public enum PlatformType { MOBILE, TABLET, DESKTOP, NONE }
public enum Platform { ANDROID, IOS, LINUX, WINDOWS, OSX, NONE}

public class main : Node2D{
	// Member variables here, example:
	// private int a = 2;
	// private string b = "textvar";

	public const String VERSION = "v0.1.2-alpha";
	public const float DELTA_NORMALIZER = 60f;

	public static Platform Platform = Platform.NONE;
	public static PlatformType PlatformType = PlatformType.NONE;

	private float getScreenDiagInInches(){
		var diagPixels = OS.GetScreenSize().Length();
		return diagPixels / OS.GetScreenDpi();}

	private const float MAX_MOBILE_DIAG_WIDTH = 7f;

    public static Boolean IsDebug = false;
    private void setupDebug(){
        foreach(var arg in OS.GetCmdlineArgs()){
            if(arg.Contains("debug=true")){
				GD.Print("Debug mode activated");
                main.IsDebug = true;}}}

	public override void _Ready(){
        this.setupDebug();
		this.setPlatformAndPlatformType();
		DbHandler.ApplyOverrideControlsToInputMap();
		SoundHandler.SetSampleVolumeLinearUnits(DbHandler.Globals.SampleVolumeLinearUnits);
		SoundHandler.SetStreamVolumeLinearUnits(DbHandler.Globals.StreamVolumeLinearUnits);
		if(DbHandler.Globals.Locale != Globals.NO_LOCALE_SET_ARBITRARY_STRING){
			TranslationServer.SetLocale(DbHandler.Globals.Locale);}}
	private void setPlatformAndPlatformType(){
		var diagInches = getScreenDiagInInches();
		var name = OS.GetName().ToLower();
		if(name.Contains("android")){
			main.Platform = Platform.ANDROID;
			if(diagInches <= MAX_MOBILE_DIAG_WIDTH){
				main.PlatformType = PlatformType.MOBILE;
			} else {
				main.PlatformType = PlatformType.TABLET;}}
		if(name.Contains("ios")){
			main.Platform = Platform.IOS;
			GD.Print("Model:");
			GD.Print(OS.GetModelName());
			if(true || diagInches <= MAX_MOBILE_DIAG_WIDTH){
				//TODO: find a way to accurately detect iPad vs iPhone
				main.PlatformType = PlatformType.MOBILE;
			} else {
				main.PlatformType = PlatformType.TABLET;}}
		if(name.Contains("windows")){
			main.Platform = Platform.WINDOWS;
			main.PlatformType = PlatformType.DESKTOP;}
		if(name.Contains("osx")){
			main.Platform = Platform.OSX;
			main.PlatformType = PlatformType.DESKTOP;}
		if(name.Contains("x11")){
			main.Platform = Platform.LINUX;
			main.PlatformType = PlatformType.DESKTOP;}
	}

	public override void _Process(float delta){
	   //this.GetTree().ChangeScene("res://scenes/title_screen/company_logo.tscn");
	   this.GetTree().ChangeScene("res://scenes/level_select/level_select.tscn");
	   //this.GetTree().ChangeScene("res://scenes/misc/PlayArbitraryLevel.tscn");
	}

}
