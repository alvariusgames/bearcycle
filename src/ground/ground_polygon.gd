extends Polygon2D

var points = []

func _add_offset(pt):
	return Vector2(pt.x + self.get_pos().x,\
				   pt.y + self.get_pos().y)

func _ready():
	for pt in self.get_polygon():
		points.append(_add_offset(pt))

func _point_is_in_range(point, v1, v2):
	return (v1.x <= point.x and point.x <= v2.x)\
		or (v2.x <= point.x and point.x <= v1.x) 

func _perp_dot_prod(a, b, c):
	return abs((a.x - c.x) * (b.y - c.y)\
			    - (a.y - c.y) * (b.x - c.x));

const epsilon_const = 0.1

func _epsilon_of_line(v1, v2):
	var dx1 = v2.x - v1.x
	var dy1 = v2.y - v1.y
	return epsilon_const\
		* (dx1 * dx1 + dy1 * dy1)

func _is_point_on_line_ppd(point, v1, v2):
	if not _point_is_in_range(point, v1, v2):
		return false
	return ( _perp_dot_prod(point, v1, v2)\
		< _epsilon_of_line(v1, v2) )

class GroundResult:
	var is_collision
	var angle_to_next
	var opposite_of_angle_to_prev

	func _init(_is_collision, _angle_to_next, _opposite_of_angle_to_prev):
		is_collision = _is_collision
		angle_to_next = _angle_to_next
		opposite_of_angle_to_prev = _opposite_of_angle_to_prev

func _angle_to_next(next_pt, curr_pt):
	return atan((curr_pt.y - next_pt.y) / (curr_pt.x - next_pt.x))

func check_on_polygon(pos):
	var collisions = []
	for i in range(0, points.size()):
		var curr_pt = points[i]
		var next_pt = points[(i + 1) % points.size()]
		if _is_point_on_line_ppd(pos, next_pt, curr_pt):
			collisions.append(GroundResult.new(true,
							  _angle_to_next(next_pt, curr_pt),
							  _angle_to_next(next_pt, curr_pt)))
	if(collisions.size() > 0):
		if(collisions.size() > 1):
			print(collisions[0].angle_to_next)
			print(collisions[1].angle_to_next)
			print("-----")
			return GroundResult.new(true,
									collisions[1].angle_to_next,
									collisions[0].opposite_of_angle_to_prev)
		return collisions[0]
	return GroundResult.new(false, null, null)