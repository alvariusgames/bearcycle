extends KinematicBody2D

var some_vec = Vector2(1,0)

func _ready():
	set_process(true)

func set_move_state(state_):
	some_vec = Vector2(0,1)

func _process(delta):
	var motion = move(some_vec)
	if(is_colliding()):
		motion = get_collision_normal().slide(motion)
		move(motion)