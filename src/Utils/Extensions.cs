using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


/// <summary>
/// from https://stackoverflow.com/questions/18986129/c-splitting-an-array-into-n-parts
/// Splits an array into several smaller arrays.
/// </summary>
/// <typeparam name="T">The type of the array.</typeparam>
/// <param name="array">The array to split.</param>
/// <param name="size">The size of the smaller arrays.</param>
/// <returns>An array containing smaller arrays.</returns>

public static class Extensions{
    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size){
        for (var i = 0; i < (float)array.Length / size; i++){
        yield return array.Skip(i * size).Take(size);}}

    public static String RemoveTrailingNumbers(this String s){
        string pattern = @"\d+$"; //find numbers at end of string
        string replacement = "";
        Regex rgx = new Regex(pattern);
        return rgx.Replace(s, replacement);
    }

    public static String RemoveNumbersAndTranslateNodeName(this Node n){
        string pattern = @"\d+$"; //find numbers at end of string
        string replacement = "";
        Regex rgx = new Regex(pattern);
        var noNumbersName = rgx.Replace(n.Name, replacement);
        return n.Tr(noNumbersName.ToUpper());
    }

}