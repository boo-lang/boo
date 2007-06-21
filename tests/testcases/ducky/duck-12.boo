"""
0
10
"""
class Vector:
	x as double
	y as double
	
class Particle:
	position = Vector()
	
items as object = array(Particle() for i in range(3))
print items[0].position.x
items[0].position.x += 10
print items[0].position.x
