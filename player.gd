extends Node

var _state
var _pos

func _ready():
	_pos = Vector2(0,0)
	self.set_process(true)

func set_state(state):
	_state = state

func react_to_state():
	if _state == get_node("/root/STATE").PLAYER.MOVE_RIGHT:
		_pos.x += 1
	elif _state == get_node("/root/STATE").PLAYER.MOVE_LEFT:
		_pos.x -= 1

func _process(delta):
	self.react_to_state()
	self.get_node("player_sprite").set_pos(_pos)