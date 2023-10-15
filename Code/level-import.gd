@tool

func post_import(level: LDTKLevel) -> LDTKLevel:
	# Behaviour goes here
	#set z index
	level.z_index = -20
	#set parentage
	for layernode in level.get_children():
		layernode.owner = level
		#do groups
		match layernode.name:
			"Floor":
				layernode.add_to_group("Floor", true)
			"Walls":
				layernode.add_to_group("Wall", true)
			"Wall_Curves":
				layernode.add_to_group("Wall", true)
			"Level_Border":
				layernode.add_to_group("Wall", true)
	# pack level into scene
	return level
