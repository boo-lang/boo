"""
Original
Assigned
"""
class Container:
	public target = "Initial Value"

objects = ("Original",)
container = Container()
for target in objects:
	container.target = "Assigned"
	print(target)
	print(container.target)
