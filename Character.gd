extends KinematicBody2D

func _ready():
	set_process(true)
   
func _process(delta):
	var motion = move(Vector2(1,0))
	if(is_colliding()):
		motion = get_collision_normal().slide(motion)
		move(motion)