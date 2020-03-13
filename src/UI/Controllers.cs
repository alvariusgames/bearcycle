using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Button{
    public Button(uint index, String name){
        this.index = index;
        this.name = name;}
    public uint index;
    public String name;}

public interface IController{
    uint Id {get;}
    uint DefaultAttackIndex{ get; }
    uint DefaultForageIndex{ get;}
    uint DefaultUseItemIndex{get;}
    String GetNameFor(uint index);
    Button None { get ;}}

public class Keyboard : IController {
        public uint Id { get { return 1; }}
        public uint DefaultAttackIndex { get { return 32;}}
        public uint DefaultForageIndex { get { return 16777238;}}
        public uint DefaultUseItemIndex {get { return 16777240;}}
        public Button None { get { return new Button(0, "");}}
        public String GetNameFor(uint index){
            return OS.GetScancodeString(index);}}
    public class XBox : IController{
        public Button None { get { return new Button(0, "");}}
        public uint Id { get { return 2;}}
        public uint DefaultAttackIndex { get { return this.A.index;}}
        public uint DefaultForageIndex { get { return this.B.index;}}
        public uint DefaultUseItemIndex {get { return this.Y.index;}}
        public Button A = new Button(0,"A");
        public Button B = new Button(1,"B");
        public Button X = new Button(2, "X");
        public Button Y = new Button(3, "Y");
        public Button LB = new Button(4, "LB");
        public Button RB = new Button(5, "RB");
        public Button LT = new Button(6, "LT");
        public Button RT = new Button(7, "RT");
        public List<Button> Buttons { get {
            return new List<Button>(){
            this.A,
            this.B,
            this.X,
            this.Y,
            this.RB,
            this.LB,
            this.RT,
            this.LT,
            };}}
        public String GetNameFor(uint index){
            var output = this.Buttons.Where(x => x.index == index).FirstOrDefault();
            if(output != null){
                return output.name;}
            else{
                return this.None.name;}}}


public static class Controllers{
    public static IController Keyboard { get { return new Keyboard();}}
    public static IController JoyPad{ get {
        //In the future, do fancier logic depending on what platform we're in
        return new XBox();}}
}