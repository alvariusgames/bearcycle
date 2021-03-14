using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum InputDirection {RIGHT, UP_RIGHT, UP, UP_LEFT, LEFT, DOWN_LEFT, DOWN, DOWN_RIGHT, NONE}
public class FourDirectJoystick : TouchScreenButton{

    public Sprite Base;
    public Sprite Activation;
    public Sprite Joystick;
    public Sprite Markings;
    public int TouchIndex = -1;
    public const float INPUT_RANGE_MIN_THRESH = 10f;
    public const float INPUT_RANGE_MAX_THRESH = 300f;
    public const float JOYSTICK_MOVE_THRESH = 80f;
    private Vector2 InitialJoystickPos = new Vector2(-1,-1);
    public override void _Ready(){
        foreach(Node2D child in this.GetChildren()){
            if(child is Sprite){
                if(child.Name.ToLower().Contains("base")){
                    this.Base = (Sprite)child;}
                if(child.Name.ToLower().Contains("activation")){
                    this.Activation = (Sprite)child;}
                if(child.Name.ToLower().Contains("joystick")){
                    this.Joystick = (Sprite)child;
                    this.InitialJoystickPos = this.Joystick.Position;}
                if(child.Name.ToLower().Contains("markings")){
                    this.Markings = (Sprite)child;}}}}

    public override void _Input(InputEvent @event){
        var pos = new Vector2(-1,-1);
        var index = -1;
        var isButtonRelease = false;
        if(@event is InputEventScreenDrag){
            var e = (InputEventScreenDrag)@event;
            pos = e.Position;
            index = e.Index;
            isButtonRelease = false;}
        if(@event is InputEventScreenTouch){
            var e = (InputEventScreenTouch)@event;
            pos = e.Position;
            index = e.Index;
            isButtonRelease = !e.IsPressed();}
        if(pos != new Vector2(-1,-1)){
            // The event is either an InputEventScreenTouch or InputEventScreenDrag
            if(this.TouchIndex == -1){
                // We have started touching, start tracking this finger
                this.TouchIndex = index;}
            if(this.TouchIndex != index){
                // The other finger is pressing something, ignore
                return;}
            var isInInputRange = this.isInInputRange(pos);
            if(isInInputRange){
                // Send out the 'press down signals!
                this.checkMoveSignalFrom(pos, isButtonRelease);}
            if(isButtonRelease || !isInInputRange){
                // Send out the 'release' signals and cleanup
                this.TouchIndex = -1;
                this.Joystick.Position = this.InitialJoystickPos;
                FourDirectJoystick.SendButtonReleaseToAllDirections();
                this.Activation.Visible = false;
            }
        }
    }

    public static void SendButtonReleaseToAllDirections(String[] but = null){
        //TODO: make this less of a hack, seperate out logic to own class?
        if(but == null){
            but = new String[]{};}
        foreach(var actionStr in new String[]{"ui_right", "ui_up", "ui_left", "ui_down"}){
            if(!but.Contains(actionStr)){
                var ev = new InputEventAction();
                ev.Action = actionStr;
                ev.Pressed = false;
                Input.ParseInputEvent(ev);}}}

    private void checkMoveSignalFrom(Vector2 pos, Boolean isButtonRelease){
        //If we're here, we want to evaluate if this click pos
        //should send out a `ui_right` etc. signal
        var angle = this.Base.GlobalPosition.AngleToPoint(pos);
        var inputInfo = this.getInputInfoFromAngle(angle);
        FourDirectJoystick.SendButtonReleaseToAllDirections(but: inputInfo.Item2);
        foreach(var actionStr in inputInfo.Item2){
            var ev = new InputEventAction();
            ev.Action = actionStr;
            ev.Pressed = !isButtonRelease;
            Input.ParseInputEvent(ev);}
        this.DrawJoystickAndActivation(inputInfo);
    }

    private void DrawJoystickAndActivation(Tuple<InputDirection, string[]> inputInfo){
        this.Activation.Visible = true;
        if(inputInfo.Item1.Equals(InputDirection.RIGHT) || 
           inputInfo.Item1.Equals(InputDirection.UP_RIGHT) || 
           inputInfo.Item1.Equals(InputDirection.DOWN_RIGHT)){
            this.Activation.RotationDegrees = 0f;
            this.Joystick.Position = new Vector2(this.InitialJoystickPos.x + JOYSTICK_MOVE_THRESH,
                                                 this.InitialJoystickPos.y);}
        if(inputInfo.Item1.Equals(InputDirection.UP)){
            this.Activation.RotationDegrees =  270f;
            this.Joystick.Position = new Vector2(this.InitialJoystickPos.x,
                                                 this.InitialJoystickPos.y - JOYSTICK_MOVE_THRESH);}
        if(inputInfo.Item1.Equals(InputDirection.LEFT) ||
           inputInfo.Item1.Equals(InputDirection.UP_LEFT) ||
           inputInfo.Item1.Equals(InputDirection.DOWN_LEFT)){
            this.Activation.RotationDegrees = 180f;
            this.Joystick.Position = new Vector2(this.InitialJoystickPos.x - JOYSTICK_MOVE_THRESH,
                                                 this.InitialJoystickPos.y);}
        if(inputInfo.Item1.Equals(InputDirection.DOWN)){
            this.Activation.RotationDegrees = 90f;
            this.Joystick.Position = new Vector2(this.InitialJoystickPos.x,
                                                 this.InitialJoystickPos.y + JOYSTICK_MOVE_THRESH);}

    }


    private Tuple<InputDirection, String[]> getInputInfoFromAngle(float angleRad){
        if(this.isInThreshRange(0, angleRad)){
            return Tuple.Create(InputDirection.RIGHT, new String[]{"ui_right"});}
        if(this.isInThreshRange(1, angleRad)){
            return Tuple.Create(InputDirection.DOWN_RIGHT, new String[]{"ui_right","ui_down"});}
        if(this.isInThreshRange(2, angleRad)){
            return Tuple.Create(InputDirection.DOWN, new String[]{"ui_down"});}
        if(this.isInThreshRange(3, angleRad)){
            return Tuple.Create(InputDirection.DOWN_LEFT, new String[]{"ui_down", "ui_left"});}
        if(this.isInThreshRange(4, angleRad)){
            return Tuple.Create(InputDirection.LEFT, new String[]{"ui_left"});}
        if(this.isInThreshRange(5, angleRad)){
            return Tuple.Create(InputDirection.UP_LEFT, new String[]{"ui_left", "ui_up"});}
        if(this.isInThreshRange(6, angleRad)){
            return Tuple.Create(InputDirection.UP, new String[]{"ui_up"});}
        if(this.isInThreshRange(7, angleRad)){
            return Tuple.Create(InputDirection.UP_RIGHT, new String[]{"ui_up", "ui_right"});}
        else{
            return Tuple.Create(InputDirection.NONE, new String[]{});
        }
    }

    private Boolean isInThreshRange(int stepsAlongCircle, float angleRad){
        var octantSlice = Math.PI / 4f;
        var offset = -(Math.PI + (Math.PI / 8f));
        
        var lowerAngleThresh = offset + stepsAlongCircle * octantSlice;
        var upperAngleThresh = offset + (stepsAlongCircle+1) * octantSlice;

        if(stepsAlongCircle == 0){
            var angleRad2 = angleRad - (Math.PI * 2f);
            return ((angleRad > lowerAngleThresh) && (angleRad < upperAngleThresh)) ||
                   ((angleRad2 > lowerAngleThresh) && (angleRad2 < upperAngleThresh));
        }
        return (angleRad > lowerAngleThresh) && (angleRad < upperAngleThresh);
    }

    private Boolean isInInputRange(Vector2 pos){
        var dist = this.Base.GlobalPosition.DistanceTo(pos);
        return dist > INPUT_RANGE_MIN_THRESH && dist < INPUT_RANGE_MAX_THRESH;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
