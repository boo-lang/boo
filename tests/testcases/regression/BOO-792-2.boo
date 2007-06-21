"""
Container.target
Original
"""
class Container:
	def target():
		print "Container.target"

objects = ("Original",)
container = Container()
for target in objects:
	container.target()
	print(target)
