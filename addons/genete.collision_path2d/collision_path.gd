tool
extends Path2D

const RADIUS = 2
const COLOR = Color(255, 0, 0)
export (bool) var draw_points = true setget set_draw_points
export (int) var max_stages = 5 setget set_max_stages
export (float) var tolerance_degrees=4  setget set_tolerance_degrees
export (bool) var enabled = true


func _ready():
	update()
	pass

func _draw():
	if get_tree().is_editor_hint() and enabled:
		var collision_node=get_parent()
		if collision_node extends CollisionPolygon2D:
			var curve = get_curve()
			if curve and (curve.get_point_count() > 1):
				collision_node.set_polygon(curve.tesselate(max_stages, tolerance_degrees))
			if draw_points:
				var polygon = collision_node.get_polygon()
				if polygon.size():
					for i in range(polygon.size()):
						draw_circle(polygon[i], RADIUS, COLOR)
	pass

func set_draw_points(value):
	draw_points = value
	update()

func set_max_stages(value):
	max_stages = value
	update()

func set_tolerance_degrees(value):
	tolerance_degrees=value
	update()