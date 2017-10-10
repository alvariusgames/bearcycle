extends Node

func _ready():
	set_process_input(true)

func _is_keyboard(event):
	return event.type == InputEvent.KEY

func _keyboard_left_not_right(event):
	return _is_keyboard(event)\
		and Input.is_key_pressed(KEY_LEFT)\
		and (not Input.is_key_pressed(KEY_RIGHT))

func _some_form_left_not_right(event):
	return _keyboard_left_not_right(event)

func _keyboard_right_not_left(event):
	return _is_keyboard(event)\
		and Input.is_key_pressed(KEY_RIGHT)\
		and (not Input.is_key_pressed(KEY_LEFT))

func _some_form_right_not_left(event):
	return _keyboard_right_not_left(event)

func _keyboard_not_left_and_not_right(event):
	return _is_keyboard(event)\
		and (not Input.is_key_pressed(KEY_LEFT))\
		and (not Input.is_key_pressed(KEY_RIGHT))

func _some_form_not_left_and_not_right(event):
	return _keyboard_not_left_and_not_right(event)

func _keyboard_left_and_right(event):
	return _is_keyboard(event)\
		and (Input.is_key_pressed(KEY_LEFT))\
		and (Input.is_key_pressed(KEY_RIGHT))

func _some_form_left_and_right(event):
	return _keyboard_left_and_right(event)

func player_move_right():
	get_node("player").set_move_state(\
		get_node("/root/STATE").PLAYER.MOVE_RIGHT)

func player_move_left():
	get_node("player").set_move_state(\
		get_node("/root/STATE").PLAYER.MOVE_LEFT)

func player_idle():
	get_node("player").set_move_state(\
		get_node("/root/STATE").PLAYER.IDLE)

func player_brake():
	get_node("player").set_move_state(\
		get_node("/root/STATE").PLAYER.BRAKE)

func _input(event):
	if(_some_form_right_not_left(event)):
		player_move_right()
	elif(_some_form_left_not_right(event)):
		player_move_left()
	elif(_some_form_left_and_right(event)):
		player_brake()
	elif(_some_form_not_left_and_not_right(event)):
		player_idle()