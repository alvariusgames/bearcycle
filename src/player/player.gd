extends Node

var _state
var _pos
var _speed = 0
var _angle_rad = 0
var _accel = 0.25
var _max_speed = 10
var _idle_effect = 0.95
var _brake_effect = 0.85

func _ready():
	self.set_process(true)
	_pos = get_node("player_sprite").get_pos()

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
	var ground_result = get_node("ground").check_if_on_ground(_pos)
	if(ground_result.is_collision):
		_angle_rad = ground_result.angle_to_next
		#print(_angle_rad)
	_pos.x += cos(_angle_rad) * _speed
	_pos.y += sin(_angle_rad) * _speed
	self.get_node("player_sprite").set_pos(_pos)

func _process(delta):
	self.react_to_state()
	self.react_stateless()