extends Node

var _is_on_polygon
var _next_point
var _prev_point

func _ready():
	pass

func check_if_on_ground(pos):
	"""Checks all Polgyon2D children's points.
	"""
	for child in self.get_children():
		_is_on_polygon = child.check_on_polygon(pos)
		if(_is_on_polygon):
			_next_point = child.next_point_for(pos)
			_prev_point = child.prev_point_for(pos)
			return {"is_on_polygon" : _is_on_polygon,
					"next_point" : _next_point,
					"prev_point" : _prev_point}
	return {"is_on_polygon" : false,
			"next_point" : null,
			"prev_point" : null}