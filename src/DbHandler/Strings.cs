using Godot;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public static class Strings{
    //https://docs.godotengine.org/en/3.1/tutorials/i18n/locales.html#doc-locales
    private static JObject ui = null;

    public static JObject UI { get { 
        if(Strings.ui == null){
            var file = new File();
            file.Open(Strings.UIFilePath, File.ModeFlags.Read);
            Strings.ui = JObject.Parse(file.GetAsText());}
        return Strings.ui;}}

    public static IEnumerable<String> SupportedLocales(){
        var dir = new Directory();
        dir.Open("res://strings/ui/");
        dir.ListDirBegin();
        while(true){
            var file = dir.GetNext();
            if(file == ""){
                break;
            } else if(!file.BeginsWith(".") && file.EndsWith(".json")){
                yield return System.IO.Path.GetFileNameWithoutExtension(file);}
        }
    }

    public static void ClearCache(){
        Strings.ui = null;
    }
    private static String UIFilePath { get { return "res://strings/ui/" + DbHandler.Globals.Locale + ".json"; }}

    public static readonly String[] LatinLangs = new String[]{
        "en", "en_US", "es",
    };
    public const String LatinFont = "res://media/fonts/en_US.tres";

    public static readonly String[] KoreanLangs = new String[]{
        "ko", "ko_KR"
    };
    public const String KoreanFont = "res://media/fonts/ko_KR.tres";
    public static readonly String[] JapaneseLangs = new String[]{
        "ja", "ja_JP"
    };
    public const String JapaneseFont = "res://media/fonts/ja_JP.tres";
    public static readonly String[] SimplifiedChineseLangs = new String[]{
        "zh", "zh_CN"
    };
    public const String SimplifiedChineseFont = "res://media/fonts/zh_CN.tres";
    public static readonly String[] HindiLangs = new String[]{
        "hi", "hi_IN"
    };
    public const String HindiFont = "res://media/fonts/hi_IN.tres";
    public static readonly String[] KannadaLangs = new String[]{
        "kn_IN",
    };
    public const String KannadaFont = "res://media/fonts/kn_IN.tres";
    private static Dictionary<string, String[]> FontsToLangs = new Dictionary<string, String[]>(){
        {LatinFont, LatinLangs},
        {KoreanFont, KoreanLangs},
        {JapaneseFont, JapaneseLangs},
        {SimplifiedChineseFont, SimplifiedChineseLangs},
        {HindiFont, HindiLangs},
        {KannadaFont, KannadaLangs}
    };

    public static Font CompatibleFont { get {
        var fontToLang = FontsToLangs.FirstOrDefault(x => x.Value.Contains(DbHandler.Globals.Locale));
        return (Font)GD.Load(fontToLang.Key);}}

    public static String LocaleToLanguageString(string locale){ 
        var lookup = new Dictionary<String, String> {
            {"en_US", "American English"}};
        return lookup[locale];}

}