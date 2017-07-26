extends Polygon2D

# class member variables go here, for example:
# var a = 2
# var b = "textvar"

func _ready():
	# Called every time the node is added to the scene.
	# Initialization here
	pass

func check_on_polygon(pos):
	return true

func next_point_for(pos):
	return Vector2(0,0)

func prev_point_for(pos):
	return Vector2(0,0)