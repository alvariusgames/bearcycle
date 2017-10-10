extends KinematicBody2D

var _move_state = null
var velocity = Vector2()
var _speed = 0.01
var _grav_speed = 0
var _angle_rad = 0
var _accel = 20
var _grav = 500
var _max_speed = 700
var _idle_effect = 0.95
var _brake_effect = 0.85

func _ready():
	set_process(true)

func set_move_state(state_):
	_move_state = state_

func _react_to_state(delta):
	velocity.y += delta * _grav
	if(_move_state == get_node("/root/STATE").PLAYER.MOVE_RIGHT):
		if(velocity.x < _max_speed):
			velocity.x += _accel
	elif(_move_state == get_node("/root/STATE").PLAYER.MOVE_LEFT):
		if(velocity.x > -_max_speed):
			velocity.x -= _accel
	elif(_move_state == get_node("/root/STATE").PLAYER.IDLE):
			velocity.x = velocity.x * _idle_effect
	elif(_move_state == get_node("/root/STATE").PLAYER.BRAKE):
			velocity.x = velocity.x * _brake_effect
	

func _process(delta):
	_react_to_state(delta)
	var motion = velocity * delta
	motion = move(motion)
	if (is_colliding()):
		var n = get_collision_normal()
		motion = n.slide(motion)
		velocity = n.slide(velocity)
		move(motion)

