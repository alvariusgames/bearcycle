using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class Extensions{
    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size){
        for (var i = 0; i < (float)array.Length / size; i++){
        yield return array.Skip(i * size).Take(size);}}

    public static String RemoveTrailingNumbers(this String s){
        string pattern = @"\d+$"; //find numbers at end of string
        string replacement = "";
        Regex rgx = new Regex(pattern);
        return rgx.Replace(s, replacement);}

    public static String RemoveNumbersAndTranslateNodeName(this Node n){
        string pattern = @"\d+$"; //find numbers at end of string
        string replacement = "";
        Regex rgx = new Regex(pattern);
        var noNumbersName = rgx.Replace(n.Name, replacement);
        noNumbersName = noNumbersName.Replace("@", "");
        return n.Tr(noNumbersName.ToUpper());}

    public static bool NextBool(this Random r, int truePercentage = 50){
        return r.NextDouble() < truePercentage / 100.0;}


    private static Random rnd;
    public static T PickRandom<T>(this IEnumerable<T> source){
        if(Extensions.rnd is null){
            Extensions.rnd = new Random();}
        var index = Extensions.rnd.Next(source.Count());
        return source.ElementAt(index);}

    public static Color mixTwoColors(this Node2D node, Color c1, Color c2, float mix){
        return new Color((c1.r * mix) + (c2.r * (1-mix)),
                         (c1.g * mix) + (c2.g * (1-mix)),
                         (c1.b * mix) + (c2.b * (1-mix)));
    }


}