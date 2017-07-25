extends Node

func _ready():
	self.set_process(true)

func _process(delta):
	get_node("player").set_state(get_node("/root/STATE").PLAYER.MOVE_RIGHT)