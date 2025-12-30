# Credits to unlessgames - https://github.com/godotengine/godot-proposals/issues/1186#issuecomment-2974716468
class_name AudioBusReloader extends Node

@export var bus_path := "res://default_bus_layout.tres"
@export_range(0.016, 1.0) var delay_seconds := 0.05

var last_modified = 0


func _ready() -> void:
	if OS.has_feature("editor"):
		_watch_bus()
	else:
		queue_free()


func _watch_bus() -> void:
	if not FileAccess.file_exists(bus_path):
		push_error("no file at '%s'" % bus_path)
		return

	var modified = FileAccess.get_modified_time(bus_path)
	if modified != last_modified:
		last_modified = modified
		var bus_layout = load(bus_path)
		if bus_layout is AudioBusLayout:
			AudioServer.set_bus_layout(bus_layout)
		else:
			push_error("file at '%s' is not a valid AudioBusLayout" % bus_path)
			return

	await get_tree().create_timer(delay_seconds).timeout
	_watch_bus()