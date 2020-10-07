using Godot;
using System;
using System.Collections.Generic;
public enum Group {WHITE, LATINO, AFRICAN, NORTH_ASIAN, SOUTH_ASIAN};

public class Demographics{
    ///Default United States Demographics
    public float WhitePerc = 0.6f;
    public float LatinoPerc = 0.2f;
    public float AfricanPerc = 0.14f;
    public float NorthAsianPerc = 0.03f;
    public float SouthAsianPerc = 0.03f;
    public Group GetRandomGroup(){
        var r = new Random();
        var randomNum = (float)r.NextDouble();
        var aggr = this.WhitePerc;
        if(randomNum < aggr){
            return Group.WHITE;}
        aggr += this.LatinoPerc;
        if(randomNum < aggr){
            return Group.LATINO;}
        aggr += this.AfricanPerc;
        if(randomNum < aggr){
            return Group.AFRICAN;}
        aggr += this.NorthAsianPerc;
        if(randomNum < aggr){
            return Group.NORTH_ASIAN;}
        aggr += this.SouthAsianPerc;
        if(randomNum < aggr){
            return Group.SOUTH_ASIAN;}
        else{
            return Group.WHITE;}}}

public class DiversityResult {
    public Group Group;
    public Boolean IsMale;
}

public static class PCComplianceFactory{
    public static Color WhiteSkin = new Color("#f5a57a");
    public static Color[] WhiteHairs = new Color[]{
        new Color("#87460d"),
        new Color("#f1cc50"),
        new Color("#fe5900"),
        new Color("#766f66"),
    };
    public static Color LatinoSkin = new Color("#cd8544");
    public static Color[] LatinoHairs = new Color[]{
        new Color("#87460d"),
        new Color("#472a1b"),
        new Color("#766f66"),
    };
    public static Color AfricanSkin = new Color("#60340e");
    public static Color[] AfricanHairs = new Color[]{
        new Color("#241208"),
        new Color("#2c1a01"),
        new Color("#766f66"),
    };
    public static Color NorthAsianSkin = new Color("#fac27b");
    public static Color[] NorthAsianHairs = new Color[]{
        new Color("#201c17"),
        new Color("#766f66"),
    };
    public static Color SouthAsianSkin = new Color("#864e12");
    public static Color[] SouthAsianHairs = AfricanHairs;
    public static DiversityResult FillDiversityQuota(Node2D parent,
                                                     List<Node2D> skinSprites, 
                                                     List<Node2D> hairSprites,
                                                     List<Node2D> maleSprites,
                                                     List<Node2D> femaleSprites,
                                                     Demographics demographics = null){
        ///Please don't cancel me

        if(demographics is null){
            demographics = new Demographics();}
        var group = demographics.GetRandomGroup();
        var rand = new Random();
        switch(group){
            case Group.WHITE:
                modulateEachNodeWith(skinSprites, WhiteSkin);
                modulateEachNodeWith(hairSprites, 
                                     WhiteHairs[rand.Next(WhiteHairs.Length)]);
                break;
            case Group.LATINO:
                modulateEachNodeWith(skinSprites, LatinoSkin);
                modulateEachNodeWith(hairSprites, 
                                     LatinoHairs[rand.Next(LatinoHairs.Length)]);
                break;
            case Group.AFRICAN:
                modulateEachNodeWith(skinSprites, AfricanSkin);
                modulateEachNodeWith(hairSprites, 
                                     AfricanHairs[rand.Next(AfricanHairs.Length)]);
                break;
            case Group.NORTH_ASIAN:
                modulateEachNodeWith(skinSprites, NorthAsianSkin);
                modulateEachNodeWith(hairSprites, 
                                     NorthAsianHairs[rand.Next(NorthAsianHairs.Length)]);
                break;
            case Group.SOUTH_ASIAN:
                modulateEachNodeWith(skinSprites, SouthAsianSkin);
                modulateEachNodeWith(hairSprites, 
                                     SouthAsianHairs[rand.Next(SouthAsianHairs.Length)]);
                break;}

        var isMale = rand.NextBool();
        foreach(var maleSprite in maleSprites){
            if(isMale){
                maleSprite.Visible = true;}
            else{
                maleSprite.Visible = false;}}
        var isFemale = !isMale;
        foreach(var femaleSprite in femaleSprites){
            if(isFemale){
                femaleSprite.Visible = true;}
            else{
                femaleSprite.Visible = false;}}

        return new DiversityResult(){Group = group,
                                     IsMale = isMale};
    }

    private static void modulateEachNodeWith(List<Node2D> nodes, Color color){
        foreach(Node2D node in nodes){
            node.Modulate = color;}
    }

}