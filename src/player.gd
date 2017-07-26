extends Node

var _state
var _pos = Vector2(50,50)
var _speed = 0
var _accel = 0.25
var _max_speed = 10
var _idle_effect = 0.95
var _brake_effect = 0.85

func _ready():
	self.set_process(true)

func set_state(state):
	_state = state

func react_to_state():
	if(_state == get_node("/root/STATE").PLAYER.MOVE_RIGHT):
		if(_speed < _max_speed):
			_speed += _accel
	elif(_state == get_node("/root/STATE").PLAYER.MOVE_LEFT):
		if(_speed > -_max_speed):
			_speed -= _accel
	elif(_state == get_node("/root/STATE").PLAYER.IDLE):
			_speed = _speed * _idle_effect
	elif(_state == get_node("/root/STATE").PLAYER.BRAKE):
			_speed = _speed * _brake_effect

func react_stateless():
	_pos.x += _speed
	self.get_node("player_sprite").set_pos(_pos)

func _process(delta):
	self.react_to_state()
	self.react_stateless()