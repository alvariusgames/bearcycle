extends Node

func _ready():
	set_process_input(true)

func _input(event):
	if(event.type == InputEvent.KEY):
		if(event.scancode == KEY_RIGHT):
			get_node("player").set_state(\
				get_node("/root/STATE").PLAYER.MOVE_RIGHT)
		elif(event.scancode == KEY_LEFT):
			get_node("player").set_state(\
				get_node("/root/STATE").PLAYER.MOVE_LEFT)