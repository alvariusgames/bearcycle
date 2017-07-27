extends Node

func _ready():
	pass

func check_if_on_ground(pos):
	"""Checks all Polgyon2D children's points.
	"""
	var result
	for child in self.get_children():
		result = child.check_on_polygon(pos)
		if(result.is_collision):
			return result
	return result