extends Polygon2D

var pts

func _ready():
	pts = []

func _draw():
	for i in range(0, pts.size()-1):
		draw_line(pts[i], pts[i+1], Color(255, 0, 0), 1)