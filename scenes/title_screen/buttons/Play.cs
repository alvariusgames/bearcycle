using Godot;
using System;

public class Play : TouchScreenButton
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.SetProcessInput(true);
       /*
     func _ready():
    set_process_input(true)


func _input(event):
    if (event.type == InputEvent.SCREEN_TOUCH): _show_buttons() 
        */ 
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Input(InputEvent @event){
        GD.Print(@event);
        if((@event.GetType().Equals(typeof(InputEventMouseButton))) || (@event.GetType().Equals(typeof(InputEventScreenTouch)))){
            GD.Print("I am touched");
        }
    }

}
