extends Node

var _move_state
var _grav_state
var _pos
var _speed = 0.01
var _grav_speed = 0
var _angle_rad = 0
var _accel = 0.25
var _max_speed = 10
var _idle_effect = 0.95
var _brake_effect = 0.85
var _ground_result

func _ready():
	self.set_process(true)
	_pos = get_node("player_sprite").get_pos()
	set_move_state(get_node("/root/STATE").PLAYER.IDLE)
	set_grav_state(get_node("/root/STATE").PLAYER.FALL_ACCEL)

func set_move_state(state):
	_move_state = state

func set_grav_state(state):
	_grav_state = state

func change_state_from_environment():
	_ground_result = get_node("ground").check_if_on_ground(_pos)
	if(_ground_result.is_collision):
		if(_grav_state == get_node("/root/STATE").PLAYER.FALL_ACCEL or\
		 _grav_state == get_node("/root/STATE").PLAYER.FALL_TERM_VEL):
			set_grav_state(get_node("/root/STATE").PLAYER.ON_GROUND_AFTER_FALL)
		else:
			set_grav_state(get_node("/root/STATE").PLAYER.ON_GROUND)
	else:
		if(sin(_angle_rad)*_speed < get_node("/root/CONST").TERM_VELOCITY):
			set_grav_state(get_node("/root/STATE").PLAYER.FALL_ACCEL)
		else:
			set_grav_state(get_node("/root/STATE").PLAYER.FALL_TERM_VEL)

func react_to_state():
	if(_grav_state == get_node("/root/STATE").PLAYER.ON_GROUND):
		_transfer_momentum_from_any_grav()
		_pos = _ground_result.closest_point
		if(_move_state == get_node("/root/STATE").PLAYER.MOVE_RIGHT):
			if(_speed < _max_speed):
				_speed += _accel
		elif(_move_state == get_node("/root/STATE").PLAYER.MOVE_LEFT):
			if(_speed > -_max_speed):
				_speed -= _accel
		elif(_move_state == get_node("/root/STATE").PLAYER.IDLE):
				_speed = _speed * _idle_effect
		elif(_move_state == get_node("/root/STATE").PLAYER.BRAKE):
				_speed = _speed * _brake_effect
	elif(_grav_state == get_node("/root/STATE").PLAYER.FALL_ACCEL):
		_grav_speed += get_node("/root/CONST").GRAV_ACCEL
		_apply_grav_effect_math()
	elif(_grav_state == get_node("/root/STATE").PLAYER.FALL_TERM_VEL):
		_apply_grav_effect_math()
	elif(_grav_state == get_node("/root/STATE").PLAYER.ON_GROUND_AFTER_FALL):
		print(_ground_result.closest_point)
		_transfer_momentum_from_any_grav()

func _transfer_momentum_from_any_grav():
	var curr_angle = _angle_rad
	var angle_of_path
	if(_speed >= 0):
		angle_of_path = _ground_result.angle_to_next
	if(_speed < 0):
		angle_of_path = _ground_result.opposite_of_angle_to_prev
	_speed = cos(angle_of_path - curr_angle) * _speed
	_angle_rad = angle_of_path
	_grav_speed = 0

func _apply_grav_effect_math():
	var old_angle = _angle_rad
	var old_speed = _speed
	var new_angle = atan(\
		  (old_speed * sin(old_angle) + _grav_speed)\
		/ (old_speed * cos(old_angle)))
	var new_speed = (cos(old_angle) * old_speed) / cos(new_angle)
	_angle_rad = new_angle
	_speed = new_speed

func react_stateless():
	print(_grav_state)
	_pos.x += cos(_angle_rad) * _speed
	_pos.y += sin(_angle_rad) * _speed
	self.get_node("player_sprite").set_pos(_pos)

func _process(delta):
	self.change_state_from_environment()
	self.react_to_state()
	self.react_stateless()