extends CanvasItem

# class member variables go here, for example:
# var a = 2
# var b = "textvar"

var curve = null

func _ready():
	pass

func draw_curve(_curve):
	curve = _curve
	self.update()

func _draw():
	if curve:
		for i in range(0, curve.get_point_count()-1):
			draw_line(curve.get_point_pos(i), curve.get_point_pos(i+1), Color(255, 0, 0), 1)